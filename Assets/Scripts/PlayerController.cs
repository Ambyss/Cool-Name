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

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _speed = 8f;
    }

    private void FixedUpdate()
    {
        // Moving
        _input = new Vector3(_joystick.Horizontal, 0, _joystick.Vertical).normalized;
        _rigidbody.velocity = _speed * _input; 
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
    }
}
