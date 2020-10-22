using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesController : MonoBehaviour
{
    private GameObject[] _spawners;
    private int _currentWave; // TODO: Bind this and SpawnEnemies()

    public void InitSpawn()
    {
        _spawners = GameObject.FindGameObjectsWithTag("Spawner");
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        // TODO: Calculate num of enemies
        for (int i = 0; i < _spawners.Length; i++)
        {
            _spawners[i].GetComponent<EnemySpawner>().SpawnEnemies(33);
        }
    }
}
