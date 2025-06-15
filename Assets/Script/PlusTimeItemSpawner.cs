using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class PlusTimeItemSpawner : MonoBehaviour
{
    public Tilemap groundTilemap;
    public TileBase targetTile;
    public GameObject PlusTimeItemPrefab;

    [Header("������ Ÿ�ϵ� (�������� �ʵ���)")]
    public List<TileBase> excludedTiles;  // ����Ƽ �ν����Ϳ��� Ÿ�� ���� ����

    void Start()
    {
        StartCoroutine(SpawnItemDelayed());
    }

    IEnumerator SpawnItemDelayed()
    {
        yield return new WaitForSeconds(0.5f);

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
            Debug.Log("������ ���� ��ġ: " + spawnPos);
        }
        else
        {
            Debug.LogWarning("������ Ÿ�� ��ġ�� ã�� ���߽��ϴ�.");
        }
    }
}
