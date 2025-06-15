using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps; // Tilemap을 사용하기 위해 필요합니다.

// 미로 생성을 위한 방향 (Cell의 EDir과 동일하게 사용)
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
    [SerializeField] private Tilemap groundTilemap; // 지면 타일맵
    [SerializeField] private Tilemap wallTilemap;   // 벽 타일맵
    [SerializeField] private TileBase groundTile;   // 지면 타일 (길)
    [SerializeField] private TileBase wallTile;     // 벽 타일

    [Header("Maze Settings")]
    [SerializeField, Tooltip("미로 격자 크기. (예: 2 -> 2x2 타일이 미로의 1칸)")]
    private int mazeGridSize = 2; // 미로 그리드의 한 칸이 실제 타일 몇 칸을 차지할지 (예: 2는 2x2 타일이 미로의 한 칸)

    // GlobalDebugLogger와 LogPanelController의 기능을 활용하기 위해 Instance를 가져옵니다.
    // 하지만 이 스크립트는 맵 생성 로직에만 집중하므로 Debug.Log를 사용하겠습니다.

    // MapGeneratorIssac의 cellSize와 tileNumPerCell을 받아올 수 있어야 합니다.
    // Init 함수를 통해 외부에서 주입받는 것이 좋습니다.
    private int _cellSize; // MapGeneratorIssac의 cellSize
    private int _tileNumPerCell; // MapGeneratorIssac의 tileNumPerCell

    // 이 스크립트가 인스펙터에 붙는 경우에 Awake에서 초기화할 수 있지만,
    // MapGeneratorIssac에서 동적으로 호출하는 것이 일반적입니다.
    // 여기서는 Init 메서드를 통해 필수 정보를 주입받는 방식으로 설계합니다.
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
            Debug.LogError("MazeGenerator: mazeGridSize는 0보다 커야 하며, tileNumPerCell의 약수여야 합니다. 현재 tileNumPerCell: " + _tileNumPerCell + ", mazeGridSize: " + mazeGridSize);
            // 오류 발생 시 기본값으로 강제 설정 또는 함수 종료
            if (mazeGridSize <= 0) mazeGridSize = 2;
            if (_tileNumPerCell % mazeGridSize != 0) mazeGridSize = _tileNumPerCell; // 최대 크기로 설정
        }
    }


    /// <summary>
    /// 지정된 Cell 내부에 미로를 생성하고 타일맵에 그립니다.
    /// 이 메서드는 MapGeneratorIssac의 DrawCell() 함수에서 호출되어야 합니다.
    /// </summary>
    /// <param name="cell">미로를 생성할 대상 Cell 객체</param>
    /// <param name="roomID">현재 방의 ID (DungeonManager AddToTilemapDic에 전달)</param>
    public void GenerateMazeInCell(Cell cell, int roomID, bool isBossRoom)
    {
        if (cell == null)
        {
            Debug.LogError("MazeGenerator: Null Cell provided for maze generation.");
            return;
        }

        // 미로 그리드의 가로/세로 칸 수
        int mazeGridCols = _tileNumPerCell / mazeGridSize;
        int mazeGridRows = _tileNumPerCell / mazeGridSize;

        // 미로 배열 초기화 (true: 길, false: 벽)
        // 2차원 배열은 [row, col] 또는 [y, x] 순서로 많이 사용됩니다.
        // 여기서는 [행, 열] = [mazeGridRows, mazeGridCols]로 하겠습니다.
        bool[,] maze = new bool[mazeGridRows * 2 + 1, mazeGridCols * 2 + 1]; // +1은 벽을 포함한 격자

        // 모든 타일을 벽으로 초기화합니다.
        for (int r = 0; r < mazeGridRows * 2 + 1; r++)
        {
            for (int c = 0; c < mazeGridCols * 2 + 1; c++)
            {
                maze[r, c] = false; // false는 벽으로 간주 (초기 상태는 모두 벽)
            }
        }

        // 미로 생성 시작점 설정 (랜덤 또는 중앙)
        Vector2Int startPoint = new Vector2Int(Random.Range(0, mazeGridCols), Random.Range(0, mazeGridRows));

        // 재귀 백트래킹 알고리즘 시작
        GeneratePath(maze, startPoint.y * 2 + 1, startPoint.x * 2 + 1, mazeGridRows * 2 + 1, mazeGridCols * 2 + 1);

        // 생성된 미로를 실제 Tilemap에 그립니다.
        DrawMazeTiles(cell, maze, roomID, isBossRoom);
    }

    // 재귀 백트래킹 미로 생성 함수
    private void GeneratePath(bool[,] maze, int r, int c, int maxRows, int maxCols)
    {
        maze[r, c] = true; // 현재 위치를 길로 표시

        // 상하좌우 무작위 순서로 탐색
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(0, 2),  // Up (maze grid coord)
            new Vector2Int(0, -2), // Down (maze grid coord)
            new Vector2Int(2, 0),  // Right (maze grid coord)
            new Vector2Int(-2, 0)  // Left (maze grid coord)
        };
        ShuffleList(directions); // 방향을 무작위로 섞음

        foreach (Vector2Int dir in directions)
        {
            int nextR = r + dir.y;
            int nextC = c + dir.x;

            // 다음 위치가 미로 범위 내에 있고, 아직 방문하지 않은(벽인) 곳이라면
            if (nextR >= 0 && nextR < maxRows && nextC >= 0 && nextC < maxCols && !maze[nextR, nextC])
            {
                // 현재 위치와 다음 위치 사이의 벽을 허뭅니다 (길로 만듭니다)
                maze[r + dir.y / 2, c + dir.x / 2] = true;
                GeneratePath(maze, nextR, nextC, maxRows, maxCols); // 재귀 호출
            }
        }
    }

    // 미로 배열을 실제 Tilemap에 그리는 함수
    private void DrawMazeTiles(Cell cell, bool[,] maze, int roomID, bool isBossRoom)
    {
        // Cell의 Tilemap Local Position을 기준으로 타일맵에 그립니다.
        // cell.tilemapLocalPos는 Cell의 왼쪽 아래 타일의 절대 좌표입니다.
        Vector3Int cellBasePos = cell.tilemapLocalPos;

        // 미로 그리드의 각 칸을 순회하며 타일을 그립니다.
        for (int r = 0; r < maze.GetLength(0); r++) // maze 배열의 행 (Y좌표)
        {
            for (int c = 0; c < maze.GetLength(1); c++) // maze 배열의 열 (X좌표)
            {
                // 미로 그리드 좌표를 실제 타일맵 좌표로 변환
                // cellBasePos는 해당 Cell의 (0,0) 타일맵 좌표입니다.
                // 미로 그리드의 (c, r) 위치는 (c * mazeGridSize, r * mazeGridSize) 타일로 매핑됩니다.
                // 하지만 미로 배열은 벽 포함 2*N+1 크기이므로, 계산에 주의해야 합니다.
                // maze[r, c]는 (r, c) 좌표의 미로 그리드 셀입니다.
                // 이것을 실제 타일맵 좌표로 변환하려면:
                // X = cellBasePos.x + (c * (mazeGridSize / 2))
                // Y = cellBasePos.y + (r * (mazeGridSize / 2))
                // 여기서는 mazeGridSize가 2일 때 (길1칸, 벽1칸) 격자이므로,
                // c, r 자체를 Tilemap의 실제 좌표로 생각하고 그립니다.

                // 각 maze[r,c]는 실제 타일 한 칸에 해당합니다 (mazeGridSize는 이미 각 셀의 크기를 조절한것으로 간주)
                // 따라서 직접적인 타일맵 좌표 변환
                Vector3Int currentTilePos = new Vector3Int(cellBasePos.x + c, cellBasePos.y + r, 0);

                if (maze[r, c]) // 길 (true)
                {
                    if (!isBossRoom)
                    {
                        groundTilemap.SetTile(currentTilePos, groundTile);
                    }
                    else
                    {
                        groundTilemap.SetTile(currentTilePos, groundTile); // 보스룸도 길은 일반 타일로? 아니면 bossGroundTile?
                                                                           // 여기서는 bossGroundTile을 이미 바닥으로 깔았으므로, 
                                                                           // 미로의 길은 기존 보스 지면 타일을 유지하는 게 자연스럽습니다.
                                                                           // 즉, 벽만 그립니다.
                    }
                    // DungeonManager에 추가: 지면 타일은 이미 DrawTilesInCell에서 추가되었을 가능성이 높으므로
                    // 여기서는 벽을 그리는 것에 집중하거나, 모든 미로 타일을 다시 등록합니다.
                    // 모든 타일을 다시 등록하는 것이 가장 안전합니다.
                    DungeonManager.GetInstance().AddToTilemapDic(roomID, groundTilemap, currentTilePos);
                    wallTilemap.SetTile(currentTilePos, null); // 혹시 벽이 그려져 있다면 지워줍니다.
                }
                else // 벽 (false)
                {
                    wallTilemap.SetTile(currentTilePos, wallTile);
                    DungeonManager.GetInstance().AddToTilemapDic(roomID, wallTilemap, currentTilePos);
                    groundTilemap.SetTile(currentTilePos, null); // 혹시 지면이 그려져 있다면 지워줍니다.
                }
            }
        }

        // Cell의 경계에 있는 문 위치를 길로 확실히 만들어 줍니다.
        // MapGeneratorIssac의 GenerateDoor 로직과 연동해야 합니다.
        // 이 부분은 DrawCell 함수에서 문이 생성된 후에 호출되어야 합니다.
        // 아니면 DrawDoor 함수에서 해당 타일을 길로 바꿔주는 로직이 필요합니다.
        // 현재 로직은 미로가 먼저 그려지고 문이 나중에 그려지므로,
        // DrawDoor에서 wallTilemap.SetTile(pos, null); 이 있으므로 문 위치는 항상 통로입니다.
        // 그러므로 이 MazeGenerator에서 별도로 문을 처리할 필요는 없습니다.
    }

    // List를 무작위로 섞는 유틸리티 함수 (MapGeneratorIssac에서 가져옴)
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