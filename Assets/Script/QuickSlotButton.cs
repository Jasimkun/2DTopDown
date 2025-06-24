using UnityEngine;
using UnityEngine.UI; // UI ���� Ŭ������ ����ϱ� ���� �߰�
using TMPro; // TextMeshPro�� ����Ѵٸ� �߰� (������� �ʴ´ٸ� ����)

/// <summary>
/// ���� �� ���� UI ��ư�� �����ϴ� ��ũ��Ʈ�Դϴ�.
/// ������ ������, ����, ��ư Ŭ�� �̺�Ʈ�� �����մϴ�.
/// �� ��ũ��Ʈ�� �� ������ �� UI ��ư GameObject�� ����Ǿ�� �մϴ�.
/// </summary>
public class QuickSlotButton : MonoBehaviour
{
    [Tooltip("������ �������� ǥ���� Image ������Ʈ�Դϴ�. �� Image ������Ʈ�� Ȱ��ȭ�Ǿ� �ְ�, Color�� Alpha ���� 0�� �ƴ��� Ȯ���ϼ���.")]
    public Image itemIcon; // ������ ������ ǥ�� Image

    [Tooltip("������ ������ ǥ���� Text �Ǵ� TextMeshProUGUI ������Ʈ�Դϴ�.")]
    public TextMeshProUGUI quantityText; // ������ ���� ǥ�� Text (TMPro ��� ��)
    // public Text quantityText; // �⺻ Text ��� �� (TMPro�� ������� �ʴ´ٸ� �� ���� �ּ� ó���ϰ� �� ���� ���)

    // �� ���Կ� ���� �Ҵ�� ������ ������ (����� ���� ����)
    private BaseItemData _currentItemData;
    // �� ���Կ� ���� �Ҵ�� ������ ���� (UI ������Ʈ��)
    private int _currentQuantity;

    // ���� �� ������ �ε����� �����ϱ� ���� ���� (QuickSlotUIController���� ����)
    private int _slotIndex;

    /// <summary>
    /// �� ������ �ε����� �����մϴ�.
    /// </summary>
    /// <param name="index">�� �� ���� ��ư�� �ε����Դϴ�.</param>
    public void SetSlotIndex(int index)
    {
        _slotIndex = index;
    }

    /// <summary>
    /// ��ũ��Ʈ�� �ε�� �� ȣ��˴ϴ�. ��ư Ŭ�� �����ʸ� �߰��մϴ�.
    /// </summary>
    private void Awake()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnQuickSlotClick); // ��ư Ŭ�� �� OnQuickSlotClick �޼��� ȣ��
        }
        else
        {
            Debug.LogWarning($"QuickSlotButton ��ũ��Ʈ�� ���� {gameObject.name}�� Button ������Ʈ�� �����ϴ�.");
        }
        ClearQuickSlot(); // �ʱ�ȭ �� ������ ��� (������ ����)
    }

    /// <summary>
    /// �� �� ���Կ� �������� �����ϰ� UI�� ������Ʈ�մϴ�.
    /// BaseItemData�� itemIcon�� �Ҵ�Ǿ� �־�� �������� ���Դϴ�.
    /// </summary>
    /// <param name="data">�� ���Կ� �Ҵ��� BaseItemData ��ü�Դϴ�.</param>
    /// <param name="quantity">�� ���Կ� �Ҵ��� �������� �����Դϴ�.</param>
    public void SetQuickSlotItem(BaseItemData data, int quantity)
    {
        _currentItemData = data;
        _currentQuantity = quantity;

        if (_currentItemData != null)
        {
            // ������ Image ������Ʈ�� �Ҵ�Ǿ� �ְ�, BaseItemData�� ������ ��������Ʈ�� �Ҵ�Ǿ� �ִٸ�
            if (itemIcon != null && _currentItemData.icon != null)
            {
                itemIcon.sprite = _currentItemData.icon; // BaseItemData�� ������ ���
                itemIcon.enabled = true; // ������ Image ������Ʈ Ȱ��ȭ
                // Debug.Log($"������ ����: {itemIcon.sprite.name} for {_currentItemData.itemName}");
            }
            else
            {
                // ������ Image ������Ʈ�� ���ų�, BaseItemData�� �������� �Ҵ���� ���� ���
                if (itemIcon != null) itemIcon.enabled = false; // ������ ��Ȱ��ȭ
                Debug.LogWarning($"�� ���� {_slotIndex}: ������ Image ������Ʈ�� ���ų�, {_currentItemData.itemName}�� itemIcon�� �Ҵ���� �ʾҽ��ϴ�. �������� ������ ���� �� �ֽ��ϴ�.");
            }

            if (quantityText != null)
            {
                // ������ 1���� ũ�� ǥ��, �ƴϸ� ����
                quantityText.text = (quantity > 1) ? quantity.ToString() : "";
                quantityText.enabled = (quantity > 1); // ���� �ؽ�Ʈ Ȱ��ȭ/��Ȱ��ȭ
            }
        }
        else
        {
            ClearQuickSlot(); // �����Ͱ� null�̸� ���� ���
        }
    }

    /// <summary>
    /// �� �� ������ ���� UI�� �ʱ�ȭ�մϴ�.
    /// ������ Image�� ���� Text�� ��Ȱ��ȭ�մϴ�.
    /// </summary>
    public void ClearQuickSlot()
    {
        _currentItemData = null;
        _currentQuantity = 0;

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false; // ������ ��Ȱ��ȭ
        }
        if (quantityText != null)
        {
            quantityText.text = "";
            quantityText.enabled = false; // ���� �ؽ�Ʈ ��Ȱ��ȭ
        }
    }

    /// <summary>
    /// �� ���� ��ư�� Ŭ���Ǿ��� �� ȣ��˴ϴ�.
    /// </summary>
    private void OnQuickSlotClick()
    {
        // Debug.Log($"�� ���� {_slotIndex} Ŭ����.");

        if (_currentItemData != null && _currentQuantity > 0)
        {
            Debug.Log($"�� ���� {_slotIndex}�� ������ [{_currentItemData.itemName}] ��� �õ�. ���� ����: {_currentQuantity}");

            // ���⿡ ���� ������ ��� ������ �߰��մϴ�.
            // ���� ���, InventorySystem�� ������ ����� ��û�ϰ�, ����� �����ϸ� ������ ������Ʈ�մϴ�.
            // �� ������ InventorySystem �Ǵ� �ٸ� ���� ���� ��ũ��Ʈ���� ó���ϴ� ���� �Ϲ����Դϴ�.

            // ����: InventorySystem�� ������ ��� ��û (InventorySystem�� UseItem() �޼��尡 �ִٰ� ����)
            // if (InventorySystem.Instance != null)
            // {
            //     bool success = InventorySystem.Instance.UseItem(_currentItemData, 1);
            //     if (success)
            //     {
            //         // UI ������Ʈ�� ���� QuickSlotUIController�� ���� ��û
            //         // QuickSlotUIController�� �̱����̰ų� ���� ���� �����ؾ� ��
            //         // �Ǵ� �̺�Ʈ �ý����� ����Ͽ� UI ������Ʈ�� �˸� �� ����
            //         // FindObjectOfType<QuickSlotUIController>()?.RefreshQuickSlotsUI();
            //     }
            // }

            // �߿���: ������ ��� �� InventorySystem�� �����͸� �����ϰ�,
            // QuickSlotUIController�� RefreshQuickSlotsUI()�� ȣ���Ͽ� UI�� �����ؾ� �մϴ�.
            // �Ϲ������� Item�� ����ϸ� �ش� Item�� Effect�� �ߵ��ϰ�, InventorySystem���� Item�� �����ؾ� �մϴ�.
        }
        else
        {
            Debug.Log($"�� ���� {_slotIndex}�� ����ְų� ������ ������ �����Ͽ� ����� �� �����ϴ�.");
            // ����ִ� ������ Ŭ������ ���� �߰� ���� (��: ������ �Ҵ� UI �˾�)
        }
    }
}
