using UnityEngine;

// PlusTimeItem은 플레이어가 획득할 수 있는 게임 내 아이템을 나타냅니다.
// 이 아이템을 획득하면 InventorySystem을 통해 인벤토리에 추가됩니다.
public class PlusTimeItem : MonoBehaviour
{
    // 아이템의 이름을 표시합니다. (선택 사항)
    public string itemName;

    // BaseItemData ScriptableObject에 대한 참조입니다.
    // 이 데이터를 통해 아이템의 공통 속성을 가져오고, 인벤토리에 전달합니다.
    // Inspector에서 PlusTimeItemData 에셋을 할당해야 합니다.
    [SerializeField] private BaseItemData data;

    // ⬇️ [제거] GetTimeToAdd() 메서드를 제거합니다.
    // 아이템의 실제 효과(시간 추가)는 PlusTimeItemData의 UseItemEffect 메서드에서 처리됩니다.
    // public float GetTimeToAdd()
    // {
    //     return data.timeToAdd; // BaseItemData에는 timeToAdd가 없으므로 오류 발생
    // }

    /// <summary>
    /// 다른 2D 콜라이더와 충돌했을 때 호출됩니다.
    /// 플레이어와 충돌하면 아이템을 인벤토리에 추가한 후 아이템 GameObject를 파괴합니다.
    /// </summary>
    /// <param name="other">충돌한 다른 콜라이더입니다.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트의 태그가 "Player"인지 확인합니다.
        if (other.CompareTag("Player"))
        {
            // 인벤토리 시스템의 싱글톤 인스턴스를 가져옵니다.
            InventorySystem inventory = InventorySystem.Instance;
            if (inventory != null)
            {
                // BaseItemData 타입의 'data'를 인벤토리 시스템에 전달하여 아이템을 추가합니다.
                inventory.AddItem(data);
            }
            else
            {
                Debug.LogWarning("InventorySystem.Instance를 찾을 수 없습니다. PlusTimeItem을 인벤토리에 추가할 수 없습니다.", this);
            }

            // 아이템을 획득했으므로 현재 아이템 GameObject를 파괴합니다.
            Destroy(gameObject);
        }
    }
}
