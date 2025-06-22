using UnityEngine;

// LightItem은 플레이어가 획득할 수 있는 게임 내 아이템을 나타냅니다.
// 이 아이템을 획득하면 LightItemData에 정의된 스케일 효과가 플레이어에게 적용됩니다.
public class LightItem : MonoBehaviour
{
    // 아이템의 이름을 표시합니다. (선택 사항)
    public string itemName;

    // LightItemData ScriptableObject에 대한 참조입니다.
    // BaseItemData를 상속받는 모든 아이템 데이터를 받을 수 있도록 변경
    [SerializeField] private BaseItemData data; // ⬇️ PlusTimeItemData -> BaseItemData로 변경

    /// <summary>
    /// 이 아이템과 연결된 BaseItemData를 가져옵니다.
    /// </summary>
    /// <returns>이 아이템과 연결된 BaseItemData입니다.</returns>
    public BaseItemData GetItemData() // ⬇️ 반환 타입 변경
    {
        return data;
    }

    /// <summary>
    /// 다른 2D 콜라이더와 충돌했을 때 호출됩니다.
    /// 플레이어와 충돌하면 아이템 효과를 적용하고 인벤토리에 추가한 후 아이템을 파괴합니다.
    /// </summary>
    /// <param name="other">충돌한 다른 콜라이더입니다.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트의 태그가 "Player"인지 확인합니다.
        if (other.CompareTag("Player"))
        {
            // 인벤토리 시스템을 찾아 아이템 데이터를 추가합니다.
            // 이 InventorySystem은 씬에 하나만 존재하고 DontDestroyOnLoad가 적용되었다고 가정합니다.
            // InventorySystem.Instance는 이미 싱글톤 인스턴스에 접근할 수 있습니다.
            InventorySystem inventory = InventorySystem.Instance; // ⬇️ FindObjectOfType 대신 Instance 사용 (더 효율적)
            if (inventory != null)
            {
                inventory.AddItem(data); // ⬇️ BaseItemData 타입의 data 전달
            }
            else
            {
                Debug.LogWarning("InventorySystem.Instance를 찾을 수 없습니다. LightItem을 인벤토리에 추가할 수 없습니다.", this);
            }

            // 아이템을 획득했으므로 현재 아이템 GameObject를 파괴합니다.
            Destroy(gameObject);
        }
    }
}
