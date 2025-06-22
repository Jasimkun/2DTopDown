using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewPlusTimeItem", menuName = "Item/PlusTimeItem")]
public class PlusTimeItemData : BaseItemData
{
    public float timeToAdd = 5f;
    public int point;      // ������ ������ �� (�ʿ��ϸ� ���)
    

    // ������ ��� �� ȣ��� ���� �޼���
    public override void UseItemEffect()
    {
        if (OnUseItem != null)
        {
            OnUseItem.Invoke();
        }
        else
        {
            //Debug.LogWarning($"������ '{itemName}'�� ���� ��� ȿ���� ���ǵ��� �ʾҽ��ϴ�.");
        }
    }
}
