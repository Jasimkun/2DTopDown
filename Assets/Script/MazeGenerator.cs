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
                    //  ��
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

                //������ Ÿ���� ��� ������ �д�
                if(y == _height - 2 && x == _width - 2)
                {
                    continue;
                }

                //y�� ���� ���� �����ڸ� ���� ���
                if(y == _height - 2)
                {
                    //�ٸ� �������� ���� �մ´�
                    GridTile[x + 1, y] = true;
                    continue;
                }

                //x�� ���� ���� �����ڸ� ���� ���
                if(x == _width - 2)
                {
                    //�ٸ� �������� ���� �մ´�
                    GridTile[x, y + 1] = true;
                    continue;
                }

                //������ Ȯ���� ������ �Ǵ� �������� ���� �մ´�
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
