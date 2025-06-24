using UnityEngine;

[CreateAssetMenu(fileName = "NewPlusTimeItem", menuName = "Inventory/Items/Plus Time Item")]
public class PlusTimeItemData : BaseItemData
{
    [Tooltip("이 아이템 사용 시 추가될 시간(초)입니다.")]
    public float timeToAdd = 5f;

    // BaseItemData의 UseItemEffect() 메서드를 오버라이드하여 Plus Time Item의 고유 로직 구현
    public override void UseItemEffect()
    {
        Debug.Log($"[PlusTimeItemData] '{itemName}' 아이템 사용됨! 시간 {timeToAdd}초 추가.");

        // 실제 게임 로직에 따라 아래처럼 시간 추가
        // TimeManager.Instance.AddTime(timeToAdd);
    }
    public float GetEffectiveTimeToAdd()
    {
        // 여기서 업그레이드나 기타 로직 고려 가능
        return timeToAdd;
    }
    public float GetEffectiveTimeToAdd(int level)
    {
        // 레벨에 따른 증가 로직 예시
        return timeToAdd * (1f + 0.1f * level); // 레벨마다 10% 증가
    }

}
