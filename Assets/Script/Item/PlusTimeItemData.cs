using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewPlusTimeItem", menuName = "Item/PlusTimeItem")]
public class PlusTimeItemData : BaseItemData
{
    public float timeToAdd = 5f;
    public int point;      // 아이템 점수나 값 (필요하면 사용)
    

    // 아이템 사용 시 호출될 공용 메서드
    public override void UseItemEffect()
    {
        if (OnUseItem != null)
        {
            OnUseItem.Invoke();
        }
        else
        {
            //Debug.LogWarning($"아이템 '{itemName}'에 대한 사용 효과가 정의되지 않았습니다.");
        }
    }
}
