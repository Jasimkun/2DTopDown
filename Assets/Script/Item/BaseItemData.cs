using UnityEngine;
using System; // Action 델리게이트 사용을 위해

// 모든 아이템 데이터의 기본 클래스입니다.
// ScriptableObject를 상속받아 에셋으로 만들 수 있게 합니다.
public abstract class BaseItemData : ScriptableObject
{
    [Header("공통 아이템 속성")]
    public string itemName; // 아이템 이름
    public Sprite icon;     // 인벤토리 UI에 표시될 아이콘

    // 퀵 슬롯 UI에서 아이템 사용 시 호출될 액션 델리게이트입니다.
    // [NonSerialized]로 JSON 저장 시 제외되어 씬 로드 시마다 다시 연결됩니다.
    [NonSerialized]
    public Action OnUseItem;

    /// <summary>
    /// 이 아이템의 효과를 적용하는 가상(virtual) 메서드입니다.
    /// 모든 하위 아이템 데이터 클래스에서 이 메서드를 오버라이드하여 고유한 효과를 구현할 수 있습니다.
    /// </summary>
    /// <param name="user">아이템을 사용하는 GameObject (예: 플레이어)</param>
    public virtual void UseItemEffect(GameObject user = null) // <<< 이 줄에 'virtual' 키워드가 반드시 있어야 합니다.
    {
        Debug.Log($"[BaseItemData] '{itemName}' 아이템의 기본 효과가 사용되었습니다.");
        // OnUseItem 델리게이트가 할당되어 있다면 실행합니다.
        OnUseItem?.Invoke();
    }
}
