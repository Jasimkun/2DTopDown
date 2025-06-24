using UnityEngine;
// System; 네임스페이스는 Action 델리게이트를 사용하지 않으므로 제거합니다.

// 모든 아이템 데이터의 기본 클래스입니다.
// ScriptableObject를 상속받아 에셋으로 만들 수 있게 합니다.
public abstract class BaseItemData : ScriptableObject
{
    [Header("공통 아이템 속성")]
    public string itemName; // 아이템 이름
    public Sprite icon;     // 인벤토리 UI에 표시될 아이콘

    // ⬇️ [제거] OnUseItem 델리게이트를 제거합니다. 아이템 효과는 UseItemEffect에서 직접 처리합니다.
    // [NonSerialized]
    // public Action OnUseItem;

    /// <summary>
    /// 이 아이템의 효과를 적용하는 추상(abstract) 메서드입니다.
    /// 모든 하위 아이템 데이터 클래스에서 이 메서드를 반드시 오버라이드하여 고유한 효과를 구현해야 합니다.
    /// 추상 메서드는 본문(body)을 가질 수 없습니다.
    /// </summary>
    /// <param name="user">아이템을 사용하는 GameObject (예: 플레이어)</param>
    public abstract void UseItemEffect(GameObject user = null);
}
