using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorCell : MonoBehaviour
{
    public int X;
    public int Y;
    public bool WallLeft = true;
    public bool WallBottom = true;
    public bool Visited = false;

    public Vector3 GetPosition()
    {
        return new Vector3(X, 0, Y);
    }
}

public class MazeGenerator
{
    public int Width = 10;
    public int Height = 10;

    public MazeGeneratorCell[,] GenerateMaze()
    {
        MazeGeneratorCell[,] maze = new MazeGeneratorCell[Width,Height];

        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                maze[i, j] = new MazeGeneratorCell {X = i, Y = j};
            }
        }
        

        for (int i = 0; i < maze.GetLength(0); i++)
        {
            maze[i, Height - 1].WallLeft = false;
        }
        for (int i = 0; i < maze.GetLength(1); i++)
        {
            maze[Width - 1, i].WallBottom = false;
        }
        
        RemoveWalls(maze);
        
        return maze;
    }

    public void RemoveWalls(MazeGeneratorCell[,] maze)
    {
        MazeGeneratorCell currentCell = maze[0, 0];
        currentCell.Visited = true;
        
        Stack<MazeGeneratorCell> stack = new Stack<MazeGeneratorCell>();
        do
        {
            List<MazeGeneratorCell> unvisitedCells = new List<MazeGeneratorCell>();

            int x = currentCell.X;
            int y = currentCell.Y;
            
            if (x > 0 && !maze[x -1, y].Visited) unvisitedCells.Add(maze[x -1, y]);
            if (y > 0 && !maze[x, y - 1].Visited) unvisitedCells.Add(maze[x, y - 1]);
            if (x < Width - 2 && !maze[x + 1, y].Visited) unvisitedCells.Add(maze[x + 1, y]);
            if (y < Height - 2 && !maze[x, y + 1].Visited) unvisitedCells.Add(maze[x, y + 1]);

            if (unvisitedCells.Count > 0)
            {
                MazeGeneratorCell chosen = unvisitedCells[Random.Range(0, unvisitedCells.Count)];
                RemoveCells(currentCell, chosen);
                chosen.Visited = true;
                stack.Push(chosen);
                currentCell = chosen;
            }
            else
            {
                currentCell = stack.Pop();
            }

        } while (stack.Count > 0);
        
        CreateExit(maze);
    }

    private void RemoveCells(MazeGeneratorCell current, MazeGeneratorCell chosen)
    {
        if (current.X == chosen.X)
        {
            if (current.Y > chosen.Y)
            {
                current.WallBottom = false;
            }
            else if (current.Y < chosen.Y)
            {
                chosen.WallBottom = false;
            }
        }
        else
        {
            if (current.X > chosen.X)
            {
                current.WallLeft = false;
            }
            else if (current.X < chosen.X)
            {
                chosen.WallLeft = false;
            }
        }

        int temp = Random.Range(1, 3);
        if (temp == 2)
        {
            if (chosen.X != 0)
                chosen.WallLeft = false;
        }
        else if (temp == 4)
        {
            if (chosen.Y != 0)
                chosen.WallBottom = false;
        }
    }

    private void CreateExit(MazeGeneratorCell[,] maze)
    {
        int randomNumber = Random.Range(0, maze.GetLength(0) - 2);
        maze[randomNumber, 0].WallBottom = false;
        randomNumber = Random.Range(0, maze.GetLength(0) - 2);
        maze[randomNumber, maze.GetLength(1) - 1].WallBottom = false;
        randomNumber = Random.Range(0, maze.GetLength(1) - 2);
        maze[0, randomNumber].WallLeft = false;
    }
}
