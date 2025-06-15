using UnityEngine;

[CreateAssetMenu(fileName = "NewPlusTimeItem", menuName = "Item/PlusTimeItem")]
public class PlusTimeItemData : ScriptableObject
{
    public float timeToAdd = 5f;
    public int point;      // 아이템 점수나 값 (필요하면 사용)
    public string itemName;
    public Sprite itemIcon;
}
