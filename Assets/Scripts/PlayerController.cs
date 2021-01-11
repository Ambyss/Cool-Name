using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _input;
    private float _speed;
    public Joystick _joystick;
    private float _tempAngle;
    private float _currentAngle;
    private int _angle;
    private float _hp;
    private float _maxHp;
    public GameObject HPLine;
    private float _hpScale;
    private WavesController _wavesController;

    private void Start()
    {
        _maxHp = 1000;
        _hp = _maxHp;
        _rigidbody = GetComponent<Rigidbody>();
        _speed = 8f;
        _hpScale = HPLine.transform.localScale.x;
        _wavesController = GameObject.FindWithTag("GameController").GetComponent<WavesController>();
    }

    private void FixedUpdate()
    {
        // Moving
        _input = new Vector3(_joystick.Horizontal, 0, _joystick.Vertical);
        if (_input.magnitude > .8f)
            _rigidbody.velocity = _speed * _input;
        else
            _rigidbody.velocity = Vector3.zero;
            _input = _input.normalized;
        if (_input.magnitude != 0)
        {
            // Rotation
            _currentAngle = transform.rotation.eulerAngles.y;
            _tempAngle = Mathf.Acos(_input.z / _input.magnitude) * Mathf.Rad2Deg;
            if (_input.x < 0)
                _tempAngle = 360 - _tempAngle;
            _angle = (int) _tempAngle;
            transform.rotation = Quaternion.Euler(0, Mathf.LerpAngle(_currentAngle, _angle, .5f), 0);
        }
        if (_hp < 100)
        {
            _hp += .05f;
            HPLine.transform.localScale = new Vector3(_hp/_maxHp * _hpScale, HPLine.transform.localScale.y);
        }
    }

    public void Damage(int hp)
    {
        _hp -= hp;
        if (_hp <= 0)
        {
            print("Death!");
            // TODO: Death animation
            _wavesController.Death();
        }
        HPLine.transform.localScale = new Vector3(_hp/_maxHp * _hpScale, HPLine.transform.localScale.y);
    }
}
