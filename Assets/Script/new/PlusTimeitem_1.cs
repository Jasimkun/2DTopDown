using UnityEngine;

public class PlusTimeItem_1 : MonoBehaviour
{
    // �� ������ ������Ʈ�� � BaseItemData_1�� �ش��ϴ��� Inspector���� �����մϴ�.
    // ���� ���, Project â�� Assets/Resources/ItemData/�ð�������_1.asset�� �巡���Ͽ� �����մϴ�.
    public BaseItemData_1 itemDataToAcquire;

    void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾� �±� Ȯ�� (�÷��̾� GameObject�� "Player" �±װ� �Ҵ�Ǿ� �־�� �մϴ�.)
        if (other.CompareTag("Player"))
        {
            if (InventorySystem_1.Instance != null)
            {
                // InventorySystem_1�� AddItem �޼��带 ȣ���Ͽ� ������ ȹ���� �õ��մϴ�.
                InventorySystem_1.Instance.AddItem(itemDataToAcquire);

                // ������ ȹ�� �� �� ���� ������Ʈ�� �ı��մϴ�.
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("[PlusTimeItem_1] InventorySystem_1 �ν��Ͻ��� ã�� �� �����ϴ�. �������� ȹ���� �� �����ϴ�.");
            }
        }
    }
}