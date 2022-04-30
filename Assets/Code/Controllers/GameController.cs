using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;

public class GameController : BaseController
{
    [SerializeField]
    Transform spawnPoint;
    [SerializeField]
    GameObject gameObjectCar;
    [SerializeField]
    GameObject gameObjectDamage;

    //RagDollAnim dollAnim;

    private InteractiveObject[] _interactiveObjects;
    private List<RagDollAnim> _dollAnim;

    private List<Car> _car;

    Vector3 beginStart;

    private Player _player;

    Rigidbody rigidbodyInteractive;
    Vector3 vectorForceInteractive;

    private Rigidbody _rigidbody;
    private RagDollAnim _ragDollAnim;


    private float _velocityRigidBody = 500f;
    private float _forceRigidBody=500f;

    float movePopupWindows = 20f;

    GameObject gameObjectPopup;

    [SerializeField]
    private TextMeshProUGUI textMeshScore;
    [SerializeField]
    private TextMeshProUGUI textMeshHighScore;
    [SerializeField]
    private TextMeshProUGUI textMeshLastScore;

    public Slider sliderVelocity;

    int score = 0, lastScore=0, highScore=0;

    int velocityTop = 0;


    public GameController(ProfilePlayer profilePlayer)
    {
        //var carController = new Car();
        //AddController(carController);

    }

    private void Awake()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            textMeshHighScore.text = $"HiSc: { PlayerPrefs.GetString("HighScore")}";
        }
        else textMeshHighScore.text = $"HiSc: 0";

        if (PlayerPrefs.HasKey("LastScore"))
        {
            textMeshLastScore.text = $"Sc: { PlayerPrefs.GetString("LastScore")}";
        }
        else textMeshLastScore.text = $"Sc: 0";

        _car = new List<Car>();

        sliderVelocity.value = 1;

        EventController.OnStoped += OnStopedTop; //Подписались на событие падения волчка;
        EventController.OnCollision += OnCollisionTop; //Подписались на столкновение
        EventController.OnCollisionWall += OnCollisionWall; //Подписались на столкновение со стеной

        _dollAnim = new List<RagDollAnim>();

        _interactiveObjects = FindObjectsOfType<InteractiveObject>();
    }

    private void OnCollisionWall(Collider collider)
    {

    }

    private void OnCollisionTop(Vector3 arg1, Collider collisionColliderObject)
    {
        //Debug.Log($"OnCollisionTop-{collisionColliderObject} {collisionColliderObject.GetComponentInParent<RagDollAnim>()}");
        //Debug.Log($"OnCollisionTop-{collisionColliderObject.gameObject.tag}");

        if (collisionColliderObject.TryGetComponent<Rigidbody>(out _rigidbody)) _rigidbody = collisionColliderObject.GetComponent<Rigidbody>();


        if (collisionColliderObject.gameObject.tag == "RagDoll")
        {
            //Debug.Log($"RagDoll-{collisionColliderObject} {collisionColliderObject.GetComponentInParent<RagDollAnim>()}");

            DamagePopup.Create(collisionColliderObject.transform.position, 1000, gameObjectDamage);

            _ragDollAnim = collisionColliderObject.GetComponent<RagDollAnim>();
            _ragDollAnim.Death();

            score += 1000;

            Destroy(collisionColliderObject.gameObject, 5f);
        }
        if  (_rigidbody!=null && collisionColliderObject.gameObject.tag == "Transport")
        {
            //Debug.Log($"Ломаем - {collisionColliderObject.gameObject.tag}");

            DamagePopup.Create(collisionColliderObject.transform.position, 500, gameObjectDamage);

            collisionColliderObject.GetComponent<PatrolView>().enabled = false;
            collisionColliderObject.GetComponent<NavMeshAgent>().enabled = false;

            _rigidbody.AddForce(transform.up* 700f);

            score += 500;
        }

        if (_rigidbody != null && collisionColliderObject.gameObject.tag == "InteractiveObject")
        {
            DamagePopup.Create(collisionColliderObject.transform.position, 100, gameObjectDamage);

            score += 100;
        }

        var collisionColliderObjects = collisionColliderObject.GetComponentsInChildren<DestructibleObjects>();


        if (collisionColliderObjects.Length > 0)
        {

            foreach (var item in collisionColliderObjects)
            {
                //rigidbodyInteractive = item.GetComponent<Rigidbody>();
                //rigidbodyInteractive.isKinematic = false;
               // rigidbodyInteractive.useGravity = true;

                //vectorForceInteractive = new Vector3(arg1.x, arg1.y * Random.Range(1f, 3f), arg1.z);

                //vectorForceInteractive = new Vector3(transform.position.x * Random.Range(30f, 50f), transform.position.y * Random.Range(300f, 500f), transform.position.z * Random.Range(30f, 50f));

                //rigidbodyInteractive.AddForce(vectorForceInteractive);
            }
        }
           
    }

    private void OnStopedTop() // Волчок упал
    {
        // Debug.Log("Player Down. Need game over");

        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = Convert.ToInt16(PlayerPrefs.GetString("HighScore"));
            lastScore = Convert.ToInt16(textMeshScore.text);
            //Debug.Log($"{lastScore} > {highScore} ");
            if (lastScore> highScore) PlayerPrefs.SetString("HighScore", textMeshScore.text);
        }
        else
        {
            PlayerPrefs.SetString("HighScore", textMeshScore.text);
        }

        PlayerPrefs.SetString("LastScore", textMeshScore.text);
        PlayerPrefs.Save();

        EventController.OnCollision -= OnCollisionTop;
        EventController.OnStoped -= OnStopedTop;
        EventController.OnCollisionWall -= OnCollisionWall;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        // Score.te
        textMeshScore.text = score.ToString();
        sliderVelocity.value = velocityTop;
        for (int i = 0; i < _interactiveObjects.Length; i++)
        {
            var interactiveObject = _interactiveObjects[i];

            if (interactiveObject == null) continue;


            if (interactiveObject is DestructibleObjects dObject)
            {
                rigidbodyInteractive = dObject.GetComponent<Rigidbody>();

                if (rigidbodyInteractive.velocity.y < -5f)
                {
                    Destroy(dObject.gameObject, 3f);
                }   
               
            }


            if (interactiveObject is BoosterObject bObject)
            {
                bObject.Flay();
            }

        }


        if (_car!=null && _car.Count>0 )
        {
            try
            {
                foreach (var itemCar in _car)
                {
                    if (itemCar.transform.position.x < -80f)
                    {
                        if (_car != null)
                            itemCar?.DestroyCar();
                        _car.Remove(itemCar);

                        GenerationObj();
                    }
                    else
                    {
                        //Debug.Log("_car.Move()");
                        itemCar?.Move();
                    }
                }
                _car.Clear();
            }
            catch { };


            }
        else
        {
        }

    }



    void CarInit()
    {
        
        _car = new List<Car>();

        foreach (var item in FindObjectsOfType<Car>())
        {
            _car.Add(item);
            item.Speed = 10f;
        }
    }

    private void DestroyObject(GameObject dObject)
    {
        if (dObject!=null)
        Destroy(dObject.gameObject, 3f);
    }


    private void GenerationObj()
    {
        Instantiate(gameObjectCar, spawnPoint.position, Quaternion.AngleAxis(-90, Vector3.up));
       // return Instantiate(gameObjectCar, spawnPoint.position, Quaternion.identity);
    }

    private GameObject SpawnPopupDamage(Vector3 posSpawnDamage)
    {
        GameObject objDamageSpawn= Instantiate(gameObjectDamage, posSpawnDamage, Quaternion.identity);
        return objDamageSpawn;
        
    }

    IEnumerator PopupDamageCoroutine(GameObject gameObject)
    {
        while (gameObject!=null)
        {
            //SpawnPopupDamage(gameObject.transform.position);
            //Debug.Log("IEnumerator begin");
            gameObject.transform.position=new Vector3(0, gameObject.transform.position.y*10f * Time.deltaTime,0);
            yield return new WaitForSeconds(10);
            Destroy(gameObject);
            Debug.Log("IEnumerator end");
        }

    }
}
