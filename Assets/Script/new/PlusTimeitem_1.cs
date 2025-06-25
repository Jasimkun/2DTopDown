using UnityEngine;

public class PlusTimeItem_1 : MonoBehaviour
{
    // 이 아이템 오브젝트가 어떤 BaseItemData_1에 해당하는지 Inspector에서 연결합니다.
    // 예를 들어, Project 창의 Assets/Resources/ItemData/시간아이템_1.asset을 드래그하여 연결합니다.
    public BaseItemData_1 itemDataToAcquire;

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그 확인 (플레이어 GameObject에 "Player" 태그가 할당되어 있어야 합니다.)
        if (other.CompareTag("Player"))
        {
            if (InventorySystem_1.Instance != null)
            {
                // InventorySystem_1의 AddItem 메서드를 호출하여 아이템 획득을 시도합니다.
                InventorySystem_1.Instance.AddItem(itemDataToAcquire);

                // 아이템 획득 후 이 게임 오브젝트는 파괴합니다.
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("[PlusTimeItem_1] InventorySystem_1 인스턴스를 찾을 수 없습니다. 아이템을 획득할 수 없습니다.");
            }
        }
    }
}