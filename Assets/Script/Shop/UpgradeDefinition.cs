using System.Collections.Generic;
using UnityEngine;

// Unity 에디터에서 "Create/Shop/Upgrade Definition" 메뉴를 통해 이 에셋을 생성할 수 있습니다.
// 이 스크립터블 오브젝트는 게임 내 특정 아이템의 업그레이드 경로를 정의합니다.
[CreateAssetMenu(fileName = "NewUpgradeDefinition", menuName = "Shop/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    [Header("업그레이드 대상 아이템")]
    [Tooltip("이 업그레이드가 적용될 BaseItemData 에셋을 할당하세요. ShopManager의 upgradeableItemDatas 목록에 포함되어야 합니다.")]
    public BaseItemData targetItemData; // 이 업그레이드가 어떤 BaseItemData에 적용되는지 참조합니다.

    [Header("업그레이드 레벨별 비용")]
    [Tooltip("업그레이드 레벨별 필요한 골드 비용 목록입니다. 리스트의 인덱스 0은 '레벨 0에서 레벨 1로 업그레이드하는 비용'입니다. (Max Level까지)")]
    public List<int> costs; // costs[0] = Level 0 -> Level 1 비용, costs[1] = Level 1 -> Level 2 비용

    [Tooltip("이 업그레이드의 최대 레벨입니다. (레벨 0 포함. 예: Max Level이 3이면 레벨 0, 1, 2, 3까지 가능합니다)")]
    public int maxLevel = 3; // 이 업그레이드의 최대 레벨 (0부터 시작)

    [Header("UI 표시 설정")]
    [Tooltip("상점 UI에 표시될 이 업그레이드의 제목입니다 (예: '시간 추가 아이템 효율 증가').")]
    public string upgradeTitle; // 상점 UI에 표시될 제목

    [Tooltip("업그레이드 효과를 설명하는 텍스트 형식입니다. {0}은 현재 레벨의 값, {1}은 다음 레벨의 값입니다.")]
    public string effectDescriptionFormat; // 예: "시간 보너스: {0}초 -> {1}초"

    /// <summary>
    /// 지정된 목표 레벨로 업그레이드하는 데 필요한 골드 비용을 반환합니다.
    /// </summary>
    /// <param name="targetLevel">업그레이드할 목표 레벨 (현재 레벨 + 1)</param>
    /// <returns>필요한 골드 비용을 반환하거나, 해당 레벨의 비용이 정의되지 않았으면 -1을 반환합니다.</returns>
    public int GetCostForLevel(int targetLevel)
    {
        // 비용 리스트는 0부터 시작하므로 targetLevel-1 인덱스를 사용합니다.
        // 예를 들어, targetLevel 1 (레벨 0 -> 1)의 비용은 costs[0]에 있습니다.
        if (targetLevel > 0 && targetLevel <= costs.Count)
        {
            return costs[targetLevel - 1];
        }
        return -1; // 유효하지 않거나 정의되지 않은 레벨
    }

    /// <summary>
    /// 아이템의 현재 레벨에 따른 효과 값을 문자열로 반환합니다.
    /// 이 값은 UI에 표시될 수 있습니다.
    /// </summary>
    /// <param name="currentLevel">아이템의 현재 업그레이드 레벨입니다.</param>
    /// <returns>현재 레벨에서의 아이템 효과를 나타내는 문자열입니다.</returns>
    public string GetCurrentEffectValueString(int currentLevel)
    {
        // 대상 아이템 데이터의 실제 타입을 확인하여 해당 타입의 고유한 효과를 가져옵니다.
        if (targetItemData is PlusTimeItemData plusTimeData)
        {
            // PlusTimeItemData의 GetEffectiveTimeToAdd 메서드를 사용하여 현재 레벨의 시간 보너스를 가져옵니다.
            return $"{plusTimeData.GetEffectiveTimeToAdd(currentLevel)}초";
        }
        if (targetItemData is LightItemData lightData)
        {
            // LightItemData의 GetEffectiveTargetScaleMultiplier 메서드를 사용하여 현재 레벨의 스케일 배율을 가져옵니다.
            // 여기서는 Vector3의 x 값을 대표로 사용하고 소수점 한 자리까지 표시합니다.
            return $"{lightData.GetEffectiveTargetScaleMultiplier(currentLevel).x:F1}배";
        }
        // 업그레이드 대상 아이템이 정의되지 않았거나 지원되지 않는 타입일 경우.
        return "알 수 없음";
    }

    /// <summary>
    /// 아이템의 다음 레벨에 따른 예상 효과 값을 문자열로 반환합니다.
    /// 이 값은 UI에 표시될 수 있습니다.
    /// </summary>
    /// <param name="currentLevel">아이템의 현재 업그레이드 레벨입니다.</param>
    /// <returns>다음 레벨에서의 아이템 효과를 나타내는 문자열입니다. 최대 레벨인 경우 "최대"를 반환합니다.</returns>
    public string GetNextEffectValueString(int currentLevel)
    {
        // 이미 최대 레벨에 도달했다면 "최대"를 반환합니다.
        if (currentLevel >= maxLevel) return "최대";

        // 대상 아이템 데이터의 실제 타입을 확인하여 해당 타입의 고유한 다음 효과를 가져옵니다.
        if (targetItemData is PlusTimeItemData plusTimeData)
        {
            // PlusTimeItemData의 GetEffectiveTimeToAdd 메서드를 사용하여 다음 레벨의 시간 보너스를 가져옵니다.
            return $"{plusTimeData.GetEffectiveTimeToAdd(currentLevel + 1)}초";
        }
        if (targetItemData is LightItemData lightData)
        {
            // LightItemData의 GetEffectiveTargetScaleMultiplier 메서드를 사용하여 다음 레벨의 스케일 배율을 가져옵니다.
            return $"{lightData.GetEffectiveTargetScaleMultiplier(currentLevel + 1).x:F1}배";
        }
        // 업그레이드 대상 아이템이 정의되지 않았거나 지원되지 않는 타입일 경우.
        return "알 수 없음";
    }
}
