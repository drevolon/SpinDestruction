using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpdateManager:MonoBehaviour  
{
    private static UpdateManager Instance;

    private static event Action OnUpdateEvent;
    private static event Action OnFixedUpdateEvent;
    private static event Action OnLateUpdateEvent;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }

    }

    public static void SubscribeToUpdate(Action callback)
    {
        if (Instance == null) return;

        OnUpdateEvent += callback;
    }

    public static void SubscribeToFixedUpdate(Action callback)
    {
        if (Instance == null) return;

        OnFixedUpdateEvent += callback;
    }

    public static void SubscribeToLateUpdate(Action callback)
    {
        if (Instance == null) return;

        OnLateUpdateEvent += callback;
    }

    public static void UnsubscribeFromUpdate(Action callback)
    {
        OnUpdateEvent -= callback;
    }

    public static void UnsubscribeFromFixedUpdate(Action callback)
    {
        OnFixedUpdateEvent -= callback;
    }

    public static void UnsubscribeFromLateUpdate(Action callback)
    {
        OnLateUpdateEvent -= callback;
    }

    private void Update()
    {
        if (OnUpdateEvent != null) OnUpdateEvent.Invoke();
    }

    private void FixedUpdate()
    {
        if (OnFixedUpdateEvent != null) OnFixedUpdateEvent.Invoke();
    }

    private void LateUpdate()
    {
        if (OnLateUpdateEvent != null) OnLateUpdateEvent.Invoke();
    }


}