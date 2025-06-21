using System.Collections;
using System.Collections.Generic; // List<T>를 사용하기 위해 필요합니다.
using UnityEngine;
using UnityEngine.Tilemaps;

public class BinaryTreeMazeGenerator : MonoBehaviour
{
    [Header("미로 설정")]
    public Vector2Int mazeSize = new Vector2Int(21, 21);
    public int minDistanceToStart = 5;

    [Header("타일맵 참조")]
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;

    [Header("타일 참조")]
    // ⬇️ [변경] 여러 개의 벽 타일을 저장할 배열 또는 리스트
    public TileBase[] wallTiles;    // 벽 타일들의 배열
    // ⬇️ [변경] 여러 개의 길 타일을 저장할 배열 또는 리스트
    public TileBase[] groundTiles;  // 길(바닥) 타일들의 배열
    public TileBase destinationTile; // 목적지 타일 (단일)

    private int[,] mazeMap;

    [HideInInspector]
    public Vector2Int destinationPoint;
    [HideInInspector]
    public Vector2Int playerStartPoint;

    void Start()
    {
        // 타일 배열이 비어 있는지 확인하여 오류 방지
        if (wallTiles == null || wallTiles.Length == 0)
        {
            Debug.LogError("Wall Tiles 배열이 비어 있습니다. 최소 하나 이상의 벽 타일을 할당해야 합니다.", this);
            return;
        }
        if (groundTiles == null || groundTiles.Length == 0)
        {
            Debug.LogError("Ground Tiles 배열이 비어 있습니다. 최소 하나 이상의 길 타일을 할당해야 합니다.", this);
            return;
        }
        if (destinationTile == null)
        {
            Debug.LogWarning("Destination Tile이 할당되지 않았습니다. 목적지가 일반 길 타일로 표시될 수 있습니다.", this);
            // destinationTile이 없으면 기본 groundTiles 중 하나를 사용하도록 폴백 설정
            destinationTile = groundTiles[0];
        }

        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        if (mazeSize.x % 2 == 0) mazeSize.x++;
        if (mazeSize.y % 2 == 0) mazeSize.y++;

        mazeMap = new int[mazeSize.x, mazeSize.y];
        for (int x = 0; x < mazeSize.x; x++)
        {
            for (int y = 0; y < mazeSize.y; y++)
            {
                mazeMap[x, y] = 0;
            }
        }

        for (int x = 1; x < mazeSize.x - 1; x += 2)
        {
            for (int y = 1; y < mazeSize.y - 1; y += 2)
            {
                mazeMap[x, y] = 1;

                if (x == mazeSize.x - 2)
                {
                    if (y + 2 < mazeSize.y) mazeMap[x, y + 1] = 1;
                    continue;
                }
                if (y == mazeSize.y - 2)
                {
                    if (x + 2 < mazeSize.x) mazeMap[x + 1, y] = 1;
                    continue;
                }

                if (Random.Range(0, 2) == 0)
                {
                    if (x + 2 < mazeSize.x) mazeMap[x + 1, y] = 1;
                }
                else
                {
                    if (y + 2 < mazeSize.y) mazeMap[x, y + 1] = 1;
                }
            }
        }

        // --- 플레이어 시작 지점을 미로 중앙으로 설정 ---
        playerStartPoint = new Vector2Int(mazeSize.x / 2, mazeSize.y / 2);

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int currentX = playerStartPoint.x + dx;
                int currentY = playerStartPoint.y + dy;

                if (currentX >= 0 && currentX < mazeSize.x &&
                    currentY >= 0 && currentY < mazeSize.y)
                {
                    mazeMap[currentX, currentY] = 1;
                }
            }
        }

        // --- BFS (Breadth-First Search)를 이용한 도달 가능한 길 찾기 ---
        List<Vector2Int> reachablePathCells = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        bool[,] visited = new bool[mazeSize.x, mazeSize.y];

        queue.Enqueue(playerStartPoint);
        visited[playerStartPoint.x, playerStartPoint.y] = true;
        reachablePathCells.Add(playerStartPoint);

        Vector2Int[] directions = {
            new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0)
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (neighbor.x >= 0 && neighbor.x < mazeSize.x &&
                    neighbor.y >= 0 && neighbor.y < mazeSize.y &&
                    !visited[neighbor.x, neighbor.y] && mazeMap[neighbor.x, neighbor.y] == 1)
                {
                    visited[neighbor.x, neighbor.y] = true;
                    queue.Enqueue(neighbor);
                    reachablePathCells.Add(neighbor);
                }
            }
        }

        // --- 가장 바깥쪽 벽을 뚫어 랜덤 목적지(출구) 생성 로직 ---
        List<Vector2Int> potentialExitPoints = new List<Vector2Int>();

        // 상단 경계 (y = mazeSize.y - 1)
        for (int x = 1; x < mazeSize.x - 1; x++)
        {
            if (mazeMap[x, mazeSize.y - 2] == 1)
            {
                potentialExitPoints.Add(new Vector2Int(x, mazeSize.y - 1));
            }
        }
        // 하단 경계 (y = 0)
        for (int x = 1; x < mazeSize.x - 1; x++)
        {
            if (mazeMap[x, 1] == 1)
            {
                potentialExitPoints.Add(new Vector2Int(x, 0));
            }
        }
        // 좌측 경계 (x = 0)
        for (int y = 1; y < mazeSize.y - 1; y++)
        {
            if (mazeMap[1, y] == 1)
            {
                potentialExitPoints.Add(new Vector2Int(0, y));
            }
        }
        // 우측 경계 (x = mazeSize.x - 1)
        for (int y = 1; y < mazeSize.y - 1; y++)
        {
            if (mazeMap[mazeSize.x - 2, y] == 1)
            {
                potentialExitPoints.Add(new Vector2Int(mazeSize.x - 1, y));
            }
        }

        if (potentialExitPoints.Count > 0)
        {
            Vector2Int chosenExit = new Vector2Int(-1, -1);
            int attempts = 0;
            const int maxAttempts = 200;

            while (attempts < maxAttempts)
            {
                int randomIndex = Random.Range(0, potentialExitPoints.Count);
                Vector2Int tempExit = potentialExitPoints[randomIndex];

                Vector2Int innerCell = tempExit;
                if (tempExit.x == 0) innerCell.x = 1;
                else if (tempExit.x == mazeSize.x - 1) innerCell.x = mazeSize.x - 2;
                else if (tempExit.y == 0) innerCell.y = 1;
                else if (tempExit.y == mazeSize.y - 1) innerCell.y = mazeSize.y - 2;

                if (innerCell.x < 0 || innerCell.x >= mazeSize.x ||
                    innerCell.y < 0 || innerCell.y >= mazeSize.y)
                {
                    attempts++;
                    continue;
                }

                if (reachablePathCells.Contains(innerCell))
                {
                    int manhattanDistance = Mathf.Abs(innerCell.x - playerStartPoint.x) + Mathf.Abs(innerCell.y - playerStartPoint.y);
                    if (manhattanDistance >= minDistanceToStart)
                    {
                        chosenExit = tempExit;
                        break;
                    }
                }
                attempts++;
            }

            if (chosenExit.x == -1)
            {
                Debug.LogWarning("적합한 랜덤 출구를 찾지 못했습니다. 우측 하단 벽에 출구를 생성합니다.");
                chosenExit = new Vector2Int(mazeSize.x - 1, mazeSize.y - 1);

                if (mazeSize.x - 2 >= 0 && mazeSize.y - 2 >= 0)
                {
                    mazeMap[mazeSize.x - 2, mazeSize.y - 1] = 1;
                    mazeMap[mazeSize.x - 1, mazeSize.y - 2] = 1;
                }
            }

            destinationPoint = chosenExit;
            mazeMap[destinationPoint.x, destinationPoint.y] = 2;

            if (destinationPoint.x == 0 && destinationPoint.x + 1 < mazeSize.x) mazeMap[destinationPoint.x + 1, destinationPoint.y] = 1;
            else if (destinationPoint.x == mazeSize.x - 1 && destinationPoint.x - 1 >= 0) mazeMap[destinationPoint.x - 1, destinationPoint.y] = 1;
            else if (destinationPoint.y == 0 && destinationPoint.y + 1 < mazeSize.y) mazeMap[destinationPoint.x, destinationPoint.y + 1] = 1;
            else if (destinationPoint.y == mazeSize.y - 1 && destinationPoint.y - 1 >= 0) mazeMap[destinationPoint.x, destinationPoint.y - 1] = 1;
        }
        else
        {
            Debug.LogError("미로의 가장자리에 출구를 생성할 수 있는 유효한 위치를 찾을 수 없습니다. 미로 크기 또는 알고리즘 확인 필요.");
            destinationPoint = new Vector2Int(mazeSize.x - 1, mazeSize.y - 1);
            mazeMap[destinationPoint.x, destinationPoint.y] = 2;
            if (mazeSize.x - 2 >= 0) mazeMap[mazeSize.x - 2, mazeSize.y - 1] = 1;
            if (mazeSize.y - 2 >= 0) mazeMap[mazeSize.x - 1, mazeSize.y - 2] = 1;
        }
    }

    /// <summary>
    /// mazeMap 데이터를 기반으로 Unity Tilemap에 타일을 그립니다.
    /// 여러 종류의 타일을 랜덤으로 선택하여 사용합니다.
    /// </summary>
    void DrawMaze()
    {
        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        for (int x = 0; x < mazeSize.x; x++)
        {
            for (int y = 0; y < mazeSize.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);

                if (mazeMap[x, y] == 0) // 현재 셀이 벽인 경우
                {
                    // ⬇️ [변경] wallTiles 배열에서 랜덤으로 타일 선택
                    wallTilemap.SetTile(tilePos, GetRandomTile(wallTiles));
                }
                else if (mazeMap[x, y] == 1) // 현재 셀이 길(바닥)인 경우
                {
                    // ⬇️ [변경] groundTiles 배열에서 랜덤으로 타일 선택
                    groundTilemap.SetTile(tilePos, GetRandomTile(groundTiles));
                }
                else if (mazeMap[x, y] == 2) // 현재 셀이 목적지인 경우
                {
                    groundTilemap.SetTile(tilePos, destinationTile);
                }
            }
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = groundTilemap.GetCellCenterWorld(new Vector3Int(playerStartPoint.x, playerStartPoint.y, 0));
        }
    }

    /// <summary>
    /// 주어진 TileBase 배열에서 무작위로 하나의 타일을 선택하여 반환합니다.
    /// </summary>
    /// <param name="tiles">타일 배열</param>
    /// <returns>선택된 TileBase</returns>
    private TileBase GetRandomTile(TileBase[] tiles)
    {
        if (tiles != null && tiles.Length > 0)
        {
            return tiles[Random.Range(0, tiles.Length)];
        }
        return null; // 배열이 비어있으면 null 반환 (실제로 이전에 null 체크를 하므로 발생하지 않아야 함)
    }
}