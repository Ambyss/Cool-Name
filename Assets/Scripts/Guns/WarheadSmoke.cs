using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarheadSmoke : MonoBehaviour
{
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void StopEmmiting()
    {
        GetComponent<ParticleSystem>().Stop();
    }
}
