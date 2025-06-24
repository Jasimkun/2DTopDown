using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgradeDefinition", menuName = "Shop/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    [Header("업그레이드 대상 아이템")]
    [Tooltip("이 업그레이드가 적용될 BaseItemData 에셋을 할당하세요. ShopManager의 upgradeableItemDatas 목록에 포함되어야 합니다.")]
    public BaseItemData targetItemData;

    [Header("업그레이드 레벨별 비용")]
    [Tooltip("업그레이드 레벨별 필요한 골드 비용 목록입니다. 리스트의 인덱스 0은 '레벨 0에서 레벨 1로 업그레이드하는 비용'입니다. (Max Level까지)")]
    public List<int> costs;

    [Tooltip("이 업그레이드의 최대 레벨입니다. (레벨 0 포함. 예: Max Level이 3이면 레벨 0, 1, 2, 3까지 가능합니다)")]
    public int maxLevel = 3;

    [Header("UI 표시 설정")]
    [Tooltip("상점 UI에 표시될 이 업그레이드의 제목입니다 (예: '시간 추가 아이템 효율 증가').")]
    public string upgradeTitle;

    [Tooltip("업그레이드 효과를 설명하는 텍스트 형식입니다. {0}은 현재 레벨의 값, {1}은 다음 레벨의 값입니다.")]
    public string effectDescriptionFormat;

    /// <summary>
    /// 특정 레벨로 업그레이드하는 데 필요한 골드 비용을 반환합니다.
    /// </summary>
    /// <param name="targetLevel">도달하려는 업그레이드 레벨입니다. (예: 레벨 1로 가기 위한 비용은 GetCostForLevel(1))</param>
    /// <returns>해당 레벨로의 업그레이드 비용 또는 -1 (비용이 정의되지 않은 경우).</returns>
    public int GetCostForLevel(int targetLevel)
    {
        // costs 리스트는 레벨 1의 비용부터 시작하므로 인덱스는 targetLevel - 1
        if (targetLevel > 0 && targetLevel <= costs.Count)
        {
            return costs[targetLevel - 1];
        }
        return -1; // 유효하지 않은 레벨
    }

    /// <summary>
    /// 현재 레벨에서의 아이템 효과 값을 문자열로 반환합니다.
    /// PlusTimeItemData와 LightItemData의 새로운 필드를 참조합니다.
    /// </summary>
    /// <param name="currentLevel">아이템의 현재 업그레이드 레벨입니다.</param>
    /// <returns>아이템의 현재 효과를 나타내는 문자열입니다.</returns>
    public string GetCurrentEffectValueString(int currentLevel)
    {
        if (targetItemData is PlusTimeItemData plusTimeData)
        {
            // ⬇️ [수정] timeToAdd 대신 GetEffectiveTimeToAdd 메서드 호출
            return $"{plusTimeData.GetEffectiveTimeToAdd(currentLevel):F0}초"; // 소수점 없이 표시
        }
        if (targetItemData is LightItemData lightData)
        {
            // ⬇️ [수정] targetScaleMultiplier 대신 GetEffectiveTargetScaleMultiplier 메서드 호출
            return $"{lightData.GetEffectiveTargetScaleMultiplier(currentLevel).x:F1}배"; // 소수점 첫째 자리까지 표시
        }
        return "알 수 없음"; // 정의되지 않은 BaseItemData 타입
    }

    /// <summary>
    /// 다음 레벨에서의 아이템 효과 값을 문자열로 반환합니다.
    /// PlusTimeItemData와 LightItemData의 새로운 필드를 참조합니다.
    /// </summary>
    /// <param name="currentLevel">아이템의 현재 업그레이드 레벨입니다.</param>
    /// <returns>아이템의 다음 레벨 효과를 나타내는 문자열입니다.</returns>
    public string GetNextEffectValueString(int currentLevel)
    {
        if (currentLevel >= maxLevel) return "최대"; // 이미 최대 레벨인 경우

        if (targetItemData is PlusTimeItemData plusTimeData)
        {
            // ⬇️ [수정] timeToAdd 대신 GetEffectiveTimeToAdd 메서드 호출
            return $"{plusTimeData.GetEffectiveTimeToAdd(currentLevel + 1):F0}초"; // 소수점 없이 표시
        }
        if (targetItemData is LightItemData lightData)
        {
            // ⬇️ [수정] targetScaleMultiplier 대신 GetEffectiveTargetScaleMultiplier 메서드 호출
            return $"{lightData.GetEffectiveTargetScaleMultiplier(currentLevel + 1).x:F1}배"; // 소수점 첫째 자리까지 표시
        }
        return "알 수 없음"; // 정의되지 않은 BaseItemData 타입
    }
}
