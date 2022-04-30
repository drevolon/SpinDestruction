using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterObject : InteractiveObject
{
    private float _lengthFlay;

    public override void Interaction()
    {
        
    }

    private void Awake()
    {
        _lengthFlay = Random.Range(1f, 3.0f);
    }

    public void Flay()
    {
        float curXPosition = Mathf.PingPong(Time.time, _lengthFlay)+3f;

        transform.position = new Vector3(transform.position.x,
            curXPosition,
            transform.position.z);
    }
}


