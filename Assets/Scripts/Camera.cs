using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;

    void Awake()
    {
        transform.position = new Vector3(target.position.x, 10, -.577f * transform.position.y + target.position.z);
    }

private void Update()
    {
        transform.position = new Vector3(target.position.x, 10, -.577f * transform.position.y + target.position.z);
    }
}
