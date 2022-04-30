using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerView Player;
    private Vector3 _offset = new Vector3(-23f, 34f, 25f);

    

    private void Update()
    {
        Player = FindObjectOfType<PlayerView>();
      
    }

    private void LateUpdate()
    {
        if (Player!=null)
        transform.position = Player.transform.position + _offset;
    }

}
