using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventController  
{
    private static PointerPoint _point = new PointerPoint();
    // event старт волчка
    public static Action<PointerPoint> OnBloodyTopEndTap;

    // event старт волчка
    public static Action OnBloodyTopBeginTap;

    // event Выбор направления старта
    public static Action<PointerPoint> OnBloodyTopTargeting;

    // event Волчок вправо
    public static Action OnBloodyTopRight; // Создать на его основе событие

    // event Волчок влево
    public static Action OnBloodyTopLeft; // Создать на его основе событие

    // event Волчок вверх
    public static Action OnBloodyTopUp; // Создать на его основе событие

    // event Волчок вниз
    public static Action OnBloodyTopDown; // Создать на его основе событие

    // event закончили корректировать траекторию
    public static Action OnEndCorrectDirection; // Создать на его основе событие

    // event single tap
    public static Action OnBloodyTopTap; // Создать на его основе событие

    public static Action OnStart; // Запуск вочка

    public static Action OnStoped; // Запуск вочка

    public static Action<Vector3,Collider> OnCollision; // При столкновении

    public static Action<Collider> OnCollisionWall; //При столкновении со стеной

    public static Action<Vector3> OnChangeVelocity; // Изменение скорости

    
    public static void StartAction()
    {
        OnStart?.Invoke();
    }

    public static void StopedAction()
    {
        OnStoped?.Invoke();
    }

    public static void SwipePointer(Vector3 pos)
    {
        _point.fingerUpPos = pos;
        OnBloodyTopTargeting?.Invoke(_point);

        switch (DetectSwipe())
        {
            case SwipeDirection.Left:
                OnBloodyTopLeft?.Invoke();
                break;
            case SwipeDirection.Right:
                OnBloodyTopRight?.Invoke();
                break;
            case SwipeDirection.Up:
                OnBloodyTopUp?.Invoke();
                break;
            case SwipeDirection.Down:
                OnBloodyTopDown?.Invoke();
                break;
        }
    }
    public static void DownPointer(Vector3 pos)
    {
        if (_point.fingerDownPos == Vector2.zero)
        {
            _point.fingerDownPos = pos;
            _point.fingerUpPos = Vector2.zero;
            OnBloodyTopBeginTap?.Invoke();
        }
    }

    public static void UpPointer(Vector3 pos)
    {
        _point.fingerUpPos = pos;
        if (DetectSwipe() != SwipeDirection.Unknown)
        {
            OnBloodyTopEndTap?.Invoke(_point);
            OnEndCorrectDirection?.Invoke();
        }
        else
        {
            OnBloodyTopTap?.Invoke();
        }

        _point.fingerDownPos = Vector2.zero;
    }

    private static SwipeDirection DetectSwipe()
    {

        if (VerticalMoveValue() > _point.SWIPE_THRESHOLD && VerticalMoveValue() > HorizontalMoveValue())
        {
            if (_point.fingerDownPos.y - _point.fingerUpPos.y > 0)
            {
                return SwipeDirection.Up;
            }
            else if (_point.fingerDownPos.y - _point.fingerUpPos.y < 0)
            {
                return SwipeDirection.Down;
            }

        }
        else if (HorizontalMoveValue() > _point.SWIPE_THRESHOLD && HorizontalMoveValue() > VerticalMoveValue())
        {
            if (_point.fingerDownPos.x - _point.fingerUpPos.x > 0)
            {
                return SwipeDirection.Left;
            }
            else if (_point.fingerDownPos.x - _point.fingerUpPos.x < 0)
            {
                return SwipeDirection.Right;
            }

        }

        return SwipeDirection.Unknown;
    }

    private static float VerticalMoveValue()
    {
        return Mathf.Abs(_point.fingerDownPos.y - _point.fingerUpPos.y);
    }

    private static float HorizontalMoveValue()
    { 
        return Mathf.Abs(_point.fingerDownPos.x - _point.fingerUpPos.x);
    }

    public static void onCollision(Vector3 ForceVector, Collider other)
    {
        OnCollision?.Invoke(ForceVector,other);
    }

    public static void onCollisionWall(Collider other)
    {
        OnCollisionWall?.Invoke(other);
    }
    public static void onChangeVelocity(Vector3 Velocity)
    {
        OnChangeVelocity?.Invoke(Velocity);
    }
}
