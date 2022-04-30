using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WallView : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        EventController.onCollisionWall(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    
}
