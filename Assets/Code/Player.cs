using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IMove, IHeal
{
    public int HP { get; set; } = 10;
    public float Speed { get; set; } = 6f;

    public float StartOmega = 5f; // Начальная скорость вращения
    public GameObject LineTarget; // Объект с LineRendere, который рисует траекторию движения
    public float StartVelocity = 50f; // Начальная скорость движения
    public float deltaVelocity = 0.05f; // скорость убывания движения
    public float angleCurvePath = 5f;
    public float deltaBoosterVelocity = 0.5f; // Изменение скорости (в долях единицы) от текущей, при столкновении с объектами-бустерами
    public float SlowMotionRate = 0.2f;

    protected Rigidbody _rigidbody;
    protected Transform _transform;
    private Vector2 fingerDownPos = Vector2.zero; // Точка экрана, где была нажата кнпка мыши или touch
    private Vector2 fingerUpPos = Vector2.zero; // Точка экрана, где была отжата кнпка мыши или touch
    protected LineRenderer _line; // LineRenderer для траектории движения

    protected float CurrentOmega = 5f;// текущая скорость вращения
    protected float StartForce = 100f; // Начальная сила 

    //public GameObject path; // Объект, рисующий траекторию (устарело)
    public float CurrentVelocity; // Текущая линейная скорость волчка

    protected float LimitOmega = 0.1f; // Обороты, при меньшем значении снимается ограничение на отклонение от оси Y
    protected float maxAngleY = 5f; // Макс отклонение от вертикальной оси при вращении до достижении скорости вращения LimitOmega
    public bool isStart = false; // Флаг о произведенном запуске
    protected Vector3[] TraceTarget = new Vector3[20]; // Массив, хранящий первоначальную траектрию


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _line = GetComponent<LineRenderer>();
        CurrentOmega = StartOmega;
        CurrentVelocity = StartVelocity;


        //// *********** Прицел с помощью GameObject sphera *****************************
        //for (int nPoint = 1; nPoint < 20; nPoint++)
        //{
        //    Transform childPoint = path.transform.GetChild(nPoint);
        //    float angleChangeVelocity = StartOmega * Time.deltaTime * nPoint *Mathf.Deg2Rad;
        //    Vector3 pos = childPoint.localPosition;
        //    float newPosx = pos.x * Mathf.Cos(-angleChangeVelocity) - pos.z * Mathf.Sin(-angleChangeVelocity);
        //    float newPosz = pos.x * Mathf.Sin(-angleChangeVelocity) + pos.z * Mathf.Cos(-angleChangeVelocity);
        //    childPoint.localPosition = new Vector3(newPosx, pos.y, newPosz);
        //}


        // LineRenderer у отдельного объекта
        _line = LineTarget.GetComponent<LineRenderer>();
        float deltaTimeBetweenTargetPoint = 1f / StartVelocity;
        for (int nPoint = 0; nPoint < 20; nPoint++)
        {
            Vector3 pos = new Vector3(nPoint + 1f, 0f, 0f);
            float angleChangeVelocity = angleCurvePath * deltaTimeBetweenTargetPoint * nPoint * Mathf.Deg2Rad;
            float newPosx = pos.x * Mathf.Cos(-angleChangeVelocity) - pos.z * Mathf.Sin(-angleChangeVelocity);
            float newPosz = pos.x * Mathf.Sin(-angleChangeVelocity) + pos.z * Mathf.Cos(-angleChangeVelocity);
            TraceTarget[nPoint] = new Vector3(newPosx, pos.y, newPosz);
        }

        UIDetector.instance.OnBloodyTopEndTap += OnStartBloodyTop;
        UIDetector.instance.OnBloodyTopBeginTap += StartSlowMotion;
        UIDetector.instance.OnBloodyTopTargeting += OnUpdateTraceLine;
       //UIDetector.instance.OnBloodyTopTap += OnSingleTap;

        //UIDetector.instance.OnBloodyTopLeft += OnChangeVelocityLeft;
        //UIDetector.instance.OnBloodyTopRight += OnChangeVelocityRight;
        //UIDetector.instance.OnBloodyTopUp += OnChangeVelocityUp;
        //UIDetector.instance.OnBloodyTopDown += OnChangeVelocityDown;
        // UIDetector.instance.OnEndCorrectDirection += OnEndCorrectDirection;

    }
    public void Move()
    {


        if (isStart)
        {

            // Уменьшаем скорость на deltaVelocity
            CurrentVelocity -= deltaVelocity * Time.deltaTime;
            if (CurrentVelocity < 0)
            {
                //_rigidbody.velocity = Vector3.zero;
                return;
            }

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

            ///
            //if (Input.GetMouseButton(0) || (Input.touchCount > 0))
            //{
            //    Vector3 InputPosition;
            //    if (Input.GetMouseButton(0))
            //    {
            //        InputPosition = Input.mousePosition;
            //    }
            //    if (Input.touchCount > 0)
            //    {
            //        InputPosition = Input.GetTouch(0).position;
            //    }

            //    float ModulV = _rigidbody.velocity.magnitude;

            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    RaycastHit Po;
            //    if (Physics.Raycast(ray, out Po))
            //    {
            //        Vector3 mousePos = new Vector3(Po.point.x, Po.point.y, Po.point.z);
            //        Vector3 vectorMove = mousePos - _transform.position;
            //        vectorMove.y = 0;
            //        vectorMove = vectorMove.normalized * ModulV;

            //        _rigidbody.velocity = vectorMove;

            //        //                    _rigidbody.AddForce(vectorMove, ForceMode.Impulse);

            //    }

            //}
            //   Debug.Log(CurrentOmega.ToString()+" "+ CurrentVelocity.ToString());
        }


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


        // Вращаем
        Vector3 VectorOS = new Vector3(0f, 1f, 0f);
        //        VectorOS = _transform.rotation * VectorOS;
        //_transform.RotateAround(VectorOS, CurrentOmega * Time.deltaTime);

        _transform.Rotate(VectorOS, CurrentOmega * Time.deltaTime);

        _line.gameObject.transform.position = new Vector3(_transform.position.x, _line.gameObject.transform.position.y, _transform.position.z);

    }

    protected void OnStartBloodyTop(Vector3 fingerDownPos, Vector3 fingerUpPos)
    {
        if (!isStart)
        {
            if ((fingerDownPos != Vector3.zero) && (fingerUpPos != Vector3.zero))
            {
                float dx = fingerUpPos.x - fingerDownPos.x;
                float dy = fingerUpPos.y - fingerDownPos.y;
                Vector3 vectorMove = new Vector3(_transform.position.x - dy, 0, _transform.position.z + dx);
                vectorMove = vectorMove.normalized * CurrentVelocity;
                _rigidbody.velocity = vectorMove;
            }
        
            isStart = true;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;

            _line.enabled = false;
            //                path.SetActive(false);
        }
    

    }

    protected void OnUpdateTraceLine(Vector3 fingerDownPos, Vector3 fingerUpPos)
    {
        if (!isStart)
        {
            //fingerUpPos = Input.mousePosition;
            //_line.positionCount = 2;
            //Vector3[] PosArrow = new Vector3[2];
            //PosArrow[0] = _transform.position;
            //float dx = fingerUpPos.x - fingerDownPos.x;
            //float dy = fingerUpPos.y - fingerDownPos.y;
            //float dl = Mathf.Sqrt(dx * dx + dy * dy) / 25;

            //_line.enabled = true;
            //PosArrow[1] = new Vector3(_transform.position.x - dy/10, _transform.position.y, _transform.position.z + dx/10);
            //_line.SetPositions(PosArrow);


            //// Прицел с использованием GameObjects Sphere
            //fingerUpPos = Input.mousePosition;
            //float dx = fingerUpPos.x - fingerDownPos.x;
            //float dy = fingerUpPos.y - fingerDownPos.y;
            //float anglePath = Mathf.Atan2(dx, dy) * Mathf.Rad2Deg - 180f;

            //float dl = Mathf.Sqrt(dx * dx + dy * dy)/10;
            //int countPoints = Mathf.RoundToInt(dl);

            //for (int nPoint =0;nPoint<20;nPoint++)
            //{
            //    path.transform.GetChild(nPoint).gameObject.SetActive(nPoint <= countPoints);
            //}

            //path.transform.position = _transform.position;
            //Vector3 to = new Vector3(0, anglePath, 0);

            //path.transform.eulerAngles = to;
            //path.SetActive(true);

            // Прицел с использованием LineRenderer
            float dx = fingerUpPos.x - fingerDownPos.x;
            float dy = fingerUpPos.y - fingerDownPos.y;
            float anglePath = Mathf.Atan2(dx, dy) * Mathf.Rad2Deg - 180f;

            float dl = Mathf.Sqrt(dx * dx + dy * dy) / 10;
            int countPoints = Mathf.RoundToInt(dl);
            if (countPoints > TraceTarget.Length) countPoints = TraceTarget.Length;
            LineTarget.transform.position = new Vector3(_transform.position.x, LineTarget.transform.position.y, _transform.position.z);

            _line.positionCount = countPoints;
            Vector3[] posArray = new Vector3[countPoints];
            Array.Copy(TraceTarget, posArray, countPoints);
            _line.SetPositions(posArray);
            _line.enabled = true;

            Vector3 to = new Vector3(0, anglePath, 0);
            LineTarget.transform.eulerAngles = to;
        }
    }

    private void OnChangeVelocityRight()
    {
        Vector3 Vpos = Quaternion.Euler(new Vector3(0f, Camera.main.transform.rotation.eulerAngles.y, 0f)) * (new Vector3(10f, 0f, 0f));
        ChangeVectorVelocity(Vpos);
        _rigidbody.velocity = Quaternion.Euler(new Vector3(0f, 10f * Mathf.Sign(_rigidbody.velocity.x), 0f)) * _rigidbody.velocity;
    }
    private void OnChangeVelocityLeft()
    {
        Vector3 Vpos = Quaternion.Euler(new Vector3(0f, Camera.main.transform.rotation.eulerAngles.y, 0f)) * (new Vector3(-10f, 0f, 0f));
        ChangeVectorVelocity(Vpos);
        _rigidbody.velocity = Quaternion.Euler(new Vector3(0f, -10f * Mathf.Sign(_rigidbody.velocity.x), 0f)) * _rigidbody.velocity;
    }
    private void OnChangeVelocityUp()
    {
        Vector3 Vpos = Quaternion.Euler(new Vector3(0f, Camera.main.transform.rotation.eulerAngles.y - 90, 0f)) * (new Vector3(-10f, 0f, 0f));
        ChangeVectorVelocity(Vpos);
        _rigidbody.velocity = Quaternion.Euler(new Vector3(0f, 10f * Mathf.Sign(_rigidbody.velocity.z), 0f)) * _rigidbody.velocity;
    }
    private void OnChangeVelocityDown()
    {
        Vector3 Vpos = Quaternion.Euler(new Vector3(0f, Camera.main.transform.rotation.eulerAngles.y + 90, 0f)) * (new Vector3(-10f, 0f, 0f));
        ChangeVectorVelocity(Vpos);
        _rigidbody.velocity = Quaternion.Euler(new Vector3(0f, 10f * Mathf.Sign(_rigidbody.velocity.z), 0f)) * _rigidbody.velocity;
    }
    private void ChangeVectorVelocity(Vector3 ArrowDirection)
    {
        Vector3[] LinePnts = new Vector3[2];
        LinePnts[0] = Vector3.zero;
        LinePnts[1] = ArrowDirection;
        _line.gameObject.transform.eulerAngles = Vector3.zero;
        _line.positionCount = 2;
        _line.SetPositions(LinePnts);
        _line.enabled = true;

    }
    private void OnEndCorrectDirection()
    {
        _line.enabled = false;
    }
    private void StartSlowMotion()
    {
        if (isStart)
        {
            isStart = false;
            Time.timeScale = SlowMotionRate;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
            StartCoroutine("WaitSlowMotion");
        }
    }

    private void OnSingleTap()
    {
        StartSlowMotion();
    }

    IEnumerator WaitSlowMotion()
    {
        yield return new WaitForSeconds(3f* Time.timeScale);
        OnStartBloodyTop(Vector3.zero, Vector3.zero);
    }


    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "UpSpeed")
        {
            CurrentVelocity = CurrentVelocity * (1 + deltaBoosterVelocity);
            //Debug.Log("Increase Speed");
        }
        if (other.gameObject.tag == "DownSpeed")
        {
            CurrentVelocity = CurrentVelocity * (1 - deltaBoosterVelocity);
            // Debug.Log("Decrease Speed");
        }
    }

}


