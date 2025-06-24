using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    public Tilemap groundTilemap;
    public TileBase targetTile; // 아이템을 스폰할 길 타일 (GroundTile 등)
    public GameObject PlusTimeItemPrefab;
    public GameObject NewLightItemPrefab;

    [Header("제외할 타일들 (생성되지 않도록)")]
    [Tooltip("아이템이 스폰되어서는 안 되는 타일(예: 벽 타일)을 여기에 할당하세요.")]
    public List<TileBase> excludedTiles; // 유니티 인스펙터에서 타일 직접 지정

    [Header("스폰 위치 조정")]
    [Tooltip("아이템이 스폰될 Z축 값입니다. 플레이어나 다른 오브젝트보다 앞에 오도록 설정하세요. (예: -0.1f)")]
    public float itemSpawnZ = -0.1f;
    [Tooltip("아이템 스프라이트 크기에 따른 추가 오프셋입니다. 아이템이 타일 중앙에 완벽히 오도록 조정하세요.")]
    public Vector2 spawnOffset = new Vector2(0.5f, 0.5f);

    void Start()
    {
        StartCoroutine(SpawnItemDelayed());
    }

    IEnumerator SpawnItemDelayed()
    {
        yield return new WaitForSeconds(0.2f);

        List<Vector3Int> validPositions = new List<Vector3Int>();
        BoundsInt bounds = groundTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = groundTilemap.GetTile(pos);
            if (tile != targetTile) continue;

            if (excludedTiles.Contains(tile)) continue;

            validPositions.Add(pos);
        }

        if (validPositions.Count > 0)
        {
            Vector3Int randomCell = validPositions[Random.Range(0, validPositions.Count)];
            Vector3 spawnPos = groundTilemap.CellToWorld(randomCell) + new Vector3(spawnOffset.x, spawnOffset.y, itemSpawnZ);

            // ⬇️ [핵심 수정] itemToSpawn 변수를 if-else 블록 전에 선언하여 스코프를 넓힙니다.
            GameObject itemToSpawn = null; // null로 초기화

            if (Random.Range(0, 2) == 0)
            {
                itemToSpawn = PlusTimeItemPrefab;
            }
            else
            {
                itemToSpawn = NewLightItemPrefab;
            }

            if (itemToSpawn == null)
            {
                Debug.LogError($"선택된 아이템 프리팹이 null입니다. PlusTimeItemPrefab 또는 NewLightItemPrefab이 ItemSpawner Inspector에 할당되었는지 확인해주세요.");
                yield break;
            }

            Instantiate(itemToSpawn, spawnPos, Quaternion.identity);
            Debug.Log($"{itemToSpawn.name}이(가) {spawnPos} 위치에 생성됨. (Cell: {randomCell})");
        }
        else
        {
            Debug.LogWarning("적절한 타일 위치(targetTile)를 찾지 못했습니다. 아이템이 스폰되지 않았습니다. targetTile과 excludedTiles 설정, 그리고 맵에 targetTile이 충분히 있는지 확인하세요.");
        }
    }
}
