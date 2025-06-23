using System.Collections.Generic;
using UnityEngine;

// Unity �����Ϳ��� "Create/Shop/Upgrade Definition" �޴��� ���� �� ������ ������ �� �ֽ��ϴ�.
// �� ��ũ���ͺ� ������Ʈ�� ���� �� Ư�� �������� ���׷��̵� ��θ� �����մϴ�.
[CreateAssetMenu(fileName = "NewUpgradeDefinition", menuName = "Shop/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    [Header("���׷��̵� ��� ������")]
    [Tooltip("�� ���׷��̵尡 ����� BaseItemData ������ �Ҵ��ϼ���. ShopManager�� upgradeableItemDatas ��Ͽ� ���ԵǾ�� �մϴ�.")]
    public BaseItemData targetItemData; // �� ���׷��̵尡 � BaseItemData�� ����Ǵ��� �����մϴ�.

    [Header("���׷��̵� ������ ���")]
    [Tooltip("���׷��̵� ������ �ʿ��� ��� ��� ����Դϴ�. ����Ʈ�� �ε��� 0�� '���� 0���� ���� 1�� ���׷��̵��ϴ� ���'�Դϴ�. (Max Level����)")]
    public List<int> costs; // costs[0] = Level 0 -> Level 1 ���, costs[1] = Level 1 -> Level 2 ���

    [Tooltip("�� ���׷��̵��� �ִ� �����Դϴ�. (���� 0 ����. ��: Max Level�� 3�̸� ���� 0, 1, 2, 3���� �����մϴ�)")]
    public int maxLevel = 3; // �� ���׷��̵��� �ִ� ���� (0���� ����)

    [Header("UI ǥ�� ����")]
    [Tooltip("���� UI�� ǥ�õ� �� ���׷��̵��� �����Դϴ� (��: '�ð� �߰� ������ ȿ�� ����').")]
    public string upgradeTitle; // ���� UI�� ǥ�õ� ����

    [Tooltip("���׷��̵� ȿ���� �����ϴ� �ؽ�Ʈ �����Դϴ�. {0}�� ���� ������ ��, {1}�� ���� ������ ���Դϴ�.")]
    public string effectDescriptionFormat; // ��: "�ð� ���ʽ�: {0}�� -> {1}��"

    /// <summary>
    /// ������ ��ǥ ������ ���׷��̵��ϴ� �� �ʿ��� ��� ����� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="targetLevel">���׷��̵��� ��ǥ ���� (���� ���� + 1)</param>
    /// <returns>�ʿ��� ��� ����� ��ȯ�ϰų�, �ش� ������ ����� ���ǵ��� �ʾ����� -1�� ��ȯ�մϴ�.</returns>
    public int GetCostForLevel(int targetLevel)
    {
        // ��� ����Ʈ�� 0���� �����ϹǷ� targetLevel-1 �ε����� ����մϴ�.
        // ���� ���, targetLevel 1 (���� 0 -> 1)�� ����� costs[0]�� �ֽ��ϴ�.
        if (targetLevel > 0 && targetLevel <= costs.Count)
        {
            return costs[targetLevel - 1];
        }
        return -1; // ��ȿ���� �ʰų� ���ǵ��� ���� ����
    }

    /// <summary>
    /// �������� ���� ������ ���� ȿ�� ���� ���ڿ��� ��ȯ�մϴ�.
    /// �� ���� UI�� ǥ�õ� �� �ֽ��ϴ�.
    /// </summary>
    /// <param name="currentLevel">�������� ���� ���׷��̵� �����Դϴ�.</param>
    /// <returns>���� ���������� ������ ȿ���� ��Ÿ���� ���ڿ��Դϴ�.</returns>
    public string GetCurrentEffectValueString(int currentLevel)
    {
        // ��� ������ �������� ���� Ÿ���� Ȯ���Ͽ� �ش� Ÿ���� ������ ȿ���� �����ɴϴ�.
        if (targetItemData is PlusTimeItemData plusTimeData)
        {
            // PlusTimeItemData�� GetEffectiveTimeToAdd �޼��带 ����Ͽ� ���� ������ �ð� ���ʽ��� �����ɴϴ�.
            return $"{plusTimeData.GetEffectiveTimeToAdd(currentLevel)}��";
        }
        if (targetItemData is LightItemData lightData)
        {
            // LightItemData�� GetEffectiveTargetScaleMultiplier �޼��带 ����Ͽ� ���� ������ ������ ������ �����ɴϴ�.
            // ���⼭�� Vector3�� x ���� ��ǥ�� ����ϰ� �Ҽ��� �� �ڸ����� ǥ���մϴ�.
            return $"{lightData.GetEffectiveTargetScaleMultiplier(currentLevel).x:F1}��";
        }
        // ���׷��̵� ��� �������� ���ǵ��� �ʾҰų� �������� �ʴ� Ÿ���� ���.
        return "�� �� ����";
    }

    /// <summary>
    /// �������� ���� ������ ���� ���� ȿ�� ���� ���ڿ��� ��ȯ�մϴ�.
    /// �� ���� UI�� ǥ�õ� �� �ֽ��ϴ�.
    /// </summary>
    /// <param name="currentLevel">�������� ���� ���׷��̵� �����Դϴ�.</param>
    /// <returns>���� ���������� ������ ȿ���� ��Ÿ���� ���ڿ��Դϴ�. �ִ� ������ ��� "�ִ�"�� ��ȯ�մϴ�.</returns>
    public string GetNextEffectValueString(int currentLevel)
    {
        // �̹� �ִ� ������ �����ߴٸ� "�ִ�"�� ��ȯ�մϴ�.
        if (currentLevel >= maxLevel) return "�ִ�";

        // ��� ������ �������� ���� Ÿ���� Ȯ���Ͽ� �ش� Ÿ���� ������ ���� ȿ���� �����ɴϴ�.
        if (targetItemData is PlusTimeItemData plusTimeData)
        {
            // PlusTimeItemData�� GetEffectiveTimeToAdd �޼��带 ����Ͽ� ���� ������ �ð� ���ʽ��� �����ɴϴ�.
            return $"{plusTimeData.GetEffectiveTimeToAdd(currentLevel + 1)}��";
        }
        if (targetItemData is LightItemData lightData)
        {
            // LightItemData�� GetEffectiveTargetScaleMultiplier �޼��带 ����Ͽ� ���� ������ ������ ������ �����ɴϴ�.
            return $"{lightData.GetEffectiveTargetScaleMultiplier(currentLevel + 1).x:F1}��";
        }
        // ���׷��̵� ��� �������� ���ǵ��� �ʾҰų� �������� �ʴ� Ÿ���� ���.
        return "�� �� ����";
    }
}
