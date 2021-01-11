using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemy;
    private int _randomNumber;

    public void SpawnEnemies(int numberOfEnemies)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (Random.Range(9, 20) == 10)
                Instantiate(_enemy[1], transform.position, Quaternion.identity);
            else
                Instantiate(_enemy[0], transform.position, Quaternion.identity);
        }
    }
}
