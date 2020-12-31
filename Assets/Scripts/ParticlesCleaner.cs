using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesCleaner : MonoBehaviour
{
    private GameObject[] _objects;
    
    void Start()
    {
        StartCoroutine(Cleaner());
    }

    IEnumerator Cleaner()
    {
        yield return new WaitForSeconds(5);
        _objects = GameObject.FindGameObjectsWithTag("Clean");
        yield return new WaitForSeconds(5);
        for (int i = 0; i < _objects.Length; i++)
        {
            Destroy(_objects[i]);
        }
        
        StartCoroutine(Cleaner());
    }
}
