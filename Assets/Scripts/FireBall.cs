using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private GameObject _player;
    private Transform _target;
    private Rigidbody _rigidbody;
    private Vector3 _dir;
    private int _power;

    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _power = 50;
        _target = _player.GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody>();
        _dir = new Vector3(_target.position.x - transform.position.x, 0, _target.position.z - transform.position.z);
        _rigidbody.AddForce(_dir * _power);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            _player.GetComponent<PlayerController>().Damage(35);
            Destroy(gameObject);
        }
        else if (collider.CompareTag("Wall"))
            Destroy(gameObject);
    }
}
