using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionObject : MonoBehaviour
{
    public Action<RagDollAnim> OnRagDollCollionExit { get; set; }

    private void Start()
    {
        
    }
   
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("OnTriggerStay");

        //var ragDoll = other.gameObject.GetComponent<RagDollAnim>();

        //OnRagDollCollionExit.Invoke(ragDoll);
    }
}
