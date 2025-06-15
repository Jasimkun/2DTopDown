using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps; // Tilemap�� ����ϱ� ���� �ʿ��մϴ�.

// �̷� ������ ���� ���� (Cell�� EDir�� �����ϰ� ���)
public enum EMazeDir
{
    eRight,
    eLeft,
    eDown,
    eUp
}

public class MazeGenerator : MonoBehaviour
{
    [Header("Tilemaps & Tiles")]
    [SerializeField] private Tilemap groundTilemap; // ���� Ÿ�ϸ�
    [SerializeField] private Tilemap wallTilemap;   // �� Ÿ�ϸ�
    [SerializeField] private TileBase groundTile;   // ���� Ÿ�� (��)
    [SerializeField] private TileBase wallTile;     // �� Ÿ��

    [Header("Maze Settings")]
    [SerializeField, Tooltip("�̷� ���� ũ��. (��: 2 -> 2x2 Ÿ���� �̷��� 1ĭ)")]
    private int mazeGridSize = 2; // �̷� �׸����� �� ĭ�� ���� Ÿ�� �� ĭ�� �������� (��: 2�� 2x2 Ÿ���� �̷��� �� ĭ)

    // GlobalDebugLogger�� LogPanelController�� ����� Ȱ���ϱ� ���� Instance�� �����ɴϴ�.
    // ������ �� ��ũ��Ʈ�� �� ���� �������� �����ϹǷ� Debug.Log�� ����ϰڽ��ϴ�.

    // MapGeneratorIssac�� cellSize�� tileNumPerCell�� �޾ƿ� �� �־�� �մϴ�.
    // Init �Լ��� ���� �ܺο��� ���Թ޴� ���� �����ϴ�.
    private int _cellSize; // MapGeneratorIssac�� cellSize
    private int _tileNumPerCell; // MapGeneratorIssac�� tileNumPerCell

    // �� ��ũ��Ʈ�� �ν����Ϳ� �ٴ� ��쿡 Awake���� �ʱ�ȭ�� �� ������,
    // MapGeneratorIssac���� �������� ȣ���ϴ� ���� �Ϲ����Դϴ�.
    // ���⼭�� Init �޼��带 ���� �ʼ� ������ ���Թ޴� ������� �����մϴ�.
    public void Init(Tilemap _groundTilemap, Tilemap _wallTilemap, TileBase _groundTile, TileBase _wallTile, int cellSize, int tileNumPerCell)
    {
        groundTilemap = _groundTilemap;
        wallTilemap = _wallTilemap;
        groundTile = _groundTile;
        wallTile = _wallTile;
        _cellSize = cellSize;
        _tileNumPerCell = tileNumPerCell;

        if (mazeGridSize <= 0 || _tileNumPerCell % mazeGridSize != 0)
        {
            Debug.LogError("MazeGenerator: mazeGridSize�� 0���� Ŀ�� �ϸ�, tileNumPerCell�� ������� �մϴ�. ���� tileNumPerCell: " + _tileNumPerCell + ", mazeGridSize: " + mazeGridSize);
            // ���� �߻� �� �⺻������ ���� ���� �Ǵ� �Լ� ����
            if (mazeGridSize <= 0) mazeGridSize = 2;
            if (_tileNumPerCell % mazeGridSize != 0) mazeGridSize = _tileNumPerCell; // �ִ� ũ��� ����
        }
    }


    /// <summary>
    /// ������ Cell ���ο� �̷θ� �����ϰ� Ÿ�ϸʿ� �׸��ϴ�.
    /// �� �޼���� MapGeneratorIssac�� DrawCell() �Լ����� ȣ��Ǿ�� �մϴ�.
    /// </summary>
    /// <param name="cell">�̷θ� ������ ��� Cell ��ü</param>
    /// <param name="roomID">���� ���� ID (DungeonManager AddToTilemapDic�� ����)</param>
    public void GenerateMazeInCell(Cell cell, int roomID, bool isBossRoom)
    {
        if (cell == null)
        {
            Debug.LogError("MazeGenerator: Null Cell provided for maze generation.");
            return;
        }

        // �̷� �׸����� ����/���� ĭ ��
        int mazeGridCols = _tileNumPerCell / mazeGridSize;
        int mazeGridRows = _tileNumPerCell / mazeGridSize;

        // �̷� �迭 �ʱ�ȭ (true: ��, false: ��)
        // 2���� �迭�� [row, col] �Ǵ� [y, x] ������ ���� ���˴ϴ�.
        // ���⼭�� [��, ��] = [mazeGridRows, mazeGridCols]�� �ϰڽ��ϴ�.
        bool[,] maze = new bool[mazeGridRows * 2 + 1, mazeGridCols * 2 + 1]; // +1�� ���� ������ ����

        // ��� Ÿ���� ������ �ʱ�ȭ�մϴ�.
        for (int r = 0; r < mazeGridRows * 2 + 1; r++)
        {
            for (int c = 0; c < mazeGridCols * 2 + 1; c++)
            {
                maze[r, c] = false; // false�� ������ ���� (�ʱ� ���´� ��� ��)
            }
        }

        // �̷� ���� ������ ���� (���� �Ǵ� �߾�)
        Vector2Int startPoint = new Vector2Int(Random.Range(0, mazeGridCols), Random.Range(0, mazeGridRows));

        // ��� ��Ʈ��ŷ �˰��� ����
        GeneratePath(maze, startPoint.y * 2 + 1, startPoint.x * 2 + 1, mazeGridRows * 2 + 1, mazeGridCols * 2 + 1);

        // ������ �̷θ� ���� Tilemap�� �׸��ϴ�.
        DrawMazeTiles(cell, maze, roomID, isBossRoom);
    }

    // ��� ��Ʈ��ŷ �̷� ���� �Լ�
    private void GeneratePath(bool[,] maze, int r, int c, int maxRows, int maxCols)
    {
        maze[r, c] = true; // ���� ��ġ�� ��� ǥ��

        // �����¿� ������ ������ Ž��
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(0, 2),  // Up (maze grid coord)
            new Vector2Int(0, -2), // Down (maze grid coord)
            new Vector2Int(2, 0),  // Right (maze grid coord)
            new Vector2Int(-2, 0)  // Left (maze grid coord)
        };
        ShuffleList(directions); // ������ �������� ����

        foreach (Vector2Int dir in directions)
        {
            int nextR = r + dir.y;
            int nextC = c + dir.x;

            // ���� ��ġ�� �̷� ���� ���� �ְ�, ���� �湮���� ����(����) ���̶��
            if (nextR >= 0 && nextR < maxRows && nextC >= 0 && nextC < maxCols && !maze[nextR, nextC])
            {
                // ���� ��ġ�� ���� ��ġ ������ ���� �㹴�ϴ� (��� ����ϴ�)
                maze[r + dir.y / 2, c + dir.x / 2] = true;
                GeneratePath(maze, nextR, nextC, maxRows, maxCols); // ��� ȣ��
            }
        }
    }

    // �̷� �迭�� ���� Tilemap�� �׸��� �Լ�
    private void DrawMazeTiles(Cell cell, bool[,] maze, int roomID, bool isBossRoom)
    {
        // Cell�� Tilemap Local Position�� �������� Ÿ�ϸʿ� �׸��ϴ�.
        // cell.tilemapLocalPos�� Cell�� ���� �Ʒ� Ÿ���� ���� ��ǥ�Դϴ�.
        Vector3Int cellBasePos = cell.tilemapLocalPos;

        // �̷� �׸����� �� ĭ�� ��ȸ�ϸ� Ÿ���� �׸��ϴ�.
        for (int r = 0; r < maze.GetLength(0); r++) // maze �迭�� �� (Y��ǥ)
        {
            for (int c = 0; c < maze.GetLength(1); c++) // maze �迭�� �� (X��ǥ)
            {
                // �̷� �׸��� ��ǥ�� ���� Ÿ�ϸ� ��ǥ�� ��ȯ
                // cellBasePos�� �ش� Cell�� (0,0) Ÿ�ϸ� ��ǥ�Դϴ�.
                // �̷� �׸����� (c, r) ��ġ�� (c * mazeGridSize, r * mazeGridSize) Ÿ�Ϸ� ���ε˴ϴ�.
                // ������ �̷� �迭�� �� ���� 2*N+1 ũ���̹Ƿ�, ��꿡 �����ؾ� �մϴ�.
                // maze[r, c]�� (r, c) ��ǥ�� �̷� �׸��� ���Դϴ�.
                // �̰��� ���� Ÿ�ϸ� ��ǥ�� ��ȯ�Ϸ���:
                // X = cellBasePos.x + (c * (mazeGridSize / 2))
                // Y = cellBasePos.y + (r * (mazeGridSize / 2))
                // ���⼭�� mazeGridSize�� 2�� �� (��1ĭ, ��1ĭ) �����̹Ƿ�,
                // c, r ��ü�� Tilemap�� ���� ��ǥ�� �����ϰ� �׸��ϴ�.

                // �� maze[r,c]�� ���� Ÿ�� �� ĭ�� �ش��մϴ� (mazeGridSize�� �̹� �� ���� ũ�⸦ �����Ѱ����� ����)
                // ���� �������� Ÿ�ϸ� ��ǥ ��ȯ
                Vector3Int currentTilePos = new Vector3Int(cellBasePos.x + c, cellBasePos.y + r, 0);

                if (maze[r, c]) // �� (true)
                {
                    if (!isBossRoom)
                    {
                        groundTilemap.SetTile(currentTilePos, groundTile);
                    }
                    else
                    {
                        groundTilemap.SetTile(currentTilePos, groundTile); // �����뵵 ���� �Ϲ� Ÿ�Ϸ�? �ƴϸ� bossGroundTile?
                                                                           // ���⼭�� bossGroundTile�� �̹� �ٴ����� ������Ƿ�, 
                                                                           // �̷��� ���� ���� ���� ���� Ÿ���� �����ϴ� �� �ڿ��������ϴ�.
                                                                           // ��, ���� �׸��ϴ�.
                    }
                    // DungeonManager�� �߰�: ���� Ÿ���� �̹� DrawTilesInCell���� �߰��Ǿ��� ���ɼ��� �����Ƿ�
                    // ���⼭�� ���� �׸��� �Ϳ� �����ϰų�, ��� �̷� Ÿ���� �ٽ� ����մϴ�.
                    // ��� Ÿ���� �ٽ� ����ϴ� ���� ���� �����մϴ�.
                    DungeonManager.GetInstance().AddToTilemapDic(roomID, groundTilemap, currentTilePos);
                    wallTilemap.SetTile(currentTilePos, null); // Ȥ�� ���� �׷��� �ִٸ� �����ݴϴ�.
                }
                else // �� (false)
                {
                    wallTilemap.SetTile(currentTilePos, wallTile);
                    DungeonManager.GetInstance().AddToTilemapDic(roomID, wallTilemap, currentTilePos);
                    groundTilemap.SetTile(currentTilePos, null); // Ȥ�� ������ �׷��� �ִٸ� �����ݴϴ�.
                }
            }
        }

        // Cell�� ��迡 �ִ� �� ��ġ�� ��� Ȯ���� ����� �ݴϴ�.
        // MapGeneratorIssac�� GenerateDoor ������ �����ؾ� �մϴ�.
        // �� �κ��� DrawCell �Լ����� ���� ������ �Ŀ� ȣ��Ǿ�� �մϴ�.
        // �ƴϸ� DrawDoor �Լ����� �ش� Ÿ���� ��� �ٲ��ִ� ������ �ʿ��մϴ�.
        // ���� ������ �̷ΰ� ���� �׷����� ���� ���߿� �׷����Ƿ�,
        // DrawDoor���� wallTilemap.SetTile(pos, null); �� �����Ƿ� �� ��ġ�� �׻� ����Դϴ�.
        // �׷��Ƿ� �� MazeGenerator���� ������ ���� ó���� �ʿ�� �����ϴ�.
    }

    // List�� �������� ���� ��ƿ��Ƽ �Լ� (MapGeneratorIssac���� ������)
    private void ShuffleList<T>(List<T> list)
    {
        int random1, random2;
        T temp;

        for (int i = 0; i < list.Count; ++i)
        {
            random1 = UnityEngine.Random.Range(0, list.Count);
            random2 = UnityEngine.Random.Range(0, list.Count);

            temp = list[random1];
            list[random1] = list[random2];
            list[random2] = temp;
        }
    }
}