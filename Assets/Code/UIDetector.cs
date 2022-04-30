using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDetector : MonoBehaviour
{
	private Vector2 fingerDownPos;
	private Vector2 fingerUpPos;
    public static UIDetector instance = null;

	public float SWIPE_THRESHOLD = 20f;

	private bool isStart = false;
    public enum typeSwipeDirection
    {
        Left,
        Right,
        Up,
        Down,
        Unknown
    };

    public static typeSwipeDirection SwipeDirection;

    // event старт волчка
    public delegate void BloodyTopEndTap(Vector3 fingerUpPos, Vector3 fingerDownPos); // Добавить новый делегат
    public event BloodyTopEndTap OnBloodyTopEndTap; // Создать на его основе событие

    // event старт волчка
    public delegate void BloodyTopBeginTap(); // Добавить новый делегат
    public event BloodyTopBeginTap OnBloodyTopBeginTap; // Создать на его основе событие


    // event Выбор направления старта
    public delegate void BloodyTopTargeting(Vector3 fingerUpPos, Vector3 fingerDownPos); // Добавить новый делегат
    public event BloodyTopTargeting OnBloodyTopTargeting; // Создать на его основе событие

    // event Волчок вправо
    public delegate void BloodyTopRight(); // Добавить новый делегат
    public event BloodyTopRight OnBloodyTopRight; // Создать на его основе событие

    // event Волчок влево
    public delegate void BloodyTopLeft(); // Добавить новый делегат
    public event BloodyTopLeft OnBloodyTopLeft; // Создать на его основе событие

    // event Волчок вверх
    public delegate void BloodyTopUp(); // Добавить новый делегат
    public event BloodyTopUp OnBloodyTopUp; // Создать на его основе событие

    // event Волчок вниз
    public delegate void BloodyTopDown(); // Добавить новый делегат
    public event BloodyTopDown OnBloodyTopDown; // Создать на его основе событие

    // event закончили корректировать траекторию
    public delegate void EndCorrectDirection(); // Добавить новый делегат
    public event EndCorrectDirection OnEndCorrectDirection; // Создать на его основе событие

    // event single tap
    public delegate void BloodyTopTap(); // Добавить новый делегат
    public event BloodyTopTap OnBloodyTopTap; // Создать на его основе событие


    void Awake()
    {
        // Проверяем, задан ли инстанс нашего менеджера
        if (instance == null)
        { // Инстанс не задан
            instance = this; // Установить в инстанс текущий объект
        }
    }
    // Update is called once per frame
    void Update ()
	{

		CheckMouseDrag(isStart);
		CheckTouch(isStart);

	}

    private void SwipePointer(Vector3 pos)
    {
        fingerUpPos = pos;
        if (!isStart)
            OnBloodyTopTargeting?.Invoke(fingerDownPos, fingerUpPos);
        else
            switch (DetectSwipe())
            {
                case typeSwipeDirection.Left:
                    OnBloodyTopLeft?.Invoke();
                    break;
                case typeSwipeDirection.Right:
                    OnBloodyTopRight?.Invoke();
                    break;
                case typeSwipeDirection.Up:
                    OnBloodyTopUp?.Invoke();
                    break;
                case typeSwipeDirection.Down:
                    OnBloodyTopDown?.Invoke();
                    break;
            }
    }
    protected void instanceUpPointer(Vector3 pos)
    {
        fingerUpPos = pos;
        if (DetectSwipe() != typeSwipeDirection.Unknown)
        {
            OnBloodyTopEndTap?.Invoke(fingerDownPos, fingerUpPos);
            OnEndCorrectDirection?.Invoke();
        }
        else
        {
            OnBloodyTopTap?.Invoke();
        }
        
        fingerDownPos = Vector2.zero;
    }
    private void CheckMouseDrag(bool isStart)
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (fingerDownPos == Vector2.zero)
            {
                fingerDownPos = Input.mousePosition;
                fingerUpPos = Vector2.zero;
                OnBloodyTopBeginTap?.Invoke();
            }
        }

        if ((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0))
        {

            if (fingerDownPos != Vector2.zero)
            {
                SwipePointer(Input.mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (fingerDownPos != Vector2.zero)
            {
                instanceUpPointer(Input.mousePosition);
            }
        }
    }


    protected void CheckTouch(bool isStart)
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerDownPos = touch.position;
                fingerUpPos = Vector3.zero;
                OnBloodyTopBeginTap?.Invoke();
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if (fingerDownPos != Vector2.zero)
                {
                    SwipePointer(touch.position);
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if (fingerDownPos != Vector2.zero)
                {
                    instanceUpPointer(touch.position);
                }

            }

        }
    }

    typeSwipeDirection DetectSwipe()
    {

        if (VerticalMoveValue() > SWIPE_THRESHOLD && VerticalMoveValue() > HorizontalMoveValue()) {
            if (fingerDownPos.y - fingerUpPos.y > 0) {
                return typeSwipeDirection.Up;
            } else if (fingerDownPos.y - fingerUpPos.y < 0) {
                return typeSwipeDirection.Down;
            }

        } else if (HorizontalMoveValue() > SWIPE_THRESHOLD && HorizontalMoveValue() > VerticalMoveValue()) {
            if (fingerDownPos.x - fingerUpPos.x > 0) {
                return typeSwipeDirection.Left;
            } else if (fingerDownPos.x - fingerUpPos.x < 0) {
                return typeSwipeDirection.Right;
            }

        }

            return typeSwipeDirection.Unknown;
    }

    float VerticalMoveValue ()
	{
		return Mathf.Abs (fingerDownPos.y - fingerUpPos.y);
	}

	float HorizontalMoveValue ()
	{
		return Mathf.Abs (fingerDownPos.x - fingerUpPos.x);
	}

}
