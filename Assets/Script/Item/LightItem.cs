using UnityEngine;

// LightItem은 플레이어가 획득할 수 있는 게임 내 아이템을 나타냅니다.
public class LightItem : MonoBehaviour
{
    public string itemName;
    [SerializeField] private BaseItemData data; // ⬇️ LightItemData -> BaseItemData로 변경

    public BaseItemData GetItemData()
    {
        return data;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventorySystem inventory = InventorySystem.Instance;
            if (inventory != null)
            {
                inventory.AddItem(data); // 인벤토리에 LightItemData 추가
            }
            else
            {
                Debug.LogWarning("InventorySystem.Instance를 찾을 수 없습니다. LightItem을 인벤토리에 추가할 수 없습니다.", this);
            }

            // ⬇️ [제거] 획득 시 바로 효과를 적용하는 로직을 제거합니다.
            // 아이템 효과는 이제 인벤토리에서 '사용' 버튼을 눌러야 발동됩니다.
            // if (data != null) { data.ApplyEffect(other.gameObject); }

            Destroy(gameObject);
        }
    }
}
