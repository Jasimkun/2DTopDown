using UnityEngine;

[CreateAssetMenu(fileName = "NewPlusTimeItem", menuName = "Item/PlusTimeItem")]
public class PlusTimeItemData : ScriptableObject
{
    public float timeToAdd = 5f;
    public int point;      // ������ ������ �� (�ʿ��ϸ� ���)
}
