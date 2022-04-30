using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjects : InteractiveObject
{
    private Material _material;

    private void Awake()
    {
        
    }
    public override void Interaction()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log($"OnTriggerExitDestructObject {other.name}");
    }
}
