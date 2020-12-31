using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{
    public GameObject tempMazeSpawner;
    private int _koef;
    private int _buffer;
    private int _maxBoxes;
    private int _spawmInterval;
    public GameObject ammoBox;
    
    
    void Start()
    {
        _koef = tempMazeSpawner.GetComponent<MazeSawner>().koef;
        _buffer = 0;
        _maxBoxes = 5;
        _spawmInterval = 10;
        StartCoroutine(SpawnAmmo());
    }

    IEnumerator SpawnAmmo()
    {
        if (_buffer < _maxBoxes)
        {
            _buffer++;
            // All ranges depends from Width and Height fo Labyrinth!!
            Vector3 pos = new Vector3(Random.Range(0, 9) * _koef + 4, .5f, Random.Range(0, 9) * _koef + 4); 
            Instantiate(ammoBox, pos, Quaternion.identity);
        }
        yield return new WaitForSeconds(_spawmInterval);
        StartCoroutine(SpawnAmmo());
    }

    public void AmmoPicked()
    {
        _buffer--;
    }
}
