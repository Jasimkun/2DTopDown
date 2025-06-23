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
    public List<TileBase> excludedTiles; // 유니티 인스펙터에서 타일 직접 지정

    void Start()
    {
        // 지연 시간 후 아이템 스폰 코루틴 시작
        StartCoroutine(SpawnItemDelayed());
    }

    IEnumerator SpawnItemDelayed()
    {
        // 미로 생성이 완료될 시간을 기다립니다. (예: 0.2초)
        // 미로 생성 로직에 따라 이 시간을 조정해야 할 수 있습니다.
        yield return new WaitForSeconds(0.2f);

        List<Vector3Int> validPositions = new List<Vector3Int>();
        // groundTilemap의 모든 셀 범위를 가져옵니다.
        BoundsInt bounds = groundTilemap.cellBounds;

        // 모든 셀을 순회하며 유효한 스폰 위치를 찾습니다.
        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = groundTilemap.GetTile(pos);
            // 타일이 없거나 제외된 타일 목록에 포함되어 있으면 건너뜁니다.
            if (tile == null) continue;
            if (excludedTiles.Contains(tile)) continue; // excludedTiles에 포함된 타일은 제외

            // 타일이 targetTile과 일치하면 유효한 위치로 추가합니다.
            // targetTile은 보통 미로의 '길' 타일이어야 합니다.
            if (tile == targetTile)
            {
                validPositions.Add(pos);
            }
        }

        // 유효한 스폰 위치가 하나 이상 있다면 아이템을 스폰합니다.
        if (validPositions.Count > 0)
        {
            // 유효한 위치 중에서 랜덤으로 하나의 셀을 선택합니다.
            Vector3Int randomCell = validPositions[Random.Range(0, validPositions.Count)];
            // 선택된 셀의 월드 좌표 중앙을 계산하고 z 값을 조정하여 아이템이 올바른 깊이에 위치하도록 합니다.
            Vector3 spawnPos = groundTilemap.CellToWorld(randomCell) + new Vector3(0.5f, 0.5f, -0.1f); // z = -0.1f는 플레이어 위에 보이도록 조정

            // ⬇️ [수정] PlusTimeItemPrefab과 NewLightItemPrefab 중 랜덤으로 선택하여 스폰합니다.
            GameObject itemToSpawn;
            if (Random.Range(0, 2) == 0) // 50% 확률로 PlusTimeItem
            {
                itemToSpawn = PlusTimeItemPrefab;
            }
            else // 50% 확률로 NewLightItem
            {
                itemToSpawn = NewLightItemPrefab;
            }

            // 선택된 아이템 프리팹을 스폰합니다.
            Instantiate(itemToSpawn, spawnPos, Quaternion.identity);
            Debug.Log($"{itemToSpawn.name}이(가) 어딘가에 생성된 것 같아.");
        }
        else
        {
            // 유효한 스폰 위치를 찾지 못했을 경우 경고 로그를 출력합니다.
            //Debug.LogWarning("적절한 타일 위치를 찾지 못했습니다. 아이템이 스폰되지 않았습니다.");
        }
    }
}
