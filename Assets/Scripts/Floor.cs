using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Floor : MonoBehaviour
{
    private NavMeshSurface _surface;

    void Start()
    {
        _surface = GetComponent<NavMeshSurface>();
        StartCoroutine(Surface());
    }

    IEnumerator Surface()
    {
        yield return new WaitForSeconds(5);
        _surface.AddData();
    }
}
