using UnityEngine;

// 이 스크립터블 오브젝트를 에디터에서 쉽게 생성할 수 있도록 메뉴 추가
[CreateAssetMenu(fileName = "NewBaseItemData_1", menuName = "Inventory_1/Base Item Data")]
public abstract class BaseItemData_1 : ScriptableObject
{
    [Tooltip("아이템의 고유한 이름입니다. 인벤토리, 퀵슬롯 비교에 사용됩니다.")]
    public string itemName;
    [Tooltip("아이템의 UI 아이콘으로 사용될 스프라이트입니다.")]
    public Sprite icon;

    // 아이템 사용 시 실행되는 메서드. 자식 클래스에서 오버라이드하여 구체적인 효과 구현.
    public abstract void UseItemEffect();
}

// 예시: 시간 아이템 데이터
[CreateAssetMenu(fileName = "TimeItemData_1", menuName = "Inventory_1/Time Item Data")]
public class TimeItemData_1 : BaseItemData_1
{
    public override void UseItemEffect()
    {
        Debug.Log($"[TimeItemData_1] {itemName} 아이템 사용: 시간을 되감는 효과 발생!");
        // TODO: 여기에 시간 되감기 로직 구현
    }
}

// 예시: 빛 아이템 데이터
[CreateAssetMenu(fileName = "LightItemData_1", menuName = "Inventory_1/Light Item Data")]
public class LightItemData_1 : BaseItemData_1
{
    public override void UseItemEffect()
    {
        Debug.Log($"[LightItemData_1] {itemName} 아이템 사용: 주변이 밝아지는 효과 발생!");
        // TODO: 여기에 빛 효과 로직 구현
    }
}