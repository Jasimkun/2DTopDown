using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    public Tilemap groundTilemap;
    public TileBase targetTile;
    public GameObject PlusTimeItemPrefab;
    public GameObject NewLightItemPrefab;

    [Header("제외할 타일들 (생성되지 않도록)")]
    public List<TileBase> excludedTiles;  // 유니티 인스펙터에서 타일 직접 지정

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
            if (tile == null) continue;

            if (tile == targetTile && !excludedTiles.Contains(tile))
            {
                validPositions.Add(pos);
            }
        }

        if (validPositions.Count > 0)
        {
            Vector3Int randomCell = validPositions[Random.Range(0, validPositions.Count)];
            Vector3 spawnPos = groundTilemap.CellToWorld(randomCell) + new Vector3(0.5f, 0.5f, -0.1f);
            Instantiate(PlusTimeItemPrefab, spawnPos, Quaternion.identity);
            //Debug.Log("어딘가에 아이템이 생성된 것 같아.");
        }
        else
        {
            //Debug.LogWarning("적절한 타일 위치를 찾지 못했습니다.");
        }
    }
}
