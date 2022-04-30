using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class InteractiveObject: MonoBehaviour, IInteractable
{
    public bool IsInteractable { get; } = true;
    public abstract void Interaction();

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(nameof(other));
        if (!IsInteractable || !other.CompareTag("Player"))
        {
            return;
        }

        //Interaction();
        Destroy(gameObject, 2f);
    }

    private void Start()
    {
        ((IAction)this).Action();
    }

    void IAction.Action()
    {
        if (TryGetComponent(out Renderer renderer))
        {
            //renderer.material.color = Color.black;
        }
    }

   
}
