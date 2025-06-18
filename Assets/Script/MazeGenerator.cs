using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject GroundPrefab;
    public GameObject WallPrefab;

    public int width;
    public int height;

    public float tileRatio = 1f;

    private bool[,] GridTile;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap(width, height);
        GenerateTile(GridTile);
    }

    public void GenerateMap(int _width, int _height)
    {
        GridTile = new bool[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if(x % 2 == 0 || y % 2 == 0)
                {
                    //  벽
                    GridTile[x, y] = false;
                }
                else
                {
                    GridTile[x, y] = true;
                }
            }
        }

        for (int x = 0; x < _width; x++)
        {
            for (int y =0; y < _height; y++)
            {
                if(x % 2 == 0 || y % 2 == 0)
                {
                    continue;
                }

                //마지막 타일일 경우 벽으로 둔다
                if(y == _height - 2 && x == _width - 2)
                {
                    continue;
                }

                //y의 다음 값이 가장자리 벽일 경우
                if(y == _height - 2)
                {
                    //다른 방향으로 벽을 뚫는다
                    GridTile[x + 1, y] = true;
                    continue;
                }

                //x의 다음 값이 가장자리 벽일 경우
                if(x == _width - 2)
                {
                    //다른 방향으로 벽을 뚫는다
                    GridTile[x, y + 1] = true;
                    continue;
                }

                //랜덤한 확률로 오른쪽 또는 왼쪽으로 벽을 뚫는다
                if(Random.Range(0,2) == 0)
                {
                    GridTile[x + 1,y] = true;
                }
                else
                {
                    GridTile[x, y + 1] = true;
                }
            }
        }
    }

    protected void GenerateTile(bool[,] _Grid)
    {
        for (int x = 0; x < _Grid.GetLength(0); x++)
        {
            for (int y = 0; y < _Grid.GetLength(1); y++)
            {
                GameObject tileObj = null;
                if (! _Grid[x,y])
                {
                    tileObj = Instantiate <GameObject> (WallPrefab);
                }
                else
                {
                    tileObj = Instantiate<GameObject>(GroundPrefab);
                }
                if(tileObj != null)
                {
                    tileObj.transform.position = new Vector2(x * tileRatio, y * tileRatio);
                }
            }
        }
    }
    
}
