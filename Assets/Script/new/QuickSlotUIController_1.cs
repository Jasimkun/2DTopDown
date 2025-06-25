using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class QuickSlotUI_1
{
    public Button currentButton;
    public Image currentImage;
    public TextMeshProUGUI currentText;
}

public class QuickSlotUIController_1 : MonoBehaviour
{
    [Header("������ UI ���Ե�")]
    public QuickSlotUI_1[] quickSlots;  // �ν����Ϳ��� �Ҵ�

    [Header("������ ������")]
    public BaseItemData_1[] quickSlotItemDatas;  // �ν����Ϳ��� ������ ���� �Ҵ�

    private InventorySystem_1.InventoryItem_1[] quickSlotsInUse;

    
    public static QuickSlotUIController_1 Instance { get; private set; }

   // public QuickSlotUI_1[] quickSlots; // Inspector���� ����
    //public InventorySystem_1.InventoryItem_1[] quickSlotsInUse;
    public Sprite emptySlotSprite;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (quickSlots != null)
                quickSlotsInUse = new InventorySystem_1.InventoryItem_1[quickSlots.Length];
            else
                quickSlotsInUse = new InventorySystem_1.InventoryItem_1[0];
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        RefreshQuickSlotsUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q ����!");
            UseQuickSlotItem(0); // �ð������� ����
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E ����!");
            UseQuickSlotItem(1); // �������� ����
        }
    }

    private void UseQuickSlotItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < quickSlotsInUse.Length)
        {
            var item = quickSlotsInUse[slotIndex];
            if (item != null && !item.usedInCurrentScene)
            {
                Debug.Log($"[Q/E ���] ���� {slotIndex} ������ ���: {item.itemName}");
                InventorySystem_1.Instance?.UseInventoryItem(item);
            }
            else
            {
                Debug.LogWarning($"[Q/E ���] ���� {slotIndex} ��� �ְų� �̹� ����");
            }
        }
    }


    public void AssignItemToSpecificQuickSlot(InventorySystem_1.InventoryItem_1 itemToAssign)
    {
        if (itemToAssign == null || string.IsNullOrEmpty(itemToAssign.itemName))
        {
            Debug.LogWarning("[QuickSlotUIController_1] �Ҵ��� �������� null�̰ų� �̸��� �����ϴ�.");
            return;
        }

        int targetSlotIndex = -1;
        string name = itemToAssign.itemName.Trim();

        if (name == "TimeItemData_1")
            targetSlotIndex = 0;
        else if (name == "LightItemData_1")
            targetSlotIndex = 1;

        if (targetSlotIndex >= 0 && targetSlotIndex < quickSlotsInUse.Length)
        {
            quickSlotsInUse[targetSlotIndex] = itemToAssign;
            Debug.Log($"[QuickSlotUIController_1] '{itemToAssign.itemName}'��(��) ������ {targetSlotIndex}�� �Ҵ�");
            RefreshQuickSlotsUI();
        }
        else
        {
            Debug.LogWarning($"[QuickSlotUIController_1] ������ '{itemToAssign.itemName}'�� �´� ���� �ε����� ã�� �� ����");
        }
    }

    public void RefreshQuickSlotsUI()
    {
        for (int i = 0; i < quickSlots.Length; i++)
        {
            QuickSlotUI_1 slot = quickSlots[i];
            if (slot == null)
            {
                Debug.LogWarning($"[QuickSlotUIController_1] ���� {i}�� null�Դϴ�.");
                continue;
            }

            if (slot.currentButton == null || slot.currentImage == null || slot.currentText == null)
            {
                Debug.LogError($"[QuickSlotUIController_1] ���� {i} UI ������Ʈ ����");
                continue;
            }

            slot.currentButton.onClick.RemoveAllListeners();
            slot.currentImage.gameObject.SetActive(true);

            if (i < quickSlotsInUse.Length && quickSlotsInUse[i] != null)
            {
                var item = quickSlotsInUse[i];
                var itemData = item.itemData;

                if (itemData != null && itemData.icon != null)
                {
                    slot.currentImage.sprite = itemData.icon;

                    if (item.usedInCurrentScene)
                    {
                        slot.currentImage.color = Color.gray;
                        slot.currentButton.interactable = false;
                        slot.currentText.text = "USED";
                    }
                    else
                    {
                        slot.currentImage.color = Color.white;
                        slot.currentButton.interactable = true;
                        slot.currentText.text = item.count.ToString();
                    }

                    int slotIndex = i;
                    slot.currentButton.onClick.AddListener(() =>
                    {
                        InventorySystem_1.Instance?.UseInventoryItem(quickSlotsInUse[slotIndex]);
                    });

                    Debug.Log($"[QuickSlotUI] ���� {i} ������ ǥ�� �Ϸ�: {item.itemName}");
                }
                else
                {
                    Debug.LogWarning($"[QuickSlotUI] ���� {i} ������ ����. itemData: {itemData}");
                    slot.currentImage.sprite = emptySlotSprite;
                    slot.currentImage.color = new Color(1, 1, 1, 0.3f);
                    slot.currentButton.interactable = false;
                    slot.currentText.text = "";
                }
            }
            else
            {
                slot.currentImage.sprite = emptySlotSprite;
                slot.currentImage.color = new Color(1, 1, 1, 0.3f);
                slot.currentButton.interactable = false;
                slot.currentText.text = "";
            }
        }
    }

    public void RemoveItemFromQuickSlot(InventorySystem_1.InventoryItem_1 itemToRemove)
    {
        if (itemToRemove == null) return;

        for (int i = 0; i < quickSlotsInUse.Length; i++)
        {
            if (quickSlotsInUse[i] == itemToRemove)
            {
                quickSlotsInUse[i] = null;
                Debug.Log($"[QuickSlotUIController_1] ������ {i}���� '{itemToRemove.itemName}' ����");
                RefreshQuickSlotsUI();
                return;
            }
        }
    }

    public void ClearAllQuickSlotsUI()
    {
        for (int i = 0; i < quickSlotsInUse.Length; i++)
            quickSlotsInUse[i] = null;

        RefreshQuickSlotsUI();
        Debug.Log("[QuickSlotUIController_1] ��� ������ �ʱ�ȭ");
    }
}
