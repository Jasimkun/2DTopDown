using UnityEngine;

// �� ��ũ���ͺ� ������Ʈ�� �����Ϳ��� ���� ������ �� �ֵ��� �޴� �߰�
[CreateAssetMenu(fileName = "NewBaseItemData_1", menuName = "Inventory_1/Base Item Data")]
public abstract class BaseItemData_1 : ScriptableObject
{
    [Tooltip("�������� ������ �̸��Դϴ�. �κ��丮, ������ �񱳿� ���˴ϴ�.")]
    public string itemName;
    [Tooltip("�������� UI ���������� ���� ��������Ʈ�Դϴ�.")]
    public Sprite icon;

    // ������ ��� �� ����Ǵ� �޼���. �ڽ� Ŭ�������� �������̵��Ͽ� ��ü���� ȿ�� ����.
    public abstract void UseItemEffect();
}

// ����: �ð� ������ ������
[CreateAssetMenu(fileName = "TimeItemData_1", menuName = "Inventory_1/Time Item Data")]
public class TimeItemData_1 : BaseItemData_1
{
    public override void UseItemEffect()
    {
        Debug.Log($"[TimeItemData_1] {itemName} ������ ���: �ð��� �ǰ��� ȿ�� �߻�!");
        // TODO: ���⿡ �ð� �ǰ��� ���� ����
    }
}

// ����: �� ������ ������
[CreateAssetMenu(fileName = "LightItemData_1", menuName = "Inventory_1/Light Item Data")]
public class LightItemData_1 : BaseItemData_1
{
    public override void UseItemEffect()
    {
        Debug.Log($"[LightItemData_1] {itemName} ������ ���: �ֺ��� ������� ȿ�� �߻�!");
        // TODO: ���⿡ �� ȿ�� ���� ����
    }
}