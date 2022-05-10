using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolView : MonoBehaviour
{
    NavMeshAgent agent;
    int i;

    public List<Transform> point;

    private PointView[] pointView;

    private RagDollAnim ragDoll;

    void Start()
    {
        ragDoll = GetComponent<RagDollAnim>();

        pointView =  FindObjectsOfType<PointView>();

        agent = GetComponent<NavMeshAgent>();
    }

    private void PointUpdate()
    {
        if (ragDoll is null)
        {
            i = Random.Range(0, pointView.Length);
        }
        else
        {
            i = Random.Range(0, point.Count);
        }
    }

    void Update()
    {
        if (agent.transform.position==agent.pathEndPosition)
        {
            PointUpdate();
        }
        if (ragDoll is null) agent.SetDestination(pointView[i].GetComponent<Transform>().position);
        else
            agent.SetDestination(point[i].position);
    }
}
