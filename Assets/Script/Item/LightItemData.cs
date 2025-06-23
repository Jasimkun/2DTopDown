using UnityEngine;

[CreateAssetMenu(fileName = "NewLightItem", menuName = "Item/LightItem")]
public class LightItemData : BaseItemData
{
    // ⬇️ [추가] 레벨 0에서의 기본 스케일 배율
    public Vector3 baseTargetScaleMultiplier = new Vector3(1.5f, 1.5f, 1.5f);
    // ⬇️ [추가] 레벨 1 증가당 추가되는 스케일 배율
    public float scaleMultiplierPerLevel = 0.1f;

    public float scaleHoldDuration = 5f; // (기존대로 유지)
    public int point; // (기존대로 유지)

    /// <summary>
    /// 주어진 레벨에 해당하는 실제 스케일 배율 값을 계산하여 반환합니다.
    /// </summary>
    /// <param name="level">계산할 업그레이드 레벨입니다.</param>
    /// <returns>해당 레벨에서의 최종 스케일 배율 값입니다.</returns>
    public Vector3 GetEffectiveTargetScaleMultiplier(int level)
    {
        // 각 축(X, Y, Z)에 대해 레벨에 따라 스케일 증가량을 더합니다.
        return baseTargetScaleMultiplier + (Vector3.one * (level * scaleMultiplierPerLevel));
    }

    public override void UseItemEffect(GameObject user = null)
    {
        if (user != null)
        {
            ScaleEffectHandler handler = user.GetComponent<ScaleEffectHandler>();
            if (handler == null)
            {
                handler = user.AddComponent<ScaleEffectHandler>();
            }
            // ScaleEffectHandler로 LightItemData 자체를 넘겨,
            // 핸들러 내부에서 ShopManager로부터 현재 레벨을 조회하도록 합니다.
            handler.StartScaleEffect(this);
            Debug.Log($"[LightItemData] '{itemName}' 아이템 효과 사용! 스케일 효과 적용. (Level will be determined by ShopManager)");
        }
        else
        {
            Debug.LogWarning($"[LightItemData] '{itemName}' 아이템 효과를 적용할 대상(user)이 없습니다.");
        }
        // LightItemData는 OnUseItem 델리게이트를 직접 사용하지 않을 수 있으므로, 주석 처리합니다.
        // OnUseItem?.Invoke();
    }
}
