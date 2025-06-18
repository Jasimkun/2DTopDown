using UnityEngine;

// Unity �������� Assets �޴����� "Create/Item/LightItem"���� �� ScriptableObject Asset�� ������ �� �ֽ��ϴ�.
[CreateAssetMenu(fileName = "NewLightItem", menuName = "Item/LightItem")]
public class LightItemData : ScriptableObject
{
    [Header("��������Ʈ ������ ȿ�� ����")]
    [Tooltip("���� ������ ��� ��ǥ ������ �����Դϴ�.")]
    public Vector3 targetScaleMultiplier = new Vector3(1.5f, 1.5f, 1.5f);


    [Tooltip("�������� Ŀ�� ���·� �����Ǵ� �ð� (��)�Դϴ�.")]
    public float scaleHoldDuration = 5f;

    public int point;      // ������ ������ �� (�ʿ��ϸ� ���)
    public string itemName;
    public Sprite icon;

    /// <summary>
    /// �� ȿ���� Ư�� GameObject�� �����ϵ��� �����մϴ�.
    /// ��� GameObject�� ScaleEffectHandler ������Ʈ�� ������ �ڵ����� �߰��մϴ�.
    /// </summary>
    /// <param name="targetObject">������ ȿ���� ������ GameObject�Դϴ�.</param>
    public void ApplyEffect(GameObject targetObject)
    {
        // targetObject�� ScaleEffectHandler ������Ʈ�� �ִ��� Ȯ���ϰ� ������ �߰��մϴ�.
        ScaleEffectHandler handler = targetObject.GetComponent<ScaleEffectHandler>();
        if (handler == null)
        {
            handler = targetObject.AddComponent<ScaleEffectHandler>();
        }

        // �ڵ鷯���� ȿ�� ������ �����ϰ� �����ϵ��� �մϴ�.
        handler.StartScaleEffect(this);
    }
}