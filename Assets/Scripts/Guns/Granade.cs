using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public GameObject _explosion;


    void Start()
    {
        StartCoroutine(ExplTime());
    }
    
    IEnumerator ExplTime()
    {
        yield return new WaitForSeconds(2f);
        Instantiate(_explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
