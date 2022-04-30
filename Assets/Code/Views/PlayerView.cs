using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : BaseController
{

    public float StartOmega = 1500f; // Начальная скорость вращения
    public float StartVelocity = 100f; // Начальная скорость движения
    public float deltaVelocity = 2.5f; // скорость убывания движения
    public float angleCurvePath = 25f;
    public float deltaBoosterVelocity = 0.2f; // Изменение скорости (в долях единицы) от текущей, при столкновении с объектами-бустерами


    public float CurrentOmega;// текущая скорость вращения
    public float CurrentVelocity; // Текущая линейная скорость волчка

    protected float LimitOmega = 0.1f; // Обороты, при меньшем значении снимается ограничение на отклонение от оси Y
    protected float maxAngleY = 5f; // Макс отклонение от вертикальной оси при вращении до достижении скорости вращения LimitOmega

    protected Rigidbody _rigidbody;
    protected Transform _transform;

    private SubscriptionProperty<PlayerState> _playerState;

    private Vector3 MomemtumOnBeginHit;
    private float TimeBeginHit;

    public void Init(SubscriptionProperty<PlayerState> playerState)
    {
        StartVelocity = PlayerParams.StartVelocity;
        deltaVelocity = PlayerParams.DeltaVelocity;
        StartOmega = PlayerParams.StartOmega;



        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _playerState = playerState;

        _playerState.SubscribeOnChange(OnChangePlayerState);

        CurrentOmega = StartOmega;
        CurrentVelocity = StartVelocity;

        UpdateManager.SubscribeToUpdate(ChangeVerticalAngle);
        UpdateManager.SubscribeToUpdate(UpdateRotate);

    }

    private void OnChangePlayerState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.NotStart:
                break;
            case PlayerState.Start:
                UpdateManager.SubscribeToUpdate(UpdatePositionTop);
                break;
            case PlayerState.SlowMotion:
                break;
            case PlayerState.Stop:
                UpdateManager.UnsubscribeFromUpdate(ChangeVerticalAngle);
                UpdateManager.UnsubscribeFromUpdate(UpdateRotate);
//                _playerState.UnSubscriptionOnChange(OnChangePlayerState);
                break;
            default:
                break;
        }

    }

    private void UpdatePositionTop()
    {
        // Уменьшаем скорость на deltaVelocity
        if (CurrentVelocity < 0) // Волчок остановился!!!!!!!
        {
            //Debug.Log("КОНЕЦ");
            UpdateManager.UnsubscribeFromUpdate(UpdatePositionTop);
            _playerState.Value = PlayerState.Stop;
            StartCoroutine(WaitDelayBeforeDrop(3f)); // Конец сцены
            return;
        }
        CurrentVelocity -= deltaVelocity * Time.deltaTime;

        // нехрен летать, приземляем на площадку
        if (_transform.position.y > 0.2f)
            _transform.position = new Vector3(_transform.position.x, 0.2f, _transform.position.z);

        // Изменим частоту вращения
        //float angleChangeVelocity = CurrentOmega - StartOmega * CurrentVelocity / StartVelocity;
        CurrentOmega = StartOmega * CurrentVelocity / StartVelocity;
        float angleChangeVelocity = angleCurvePath * Mathf.Deg2Rad * Time.deltaTime * CurrentOmega / StartOmega;

        float dx = _rigidbody.velocity.x * Mathf.Cos(angleChangeVelocity) + _rigidbody.velocity.z * Mathf.Sin(angleChangeVelocity);
        float dz = (-1) * _rigidbody.velocity.x * Mathf.Sin(angleChangeVelocity) + _rigidbody.velocity.z * Mathf.Cos(angleChangeVelocity);


        Vector3 VelocityDirection = new Vector3(_rigidbody.velocity.x + dx, 0f, _rigidbody.velocity.z + dz).normalized * CurrentVelocity;
        _rigidbody.velocity = VelocityDirection;
        EventController.onChangeVelocity(VelocityDirection);
    }

    private void ChangeVerticalAngle()
    {
        if (CurrentOmega > LimitOmega)
        {
            // Ограничиваем прецессию 10 градусами по X и Z
            Vector3 angles = _transform.rotation.eulerAngles;
            float angleX = angles.x;
            float signX = Mathf.Sign(angleX);
            angleX = Mathf.Abs(angleX);

            if (angleX > 350) { angleX = angleX - 350; signX = signX * (-1); }
            float angleZ = angles.z;
            float signZ = Mathf.Sign(angleZ);
            angleZ = Mathf.Abs(angleZ);
            if (angleZ > 350) { angleZ = angleZ - 350; signZ = signZ * (-1); }


            if (angleX > maxAngleY) angleX = maxAngleY;
            if (angleZ > maxAngleY) angleZ = maxAngleY;

            angleX = angleX - angleX * 0.1f;
            if (angleX < 0) angleX = 0;

            angleZ = angleZ - angleZ * 0.1f;
            if (angleZ < 0) angleZ = 0;


            angles = new Vector3(angleX * signX, angles.y, angleZ * signZ);

            _transform.rotation = Quaternion.Euler(angles);
        }
    }
    private void UpdateRotate()
    {
        // Вращаем
        Vector3 VectorOS = new Vector3(0f, 1f, 0f);
        _transform.Rotate(VectorOS, CurrentOmega * Time.deltaTime);
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "UpSpeed")
        {
            CurrentVelocity = CurrentVelocity * (1 + deltaBoosterVelocity);
            return;
            //Debug.Log("Increase Speed");
        }
        if (other.gameObject.tag == "DownSpeed")
        {
            CurrentVelocity = CurrentVelocity * (1 - deltaBoosterVelocity);
            return;
            // Debug.Log("Decrease Speed");
        }
        Vector3 MomemtumOnEndHit = _rigidbody.velocity * _rigidbody.mass;
        Vector3 DeltaMomemtumHit = MomemtumOnEndHit - MomemtumOnBeginHit;
        float DeltaTimeHit = Time.time - TimeBeginHit;
        if (DeltaTimeHit != 0)
        {
            Vector3 ForceVector = MomemtumOnBeginHit.normalized * DeltaMomemtumHit.magnitude / DeltaTimeHit;
            EventController.onCollision(ForceVector,other);

            Vector3 posTarget = other.gameObject.transform.position;

        }
        

    }

    protected void OnTriggerEnter(Collider other)
    {
        if (_rigidbody != null)
        {
            MomemtumOnBeginHit = _rigidbody.velocity * _rigidbody.mass;
            TimeBeginHit = Time.time;
        }
    }

    IEnumerator WaitDelayBeforeDrop(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay * Time.timeScale);
        EventController.StopedAction();

    }

    protected override void OnDispose()
    {

    }
}