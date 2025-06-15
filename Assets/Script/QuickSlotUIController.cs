using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class QuickSlotUIController : MonoBehaviour
{
   
    // �� �� ���� ��ư�� �� ���� UI ��ҵ��� ����
    [System.Serializable] // Inspector���� ���� �����ϱ� ����
    public class QuickSlot
    {
        public Button slotButton;
        public Image itemIcon; // ������ ������ ǥ�ÿ�
        public TextMeshProUGUI itemCountText; // ������ ���� ǥ�ÿ� (���� ����)
        public KeyCode hotkey; // �ش� ���Կ� �Ҵ��� ����Ű (��: KeyCode.Alpha1)
    }

    public QuickSlot[] quickSlots; // �� ���� �迭
    [Header("UI References")]
    [Header("Dependencies")]
    public InventorySystem inventorySystem; // InventorySystem ��ũ��Ʈ ����

    void Awake()
    {
        // �κ��丮 �ý����� ����Ǿ� ���� �ʴٸ� ã�Ƽ� ����
        if (inventorySystem == null)
        {
            inventorySystem = FindObjectOfType<InventorySystem>();
            if (inventorySystem == null)
            {
                Debug.LogError("InventorySystem�� ���� �����ϴ�. QuickSlotUIController�� �۵����� �ʽ��ϴ�.");
            }
        }

        // �� �� ���Կ� �̺�Ʈ ������ �߰�
        for (int i = 0; i < quickSlots.Length; i++)
        {
            int slotIndex = i; // Ŭ���� ���� ����
            if (quickSlots[slotIndex].slotButton != null)
            {
                quickSlots[slotIndex].slotButton.onClick.AddListener(() => OnQuickSlotClicked(slotIndex));
            }
        }

        // InventorySystem�� ������ ��� ���� �� UI�� �����ϵ��� �̺�Ʈ ���� (���� ����, AddItem �� RefreshUI ȣ���� �� ���� �� ����)
        // inventorySystem.OnInventoryChanged += RefreshQuickSlotsUI; // ���� InventorySystem�� �̷� �̺�Ʈ�� �ִٸ�
    }

    void Start()
    {
        // ���� ���� �� �� ���� UI �ʱ�ȭ
        RefreshQuickSlotsUI();
    }

    void Update()
    {
        // ����Ű �Է� ó��
        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i].slotButton != null && quickSlots[i].hotkey != KeyCode.None && Input.GetKeyDown(quickSlots[i].hotkey))
            {
                // �ش� ���Կ� �������� �ְ�, ��� ������ ���� Ŭ�� �̺�Ʈ Ʈ����
                if (i < inventorySystem.items.Count) // �������� ���Կ� �Ҵ�Ǿ� ���� ��
                {
                    OnQuickSlotClicked(i);
                }
            }
        }
    }

    public void RefreshQuickSlotsUI()
    {
        // ��� ���� �ʱ�ȭ (�� ���·� ����)
        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i].itemIcon != null)
            {
                quickSlots[i].itemIcon.sprite = null; // ������ ����
                quickSlots[i].itemIcon.color = new Color(1, 1, 1, 0); // �����ϰ� ���� (������ ����)
            }
            if (quickSlots[i].itemCountText != null)
            {
                quickSlots[i].itemCountText.text = ""; // ���� �ؽ�Ʈ ����
            }
            if (quickSlots[i].slotButton != null)
            {
                quickSlots[i].slotButton.interactable = false; // �⺻������ ��Ȱ��ȭ
            }
        }

        // ���� �κ��丮 ������ ����� ������� �� ���� ä���
        // (���� inventorySystem.items�� ��� ȹ�� �������� ������ �����Ƿ�, �� ���Կ� � �������� ��ġ���� �����ϴ� ������ �ʿ���)
        // ���⼭�� �κ��丮�� ù ��°, �� ��° �������� �� ���Կ� �Ҵ��Ѵٰ� �����մϴ�.
        for (int i = 0; i < inventorySystem.items.Count && i < quickSlots.Length; i++)
        {
            InventoryItem invItem = inventorySystem.items[i];
            PlusTimeItemData itemData = inventorySystem.GetItemDataByName(invItem.itemName);

            if (itemData != null)
            {
                if (quickSlots[i].itemIcon != null && itemData.itemIcon != null) // itemData�� ������ ��������Ʈ�� �ִٸ�
                {
                    quickSlots[i].itemIcon.sprite = itemData.itemIcon;
                    quickSlots[i].itemIcon.color = new Color(1, 1, 1, 1); // �������ϰ� ���� (������ ǥ��)
                }
                if (quickSlots[i].itemCountText != null)
                {
                    quickSlots[i].itemCountText.text = invItem.count.ToString(); // ������ ���� ǥ��
                }
                if (quickSlots[i].slotButton != null)
                {
                    quickSlots[i].slotButton.interactable = true; // �������� ������ Ȱ��ȭ
                }
            }
        }
    }

    // �� ���� Ŭ�� �� ȣ��
    void OnQuickSlotClicked(int slotIndex)
    {
        // �ش� ���Կ� �������� �ִ��� Ȯ��
        if (slotIndex >= 0 && slotIndex < inventorySystem.items.Count)
        {
            InventoryItem invItemToUse = inventorySystem.items[slotIndex];
            PlusTimeItemData itemData = inventorySystem.GetItemDataByName(invItemToUse.itemName);

            if (itemData != null)
            {
                UseItemEffect(itemData); // ������ ȿ�� ����

                // ������ ��� �� �κ��丮���� ���� (���� ����ϸ� ������� �������̶��)
                // ���� count�� �׻� 1�� ItemSystem�̹Ƿ�, RemoveAll ���
                inventorySystem.items.RemoveAll(i => i.itemName == itemData.itemName);

                inventorySystem.SaveInventory(); // ���� ���� ����
                RefreshQuickSlotsUI(); // UI ���� (���ŵ� ������ �ݿ�)
            }
            else
            {
                Debug.LogWarning($"���� {slotIndex + 1}�� �Ҵ�� '{invItemToUse.itemName}'�� �����Ͱ� �����ϴ�.");
            }
        }
        else
        {
            Debug.Log("�� �� �����Դϴ�.");
        }
    }

    // ���� ������ ȿ���� �����ϴ� �Լ� (���⿡ ���� ���� �߰�)
    private void UseItemEffect(PlusTimeItemData itemData)
    {
        // TODO: ���⼭ ������ ������ ȿ���� �����ϴ� ������ �����մϴ�.
        // ��: �÷��̾��� �ð��� �߰��ϴ� �Լ� ȣ��
        Debug.Log($"������ {itemData.itemName} ���! (ȿ��: {itemData.timeToAdd}�� �߰�)");

        // ����: TimeManager ���� ���� �ð��� �߰��ϴ� �Լ� ȣ��
        // FindObjectOfType<TimeManager>()?.AddTime(itemData.timeToAdd);
        // �Ǵ� Ư�� �÷��̾� ��ũ��Ʈ�� �Լ� ȣ��
        // FindObjectOfType<PlayerController>()?.ApplyTimeEffect(itemData.timeToAdd);
    }
}