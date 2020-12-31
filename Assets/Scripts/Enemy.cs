using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class Enemy : MonoBehaviour
{
    private float _hp;
    private Rigidbody _rigidbody;
    private NavMeshAgent _agent;
    private Vector3 _dir;
    private float _stopDistance;
    private PlayerController _player;
    private bool _isFight;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _hp = 5f;
        _agent = GetComponent<NavMeshAgent>();
        _stopDistance = 2;
        _agent.stoppingDistance = _stopDistance;
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _isFight = true;
    }

    public void Damage(float damage, float puffPower)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            GameObject.Find("WavesController").GetComponent<WavesController>().EnemyKilled();
            Destroy(gameObject);
        }
        _dir = new Vector3(Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad),0,  Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad));
        _agent.velocity = -_dir * puffPower;
    }

    public void ExplosionDamage(float damage, float puffPower, Vector3 point)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            GameObject.Find("WavesController").GetComponent<WavesController>().EnemyKilled();
            Destroy(gameObject);
        }
        _dir = new Vector3((point.x - transform.position.x),0,
            point.y - transform.position.y);
        _agent.velocity = -_dir * puffPower;
    }
        
    
    private void FixedUpdate()
    {
        if (_agent.remainingDistance < _stopDistance + .1f && _agent.remainingDistance != 0)
            FIght();
    }

    private void FIght()
    {
        if (_isFight)
        {
            _player.Damage(7);
            StartCoroutine(ReloadBuffer());
        }
    }

    IEnumerator ReloadBuffer()
    {
        _isFight = false;
        yield return new WaitForSeconds(.6f);
        _isFight = true;
    }
}
