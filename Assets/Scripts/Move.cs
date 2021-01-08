using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private float previousTime;
    private float moveSpeed = 0.4f;
    public static int width = 10;
    public static int height = 20;
    private int roundedPosX;
    private int roundedPosY;
    public static Vector3 rotationPoint;
    private bool endOfGame = false;
    public static Transform[,] grid = new Transform[width, height];

    // Update is called once per frame
    private void Update()
    {
        if (!endOfGame)
            ProcessMove();
    }

    private void ProcessMove()
    {
        if (Time.time - previousTime > ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) ? moveSpeed / 10 : moveSpeed))
        {
            transform.position -= new Vector3(0, 1, 0);

            if (!IsMoveValid())
            {
                transform.position += new Vector3(0, 1, 0);
                AddToGrid();
                CheckForLines();
                this.enabled = false;
                if (!endOfGame)
                {
                    FindObjectOfType<Spawner>().DestroyGhostTetromino();
                    FindObjectOfType<Spawner>().SpawnTetromino();
                }
            }

            previousTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            Rotate();

            FindObjectOfType<Spawner>().FollowGhostTetromino();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            transform.position -= new Vector3(1, 0, 0);
            if (!IsMoveValid())
                transform.position += new Vector3(1, 0, 0);

            FindObjectOfType<Spawner>().FollowGhostTetromino();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            transform.position += new Vector3(1, 0, 0);
            if (!IsMoveValid())
                transform.position -= new Vector3(1, 0, 0);

            FindObjectOfType<Spawner>().FollowGhostTetromino();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) 
        {
            HardDrop();
        }
    }

    private void Rotate()
    {
        if (!transform.name.Contains("Tetromino O"))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);

            foreach (Transform child in transform)
            {
                roundedPosX = Mathf.RoundToInt(child.transform.position.x);
                roundedPosY = Mathf.RoundToInt(child.transform.position.y);

                try
                {
                    if (grid[roundedPosX, roundedPosY])
                    {
                        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
                        return;
                    }
                }
                catch
                {
                    while (roundedPosX <= 0)
                    {
                        transform.position += new Vector3(1, 0, 0);
                        roundedPosX = Mathf.RoundToInt(child.transform.position.x);
                    }
                    while (roundedPosX >= width)
                    {
                        transform.position -= new Vector3(1, 0, 0);
                        roundedPosX = Mathf.RoundToInt(child.transform.position.x);
                    }
                    while (roundedPosY <= 0)
                    {
                        transform.position += new Vector3(0, 1, 0);
                        roundedPosY = Mathf.RoundToInt(child.transform.position.y);
                    }
                    while (roundedPosY >= height)
                    {
                        transform.position -= new Vector3(0, 1, 0);
                        roundedPosY = Mathf.RoundToInt(child.transform.position.y);
                    }
                }
            }
        }
    }

    private void HardDrop()
    {
        while (IsMoveValid())
        {
            transform.position -= new Vector3(0, 1, 0);
        }

        if (!IsMoveValid())
        {
            transform.position += new Vector3(0, 1, 0);
        }
    }

    private bool IsMoveValid()
    {
        foreach (Transform child in transform)
        {
            roundedPosX = Mathf.RoundToInt(child.transform.position.x);
            roundedPosY = Mathf.RoundToInt(child.transform.position.y);

            if (roundedPosX < 0 || roundedPosX >= width || roundedPosY < 0)
                return false;

            if (grid[roundedPosX, roundedPosY])
                return false;
        }

        return true;
    }

    private void AddToGrid()
    {
        foreach (Transform child in transform)
        {
            roundedPosX = Mathf.RoundToInt(child.transform.position.x);
            roundedPosY = Mathf.RoundToInt(child.transform.position.y);

            try
            {
                grid[roundedPosX, roundedPosY] = child;
            }
            catch
            {
                endOfGame = !endOfGame;
                return;
            }
        }
    }

    private void CheckForLines()
    {
        for (int i = height - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                DeleteLine(i);
                DropLineDown(i);
            }
        }
    }

    private bool HasLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            if (!grid[j, i])
                return false;
        }

        return true;
    }

    private void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
        }
    }

    private void DropLineDown(int i)
    {
        for (int k = i; k < height; k++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, k])
                {
                    grid[j, k - 1] = grid[j, k];
                    grid[j, k] = null;
                    grid[j, k - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }
}
