using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewPlusTimeItem", menuName = "Item/PlusTimeItem")]
public class PlusTimeItemData : BaseItemData
{
    // ⬇️ [추가] 레벨 0에서의 기본 시간 보너스
    public float baseTimeToAdd = 5f;
    // ⬇️ [추가] 레벨 1 증가당 추가되는 시간 보너스
    public float timeToAddPerLevel = 1f;

    public int point; // (기존대로 유지)

    /// <summary>
    /// 주어진 레벨에 해당하는 실제 시간 보너스 값을 계산하여 반환합니다.
    /// </summary>
    /// <param name="level">계산할 업그레이드 레벨입니다.</param>
    /// <returns>해당 레벨에서의 최종 시간 보너스 값입니다.</returns>
    public float GetEffectiveTimeToAdd(int level)
    {
        return baseTimeToAdd + (level * timeToAddPerLevel);
    }

    public override void UseItemEffect(GameObject user = null)
    {
        // InventorySystem에서 이 델리게이트를 통해 GameTimer에 시간을 추가하는 로직을 연결할 것입니다.
        OnUseItem?.Invoke();
        Debug.Log($"[PlusTimeItemData] '{itemName}' 아이템 효과 사용! (Level will be determined by ShopManager)");
    }
}
