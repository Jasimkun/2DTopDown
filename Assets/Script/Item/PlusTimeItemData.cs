using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewPlusTimeItem", menuName = "Item/PlusTimeItem")]
public class PlusTimeItemData : ScriptableObject
{
    public float timeToAdd = 5f;
    public int point;      // 아이템 점수나 값 (필요하면 사용)
    public string itemName;
    public Sprite icon;

    // --- 추가: 아이템 사용 시 실행될 액션 ---
    // 이 액션은 아이템이 사용될 때 호출될 메서드를 저장합니다.
    [NonSerialized] // Unity 에디터에 노출되지 않도록
    public Action OnUseItem;

    // 아이템 사용 시 호출될 공용 메서드
    public void UseItemEffect()
    {
        if (OnUseItem != null)
        {
            OnUseItem.Invoke();
        }
        else
        {
            Debug.LogWarning($"아이템 '{itemName}'에 대한 사용 효과가 정의되지 않았습니다.");
        }
    }
}
