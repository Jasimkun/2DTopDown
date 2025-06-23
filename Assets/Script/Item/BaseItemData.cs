using UnityEngine;
using System;

public abstract class BaseItemData : ScriptableObject
{
    [Header("공통 아이템 속성")]
    public string itemName;
    public Sprite icon;

    [NonSerialized]
    public Action OnUseItem;

    // ⬇️ [추가] 이 아이템이 도달할 수 있는 최대 업그레이드 레벨 (0-인덱스)
    // 예를 들어, 3이면 레벨 0, 1, 2, 3까지 가능합니다.
    public int maxUpgradeLevel = 3;

    // 아이템 사용 효과를 정의하는 추상 메서드.
    // 모든 하위 아이템 데이터 클래스에서 이 메서드를 반드시 오버라이드해야 합니다.
    public abstract void UseItemEffect(GameObject user = null);
}
