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
    [Header("퀵슬롯 UI 슬롯들")]
    public QuickSlotUI_1[] quickSlots;  // 인스펙터에서 할당

    [Header("아이템 데이터")]
    public BaseItemData_1[] quickSlotItemDatas;  // 인스펙터에서 아이템 에셋 할당

    private InventorySystem_1.InventoryItem_1[] quickSlotsInUse;

    
    public static QuickSlotUIController_1 Instance { get; private set; }

   // public QuickSlotUI_1[] quickSlots; // Inspector에서 설정
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
            Debug.Log("Q 눌림!");
            UseQuickSlotItem(0); // 시간아이템 슬롯
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E 눌림!");
            UseQuickSlotItem(1); // 빛아이템 슬롯
        }
    }

    private void UseQuickSlotItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < quickSlotsInUse.Length)
        {
            var item = quickSlotsInUse[slotIndex];
            if (item != null && !item.usedInCurrentScene)
            {
                Debug.Log($"[Q/E 사용] 슬롯 {slotIndex} 아이템 사용: {item.itemName}");
                InventorySystem_1.Instance?.UseInventoryItem(item);
            }
            else
            {
                Debug.LogWarning($"[Q/E 사용] 슬롯 {slotIndex} 비어 있거나 이미 사용됨");
            }
        }
    }


    public void AssignItemToSpecificQuickSlot(InventorySystem_1.InventoryItem_1 itemToAssign)
    {
        if (itemToAssign == null || string.IsNullOrEmpty(itemToAssign.itemName))
        {
            Debug.LogWarning("[QuickSlotUIController_1] 할당할 아이템이 null이거나 이름이 없습니다.");
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
            Debug.Log($"[QuickSlotUIController_1] '{itemToAssign.itemName}'을(를) 퀵슬롯 {targetSlotIndex}에 할당");
            RefreshQuickSlotsUI();
        }
        else
        {
            Debug.LogWarning($"[QuickSlotUIController_1] 아이템 '{itemToAssign.itemName}'에 맞는 슬롯 인덱스를 찾을 수 없음");
        }
    }

    public void RefreshQuickSlotsUI()
    {
        for (int i = 0; i < quickSlots.Length; i++)
        {
            QuickSlotUI_1 slot = quickSlots[i];
            if (slot == null)
            {
                Debug.LogWarning($"[QuickSlotUIController_1] 슬롯 {i}가 null입니다.");
                continue;
            }

            if (slot.currentButton == null || slot.currentImage == null || slot.currentText == null)
            {
                Debug.LogError($"[QuickSlotUIController_1] 슬롯 {i} UI 컴포넌트 누락");
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

                    Debug.Log($"[QuickSlotUI] 슬롯 {i} 아이템 표시 완료: {item.itemName}");
                }
                else
                {
                    Debug.LogWarning($"[QuickSlotUI] 슬롯 {i} 아이콘 없음. itemData: {itemData}");
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
                Debug.Log($"[QuickSlotUIController_1] 퀵슬롯 {i}에서 '{itemToRemove.itemName}' 제거");
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
        Debug.Log("[QuickSlotUIController_1] 모든 퀵슬롯 초기화");
    }
}
