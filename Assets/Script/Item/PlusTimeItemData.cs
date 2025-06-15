using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewPlusTimeItem", menuName = "Item/PlusTimeItem")]
public class PlusTimeItemData : ScriptableObject
{
    public float timeToAdd = 5f;
    public int point;      // ������ ������ �� (�ʿ��ϸ� ���)
    public string itemName;
    public Sprite icon;

    // --- �߰�: ������ ��� �� ����� �׼� ---
    // �� �׼��� �������� ���� �� ȣ��� �޼��带 �����մϴ�.
    [NonSerialized] // Unity �����Ϳ� ������� �ʵ���
    public Action OnUseItem;

    // ������ ��� �� ȣ��� ���� �޼���
    public void UseItemEffect()
    {
        if (OnUseItem != null)
        {
            OnUseItem.Invoke();
        }
        else
        {
            Debug.LogWarning($"������ '{itemName}'�� ���� ��� ȿ���� ���ǵ��� �ʾҽ��ϴ�.");
        }
    }
}
