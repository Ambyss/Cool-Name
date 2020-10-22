using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private float _hp;
    private Rigidbody _rigidbody;
    private NavMeshAgent _agent;
    private Vector3 _dir;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _hp = 5f;
        _agent = GetComponent<NavMeshAgent>();
    }

    public void Damage(float damage, int puffPower)
    {
        _hp -= damage;
        if (_hp <= 0)
            Destroy(gameObject);
        _dir = new Vector3(Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad),0,  Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad));
        _agent.velocity = -_dir * puffPower;
    }
}
