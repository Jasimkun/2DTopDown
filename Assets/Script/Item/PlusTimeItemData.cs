using UnityEngine;
// System; 네임스페이스는 이제 필요 없으므로 제거합니다.

[CreateAssetMenu(fileName = "NewPlusTimeItem", menuName = "Item/PlusTimeItem")]
public class PlusTimeItemData : BaseItemData
{
    [Header("시간 추가 효과 설정")]
    [Tooltip("레벨 0에서의 기본 시간 보너스입니다.")]
    public float baseTimeToAdd = 5f;
    [Tooltip("업그레이드 레벨 1 증가당 추가되는 시간 보너스입니다.")]
    public float timeToAddPerLevel = 1f;

    public int point; // 아이템 점수나 값 (필요하면 사용)

    /// <summary>
    /// 주어진 레벨에 해당하는 실제 시간 보너스 값을 계산하여 반환합니다.
    /// </summary>
    /// <param name="level">계산할 업그레이드 레벨입니다.</param>
    /// <returns>해당 레벨에서의 최종 시간 보너스 값입니다.</returns>
    public float GetEffectiveTimeToAdd(int level)
    {
        return baseTimeToAdd + (level * timeToAddPerLevel);
    }

    /// <summary>
    /// 아이템 사용 효과를 정의하는 메서드 (BaseItemData의 추상 메서드 오버라이드)
    /// </summary>
    /// <param name="user">아이템을 사용하는 GameObject (여기서는 GameTimer에 시간을 추가하므로 user는 직접 사용되지 않을 수 있습니다)</param>
    public override void UseItemEffect(GameObject user = null)
    {
        // ShopManager로부터 아이템의 현재 업그레이드 레벨을 가져옵니다.
        // ShopManager가 아직 없으면 기본 레벨 0으로 가정합니다.
        int currentLevel = ShopManager.Instance != null ? ShopManager.Instance.GetItemUpgradeLevel(this.itemName) : 0;
        float timeBonus = GetEffectiveTimeToAdd(currentLevel); // 업그레이드 레벨을 반영한 시간 보너스 계산

        GameTimer timer = FindObjectOfType<GameTimer>(); // 씬에서 GameTimer 컴포넌트를 찾습니다.
        if (timer != null)
        {
            timer.AddTime(timeBonus); // GameTimer에 시간 추가
            Debug.Log($"[PlusTimeItemData] '{itemName}' 아이템 사용! {timeBonus}초 추가됨. (현재 레벨: {currentLevel})");
        }
        else
        {
            Debug.LogWarning("씬에서 'GameTimer' 스크립트를 찾을 수 없습니다. 시간을 추가할 수 없습니다.");
        }
    }
}
