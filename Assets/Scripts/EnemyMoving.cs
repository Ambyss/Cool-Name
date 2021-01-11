using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoving : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Transform _target;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.FindWithTag("Player").GetComponent<Transform>();
        StartCoroutine(ChekcDestination());
    }

    IEnumerator ChekcDestination()
    {
        try
        {
            _agent.destination = _target.position;
        }
        catch (Exception e)
        {
            Destroy(this);
        }
        yield return new WaitForSeconds(.33f);
        StartCoroutine(ChekcDestination());
    }
}
