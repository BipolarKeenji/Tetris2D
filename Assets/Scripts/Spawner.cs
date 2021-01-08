using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] tetrominoes;
    GameObject newTetromino;

    private int roundedPosX;
    private int roundedPosY;

    // Start is called before the first frame update
    private void Start()
    {
        SpawnTetromino();
    }

    public void SpawnTetromino()
    {
        newTetromino = Instantiate(tetrominoes[Random.Range(0, tetrominoes.Length)], transform.position, Quaternion.identity);

        int randomPosX = Random.Range(0, Move.width);
        int posY = Move.height;
        newTetromino.transform.position = new Vector3(randomPosX, posY, 0);

        int angle = GetRandomAngle();
        newTetromino.transform.RotateAround(newTetromino.transform.TransformPoint(Move.rotationPoint), new Vector3(0, 0, 1), -angle);

        List<string> invalidMoves = IsSpawnedPositionInvalid();
        while (invalidMoves.Contains("up"))
        {
            newTetromino.transform.position = new Vector3(randomPosX, --posY, 0);
            invalidMoves = IsSpawnedPositionInvalid();
        }
        while (invalidMoves.Contains("left"))
        {
            newTetromino.transform.position = new Vector3(++randomPosX, posY, 0);
            invalidMoves = IsSpawnedPositionInvalid();
        }
        while (invalidMoves.Contains("right"))
        {
            newTetromino.transform.position = new Vector3(--randomPosX, posY, 0);
            invalidMoves = IsSpawnedPositionInvalid();
        }

        MoveLastSpawnedTetrominoUp();
    }

    private List<string> IsSpawnedPositionInvalid()
    {
        List<string> invalidMoves = new List<string>();

        foreach (Transform child in newTetromino.transform)
        {
            roundedPosX = Mathf.RoundToInt(child.transform.position.x);
            roundedPosY = Mathf.RoundToInt(child.transform.position.y);

            if (roundedPosX < 0)
                invalidMoves.Add("left");

            if (roundedPosX >= Move.width)
                invalidMoves.Add("right");

            if (roundedPosY > Move.height)
                invalidMoves.Add("up");
        }

        return invalidMoves;
    }

    private void MoveLastSpawnedTetrominoUp()
    {
        foreach (Transform child in newTetromino.transform)
        {
            roundedPosX = Mathf.RoundToInt(child.transform.position.x);
            roundedPosY = Mathf.RoundToInt(child.transform.position.y);

            try
            {
                while (Move.grid[roundedPosX, roundedPosY])
                {
                    newTetromino.transform.position += new Vector3(0, 1, 0);
                    roundedPosX = Mathf.RoundToInt(child.transform.position.x);
                    roundedPosY = Mathf.RoundToInt(child.transform.position.y);
                }
            }
            catch
            {

            }
        }
    }

    private int GetRandomAngle()
    {
        int randomNumber = Random.Range(0, 4);
        int angle = 0;

        switch (randomNumber)
        {
            case 0:
                angle = 0;
                break;
            case 1:
                angle = 90;
                break;
            case 2:
                angle = 180;
                break;
            case 3:
                angle = 270;
                break;
        }

        return angle;
    }
}
