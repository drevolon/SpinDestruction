using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenAnim : MonoBehaviour
{
    [SerializeField]
    private float _duration=1f;
    [SerializeField]
    private PathType _pathType = PathType.Linear;
    [SerializeField]
    private Transform[] _points;

    [SerializeField]
    private Vector3 _point=new Vector3(10f,0f, 0f);


    private List<Transform> _pointPosition = new List<Transform>();


    private void Start()
    {
        

        

          //  _pointPosition.Add(point.position);
       // transform.DOPath(_pointPosition.ToArray(), _duration, _pathType);

        //transform.DOMove(_point, _duration);
    }
    private void Update()
    {
        Animation();
    }

    private void Animation()
    {
        var sequence = DOTween.Sequence();
        foreach (var point in _points)
        {
            sequence.Append(transform.DOMove(point.transform.position, _duration));
        }

        sequence.Play();
    }

}
