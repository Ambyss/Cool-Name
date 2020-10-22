using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSawner : MonoBehaviour
{
    public GameObject cellPrefab;
    [SerializeField] private GameObject _spawner;
    public int koef = 8;
    
    private void Start()
    {
        Spawn();
        GameObject.Find("WavesController").GetComponent<WavesController>().InitSpawn();
    }

    private void Spawn()
    {
        MazeGenerator generator = new MazeGenerator();
        MazeGeneratorCell[,] maze = generator.GenerateMaze();
        
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                Cell c = Instantiate(cellPrefab, new Vector3(i * koef, 0, j * koef), Quaternion.identity).GetComponent<Cell>();
                c.transform.parent = gameObject.transform;
                c.WallLeft.SetActive(maze[i, j].WallLeft);
                c.WallBottom.SetActive(maze[i, j].WallBottom);
            }
        }

        for (int i = 0; i < maze.GetLength(0) - 1; i++)
        {
            if (!maze[i, 0].WallBottom)
                Instantiate(_spawner, new Vector3(maze[i, 0].GetPosition().x * koef + koef/2, 2,
                    maze[i, 0].GetPosition().z * koef - 10), Quaternion.identity);
            if (!maze[i, maze.GetLength(1) - 1].WallBottom)
                Instantiate(_spawner, new Vector3(maze[i, maze.GetLength(1) - 1].GetPosition().x * koef + koef/2, 2,
                    maze[i, maze.GetLength(1) - 1].GetPosition().z * koef + 10), Quaternion.identity);
        }

        for (int i = 0; i < maze.GetLength(1) - 1; i++)
        {
            if (!maze[0, i].WallLeft)
            {
                Instantiate(_spawner, new Vector3(maze[0, i].GetPosition().x * koef - 10, 2,
                    maze[0, i].GetPosition().z * koef + koef/2), Quaternion.identity);
            }
        }
    }
 
}
