using System.Collections;
using System.Collections.Generic; // List<T> 사용을 위해 필요합니다.
using UnityEngine;
using UnityEngine.Tilemaps;

// 이 스크립트는 이진 트리 알고리즘을 사용하여 미로를 생성하고,
// 생성된 미로를 Unity Tilemap에 그립니다.
// 플레이어의 시작 지점과 목적지(출구)를 설정하고, 목적지 위에 트리거를 생성합니다.
public class BinaryTreeMazeGenerator : MonoBehaviour
{
    [Header("미로 설정")]
    [Tooltip("미로의 크기 (가로, 세로). 홀수여야 합니다. 짝수 입력 시 자동으로 홀수로 조정됩니다.")]
    public Vector2Int mazeSize = new Vector2Int(21, 21);
    [Tooltip("플레이어 시작 지점으로부터 목적지까지의 최소 맨해튼 거리 (벽 포함).")]
    public int minDistanceToStart = 5;

    [Header("타일맵 참조")]
    [Tooltip("길(바닥) 타일이 그려질 Tilemap.")]
    public Tilemap groundTilemap;
    [Tooltip("벽 타일이 그려질 Tilemap.")]
    public Tilemap wallTilemap;

    [Header("타일 참조")]
    [Tooltip("여러 종류의 벽 타일을 담을 배열. Unity 에디터에서 할당해주세요.")]
    public TileBase[] wallTiles;    // 벽 타일들의 배열
    [Tooltip("여러 종류의 길(바닥) 타일을 담을 배열. Unity 에디터에서 할당해주세요.")]
    public TileBase[] groundTiles;  // 길(바닥) 타일들의 배열
    [Tooltip("목적지를 나타낼 단일 타일. Unity 에디터에서 할당해주세요.")]
    public TileBase destinationTile; // 목적지 타일 (단일)

    [Header("목적지 설정")]
    [Tooltip("목적지 타일 위에 생성될 트리거 오브젝트의 프리팹. 이 프리팹에는 Collider2D/3D와 DestinationTrigger 스크립트가 있어야 합니다.")]
    public GameObject destinationTriggerPrefab; // 목적지 타일에 놓일 프리팹

    private int[,] mazeMap; // 미로의 구조를 저장할 2D 배열 (0: 벽, 1: 길, 2: 목적지)

    [HideInInspector] // Unity 에디터 Inspector에서 숨김
    public Vector2Int destinationPoint; // 목적지의 좌표
    [HideInInspector] // Unity 에디터 Inspector에서 숨김
    public Vector2Int playerStartPoint; // 플레이어 시작 지점의 좌표

    void Start()
    {
        // 타일 배열이 비어 있는지 확인하여 오류 방지 및 경고 메시지 출력
        if (wallTiles == null || wallTiles.Length == 0)
        {
            Debug.LogError("Wall Tiles 배열이 비어 있습니다. 최소 하나 이상의 벽 타일을 할당해야 합니다.", this);
            return; // 스크립트 실행 중단
        }
        if (groundTiles == null || groundTiles.Length == 0)
        {
            Debug.LogError("Ground Tiles 배열이 비어 있습니다. 최소 하나 이상의 길 타일을 할당해야 합니다.", this);
            return; // 스크립트 실행 중단
        }
        if (destinationTile == null)
        {
            Debug.LogWarning("Destination Tile이 할당되지 않았습니다. 목적지가 일반 길 타일로 표시될 수 있습니다.", this);
            // destinationTile이 없으면 기본 groundTiles 중 하나를 사용하도록 폴백 설정
            destinationTile = groundTiles[0];
        }
        // 목적지 트리거 프리팹이 할당되었는지 확인
        if (destinationTriggerPrefab == null)
        {
            Debug.LogError("Destination Trigger Prefab이 할당되지 않았습니다. 게임이 제대로 작동하지 않을 수 있습니다.", this);
            return; // 스크립트 실행 중단
        }

        GenerateMaze(); // 미로 맵 데이터 생성
        DrawMaze();     // 생성된 미로 맵을 기반으로 타일 그리기
    }

    /// <summary>
    /// 이진 트리 알고리즘을 사용하여 미로 맵 데이터를 생성합니다.
    /// mazeMap 배열에 벽(0), 길(1), 목적지(2)를 설정합니다.
    /// 플레이어 시작 지점을 미로 중앙으로 설정하고, 가장자리 벽에 목적지를 랜덤하게 생성합니다.
    /// </summary>
    void GenerateMaze()
    {
        // 미로 크기를 홀수로 강제 조정 (이진 트리 알고리즘은 홀수 크기에서 잘 작동)
        if (mazeSize.x % 2 == 0) mazeSize.x++;
        if (mazeSize.y % 2 == 0) mazeSize.y++;

        // mazeMap 초기화: 모든 셀을 벽(0)으로 설정
        mazeMap = new int[mazeSize.x, mazeSize.y];
        for (int x = 0; x < mazeSize.x; x++)
        {
            for (int y = 0; y < mazeSize.y; y++)
            {
                mazeMap[x, y] = 0; // 0 = 벽 (Wall)
            }
        }

        // 이진 트리 알고리즘 적용: 2칸 간격으로 셀을 순회하며 길(1)을 뚫습니다.
        for (int x = 1; x < mazeSize.x - 1; x += 2)
        {
            for (int y = 1; y < mazeSize.y - 1; y += 2)
            {
                mazeMap[x, y] = 1; // 현재 셀을 길(Path)로 만듭니다.

                // 미로의 가장자리 처리: 오른쪽 또는 위쪽으로만 뚫을 수 있을 때
                if (x == mazeSize.x - 2) // 오른쪽 가장자리에 도달했을 경우
                {
                    if (y + 2 < mazeSize.y) mazeMap[x, y + 1] = 1; // 위쪽으로만 길을 뚫습니다.
                    continue;
                }
                if (y == mazeSize.y - 2) // 위쪽 가장자리에 도달했을 경우
                {
                    if (x + 2 < mazeSize.x) mazeMap[x + 1, y] = 1; // 오른쪽으로만 길을 뚫습니다.
                    continue;
                }

                // 일반적인 경우: 랜덤하게 오른쪽 또는 위쪽 중 한 방향으로 길을 뚫습니다.
                if (Random.Range(0, 2) == 0) // 0 = 오른쪽
                {
                    if (x + 1 < mazeSize.x) mazeMap[x + 1, y] = 1; // 오른쪽으로 길을 뚫습니다.
                }
                else // 1 = 위쪽
                {
                    if (y + 1 < mazeSize.y) mazeMap[x, y + 1] = 1; // 위쪽으로 길을 뚫습니다.
                }
            }
        }

        // --- 플레이어 시작 지점을 미로 중앙으로 설정 ---
        playerStartPoint = new Vector2Int(mazeSize.x / 2, mazeSize.y / 2);

        // 플레이어 시작 지점과 그 주변 3x3 영역을 길(1)로 만들어 플레이어가 갇히지 않도록 합니다.
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int currentX = playerStartPoint.x + dx;
                int currentY = playerStartPoint.y + dy;

                // 유효한 미로 범위 내에 있는지 확인
                if (currentX >= 0 && currentX < mazeSize.x &&
                    currentY >= 0 && currentY < mazeSize.y)
                {
                    mazeMap[currentX, currentY] = 1; // 길로 설정
                }
            }
        }

        // --- BFS (Breadth-First Search)를 이용한 도달 가능한 길 찾기 ---
        // 플레이어 시작 지점에서 도달할 수 있는 모든 길 셀을 찾습니다.
        // 이는 목적지를 플레이어 시작 지점에서 도달 가능한 곳에 두기 위함입니다.
        List<Vector2Int> reachablePathCells = new List<Vector2Int>(); // 도달 가능한 길 셀 목록
        Queue<Vector2Int> queue = new Queue<Vector2Int>(); // BFS 탐색을 위한 큐
        bool[,] visited = new bool[mazeSize.x, mazeSize.y]; // 방문 여부 확인 배열

        queue.Enqueue(playerStartPoint); // 시작 지점을 큐에 추가
        visited[playerStartPoint.x, playerStartPoint.y] = true; // 시작 지점 방문 처리
        reachablePathCells.Add(playerStartPoint); // 도달 가능한 목록에 시작 지점 추가

        // 상하좌우 이동을 위한 방향 벡터
        Vector2Int[] directions = {
            new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0)
        };

        // 큐가 비어있지 않은 동안 BFS 계속
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue(); // 현재 셀 가져오기
            foreach (Vector2Int dir in directions) // 각 방향으로 이웃 셀 확인
            {
                Vector2Int neighbor = current + dir; // 이웃 셀 좌표
                // 이웃 셀이 미로 범위 내에 있고, 아직 방문하지 않았으며, 길(1)인 경우
                if (neighbor.x >= 0 && neighbor.x < mazeSize.x &&
                    neighbor.y >= 0 && neighbor.y < mazeSize.y &&
                    !visited[neighbor.x, neighbor.y] && mazeMap[neighbor.x, neighbor.y] == 1)
                {
                    visited[neighbor.x, neighbor.y] = true; // 방문 처리
                    queue.Enqueue(neighbor); // 큐에 추가
                    reachablePathCells.Add(neighbor); // 도달 가능한 목록에 추가
                }
            }
        }

        // --- 가장 바깥쪽 벽을 뚫어 랜덤 목적지(출구) 생성 로직 ---
        List<Vector2Int> potentialExitPoints = new List<Vector2Int>(); // 잠재적인 출구 후보 지점들

        // 상단 경계 (y = mazeSize.y - 1)에서 출구 후보 찾기
        for (int x = 1; x < mazeSize.x - 1; x++)
        {
            // 한 칸 안쪽이 길이면 (출구를 뚫을 수 있는 곳)
            if (mazeMap[x, mazeSize.y - 2] == 1)
            {
                potentialExitPoints.Add(new Vector2Int(x, mazeSize.y - 1));
            }
        }
        // 하단 경계 (y = 0)에서 출구 후보 찾기
        for (int x = 1; x < mazeSize.x - 1; x++)
        {
            if (mazeMap[x, 1] == 1)
            {
                potentialExitPoints.Add(new Vector2Int(x, 0));
            }
        }
        // 좌측 경계 (x = 0)에서 출구 후보 찾기
        for (int y = 1; y < mazeSize.y - 1; y++)
        {
            if (mazeMap[1, y] == 1)
            {
                potentialExitPoints.Add(new Vector2Int(0, y));
            }
        }
        // 우측 경계 (x = mazeSize.x - 1)에서 출구 후보 찾기
        for (int y = 1; y < mazeSize.y - 1; y++)
        {
            if (mazeMap[mazeSize.x - 2, y] == 1)
            {
                potentialExitPoints.Add(new Vector2Int(mazeSize.x - 1, y));
            }
        }

        // 유효한 출구 후보 지점이 있을 경우
        if (potentialExitPoints.Count > 0)
        {
            Vector2Int chosenExit = new Vector2Int(-1, -1); // 선택된 출구 지점
            int attempts = 0; // 시도 횟수
            const int maxAttempts = 200; // 최대 시도 횟수

            // 무작위로 출구를 선택하고, 플레이어 시작 지점으로부터 충분히 멀리 떨어져 있는지 확인
            while (attempts < maxAttempts)
            {
                int randomIndex = Random.Range(0, potentialExitPoints.Count);
                Vector2Int tempExit = potentialExitPoints[randomIndex];

                // 출구 지점의 바로 안쪽 셀 (길이 이어져 있는지 확인하기 위함)
                Vector2Int innerCell = tempExit;
                if (tempExit.x == 0) innerCell.x = 1;
                else if (tempExit.x == mazeSize.x - 1) innerCell.x = mazeSize.x - 2;
                else if (tempExit.y == 0) innerCell.y = 1;
                else if (tempExit.y == mazeSize.y - 1) innerCell.y = mazeSize.y - 2;

                // 내부 셀이 유효한 범위 내에 있는지 확인
                if (innerCell.x < 0 || innerCell.x >= mazeSize.x ||
                    innerCell.y < 0 || innerCell.y >= mazeSize.y)
                {
                    attempts++;
                    continue;
                }

                // 내부 셀이 플레이어 시작 지점에서 도달 가능하고, 최소 거리를 충족하는지 확인
                if (reachablePathCells.Contains(innerCell))
                {
                    int manhattanDistance = Mathf.Abs(innerCell.x - playerStartPoint.x) + Mathf.Abs(innerCell.y - playerStartPoint.y);
                    if (manhattanDistance >= minDistanceToStart)
                    {
                        chosenExit = tempExit; // 적합한 출구 선택
                        break; // 반복 종료
                    }
                }
                attempts++; // 시도 횟수 증가
            }

            // 적합한 랜덤 출구를 찾지 못했을 경우, fallback으로 우측 하단에 출구 생성
            if (chosenExit.x == -1)
            {
                Debug.LogWarning("적합한 랜덤 출구를 찾지 못했습니다. 우측 하단 벽에 출구를 생성합니다.");
                chosenExit = new Vector2Int(mazeSize.x - 1, mazeSize.y - 1); // 우측 하단 코너
                // 해당 코너와 인접한 내부 길을 뚫어줌 (미로 크기에 따라)
                if (mazeSize.x - 2 >= 0 && mazeSize.y - 1 >= 0) mazeMap[mazeSize.x - 2, mazeSize.y - 1] = 1;
                if (mazeSize.x - 1 >= 0 && mazeSize.y - 2 >= 0) mazeMap[mazeSize.x - 1, mazeSize.y - 2] = 1;
            }

            destinationPoint = chosenExit; // 최종 목적지 설정
            mazeMap[destinationPoint.x, destinationPoint.y] = 2; // mazeMap에 목적지(2) 표시

            // 목적지와 인접한 내부 타일도 길(1)로 만들어 출구가 확실히 연결되도록 합니다.
            if (destinationPoint.x == 0 && destinationPoint.x + 1 < mazeSize.x) mazeMap[destinationPoint.x + 1, destinationPoint.y] = 1;
            else if (destinationPoint.x == mazeSize.x - 1 && destinationPoint.x - 1 >= 0) mazeMap[destinationPoint.x - 1, destinationPoint.y] = 1;
            else if (destinationPoint.y == 0 && destinationPoint.y + 1 < mazeSize.y) mazeMap[destinationPoint.x, destinationPoint.y + 1] = 1;
            else if (destinationPoint.y == mazeSize.y - 1 && destinationPoint.y - 1 >= 0) mazeMap[destinationPoint.x, destinationPoint.y - 1] = 1;
        }
        else // 출구 후보 지점이 전혀 없는 경우 (심각한 오류)
        {
            Debug.LogError("미로의 가장자리에 출구를 생성할 수 있는 유효한 위치를 찾을 수 없습니다. 미로 크기 또는 알고리즘 확인 필요. 기본 목적지를 우측 하단으로 설정합니다.");
            destinationPoint = new Vector2Int(mazeSize.x - 1, mazeSize.y - 1);
            mazeMap[destinationPoint.x, destinationPoint.y] = 2;
            if (mazeSize.x - 2 >= 0) mazeMap[mazeSize.x - 2, mazeSize.y - 1] = 1;
            if (mazeSize.y - 2 >= 0) mazeMap[mazeSize.x - 1, mazeSize.y - 2] = 1;
        }
    }

    /// <summary>
    /// mazeMap 데이터를 기반으로 Unity Tilemap에 타일을 그립니다.
    /// 여러 종류의 타일을 랜덤으로 선택하여 사용하며, 목적지 위에 DestinationTrigger 프리팹을 생성합니다.
    /// </summary>
    void DrawMaze()
    {
        groundTilemap.ClearAllTiles(); // 기존 길 타일 모두 지우기
        wallTilemap.ClearAllTiles();   // 기존 벽 타일 모두 지우기

        // mazeMap 데이터를 순회하며 타일을 그립니다.
        for (int x = 0; x < mazeSize.x; x++)
        {
            for (int y = 0; y < mazeSize.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0); // 현재 타일의 타일맵 좌표

                if (mazeMap[x, y] == 0) // 현재 셀이 벽인 경우
                {
                    // wallTiles 배열에서 랜덤으로 타일을 선택하여 벽 타일맵에 설정
                    wallTilemap.SetTile(tilePos, GetRandomTile(wallTiles));
                }
                else if (mazeMap[x, y] == 1) // 현재 셀이 길(바닥)인 경우
                {
                    // groundTiles 배열에서 랜덤으로 타일을 선택하여 길 타일맵에 설정
                    groundTilemap.SetTile(tilePos, GetRandomTile(groundTiles));
                }
                else if (mazeMap[x, y] == 2) // 현재 셀이 목적지인 경우
                {
                    // 목적지 타일을 길 타일맵에 설정
                    groundTilemap.SetTile(tilePos, destinationTile);

                    // ⬇️ [변경] 목적지 위에 DestinationTrigger 프리팹을 생성합니다.
                    // 목적지 타일의 월드 중앙 좌표를 계산
                    Vector3 worldPos = groundTilemap.GetCellCenterWorld(tilePos);
                    // 프리팹 인스턴스화
                    Instantiate(destinationTriggerPrefab, worldPos, Quaternion.identity);
                    Debug.Log($"[BinaryTreeMazeGenerator] 목적지 트리거를 ({worldPos.x:F2}, {worldPos.y:F2})에 생성했습니다.");
                }
            }
        }

        // "Player" 태그를 가진 GameObject를 찾아 시작 지점으로 이동시킵니다.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 플레이어의 위치를 시작 지점 타일의 중앙으로 설정
            player.transform.position = groundTilemap.GetCellCenterWorld(new Vector3Int(playerStartPoint.x, playerStartPoint.y, 0));
            Debug.Log($"[BinaryTreeMazeGenerator] 플레이어 시작 지점: {playerStartPoint} (월드 좌표: {player.transform.position.x:F2}, {player.transform.position.y:F2})");
        }
        else
        {
            Debug.LogWarning("[BinaryTreeMazeGenerator] 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다. 플레이어 시작 위치가 설정되지 않았습니다.");
        }
    }

    /// <summary>
    /// 주어진 TileBase 배열에서 무작위로 하나의 타일을 선택하여 반환합니다.
    /// 배열이 비어있거나 null이면 null을 반환합니다.
    /// </summary>
    /// <param name="tiles">타일 배열</param>
    /// <returns>선택된 TileBase</returns>
    private TileBase GetRandomTile(TileBase[] tiles)
    {
        if (tiles != null && tiles.Length > 0)
        {
            return tiles[Random.Range(0, tiles.Length)]; // 배열 내에서 랜덤 인덱스 선택
        }
        return null; // 배열이 비어있거나 null인 경우
    }
}
