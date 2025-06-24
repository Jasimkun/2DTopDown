using UnityEngine;
using static InventorySystem;

// PlusTimeItem은 플레이어가 획득할 수 있는 게임 내 아이템을 나타냅니다.
public class PlusTimeItem : MonoBehaviour
{
    public string itemName;

    // PlusTimeItemData ScriptableObject에 대한 참조입니다.
    // Inspector에서 PlusTimeItemData 에셋을 할당해야 합니다.
    [SerializeField] private PlusTimeItemData data; // ⬇️ [변경] BaseItemData 대신 PlusTimeItemData 직접 참조

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventorySystem inventory = FindObjectOfType<InventorySystem>();
            if (inventory != null)
            {
                InventoryItem newItem = new InventoryItem
                {
                    itemName = data.itemName,
                    count = 1,
                    usedInCurrentScene = false
                };
                //inventory.AddItem(newItem); // ⬇️ [변경] AddItem에 PlusTimeItemData 직접 전달
            }
            else
            {
                Debug.LogWarning("InventorySystem을 찾을 수 없습니다. PlusTimeItem을 인벤토리에 추가할 수 없습니다.", this);
            }

            Destroy(gameObject);
        }
    }
}
