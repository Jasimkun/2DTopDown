using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class QuickSlotUI
{
    public Button currentButton;
    public Image currentImage;
    public TextMeshProUGUI currentText;
}

public class QuickSlotUIController : MonoBehaviour
{
    public static QuickSlotUIController Instance { get; private set; }

    public QuickSlotUI[] quickSlots;
    public Sprite defaultSlotSprite;

    // ****** 오타 수정 완료: 'Use' 대문자 ******
    private InventorySystem.InventoryItem[] quickSlotsInUse;

    void Awake()
    {
        Debug.Log("[QuickSlotUIController] Awake 호출됨. GameObject 이름: " + gameObject.name);

        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 이 부분은 InventoryManager 스크립트에서 처리하는 것이 더 안전합니다.
            // 또는 이 스크립트가 붙은 GameObject를 DontDestroyOnLoad에 직접 자식으로 두세요.

            Debug.Log("[QuickSlotUIController] QuickSlotUIController 인스턴스 설정 완료.");

            // quickSlots 배열의 길이를 기준으로 quickSlotsInUse 배열 초기화
            if (quickSlots != null)
            {
                quickSlotsInUse = new InventorySystem.InventoryItem[quickSlots.Length];
            }
            else
            {
                Debug.LogError("[QuickSlotUIController] quickSlots 배열이 null입니다! Inspector에서 Quick Slots 배열을 설정해주세요.");
                quickSlotsInUse = new InventorySystem.InventoryItem[0]; // 안전 장치
            }
        }
        else if (Instance != this)
        {
            Debug.LogWarning("[QuickSlotUIController] 중복 인스턴스 발견! 이 오브젝트를 파괴합니다. (기존 인스턴스 이름: " + Instance.gameObject.name + ", 새로 생긴 인스턴스 이름: " + gameObject.name + ")");
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Debug.Log("[QuickSlotUIController] Start 호출됨. GameObject 이름: " + gameObject.name);
        if (defaultSlotSprite == null)
            Debug.LogWarning("[QuickSlotUIController] 기본 슬롯 스프라이트가 할당되지 않았습니다.");

        Canvas parentCanvas = FindObjectOfType<Canvas>();
        if (parentCanvas != null && transform.parent != parentCanvas.transform)
        {
            // Canvas가 부모가 아니면 Canvas의 자식으로 설정 (UI가 보이도록)
            transform.SetParent(parentCanvas.transform, false);
            Debug.Log("[QuickSlotUIController] ItemSlotsPanel을 새 Canvas에 재부모화했습니다.");
        }
        else if (parentCanvas == null)
        {
            Debug.LogWarning("[QuickSlotUIController] 현재 씬에서 Canvas를 찾을 수 없습니다. ItemSlotsPanel이 표시되지 않을 수 있습니다.");
        }

        RefreshQuickSlotsUI();
    }

    public void AssignItemToSpecificQuickSlot(InventorySystem.InventoryItem itemToAssign)
    {
        if (itemToAssign == null || string.IsNullOrEmpty(itemToAssign.itemName))
        {
            Debug.LogWarning("[QuickSlotUIController] 할당하려는 아이템이 null이거나 이름이 비어있습니다.");
            return;
        }

        int targetSlotIndex = -1;

        // 중요: 이 문자열은 BaseItemData의 Item Name 필드와 정확히 일치해야 합니다. (띄어쓰기 포함 여부 확인)
        // 사용자 이미지(image_a36f7e.png)에 따르면 "빛아이템" (띄어쓰기 없음)으로 설정되어 있습니다.
        if (itemToAssign.itemName == "시간아이템")
        {
            targetSlotIndex = 0;
        }
        else if (itemToAssign.itemName == "빛아이템")
        {
            targetSlotIndex = 1;
        }
        // 다른 고정 아이템이 있다면 여기에 else if로 추가하세요.

        if (targetSlotIndex != -1 && targetSlotIndex < quickSlotsInUse.Length) // 인덱스 범위 체크 추가
        {
            quickSlotsInUse[targetSlotIndex] = itemToAssign; // 여기에 아이템이 할당됨
            Debug.Log($"[QuickSlotUIController] {itemToAssign.itemName}이(가) 고정된 퀵슬롯 {targetSlotIndex}에 할당되었습니다.");
            RefreshQuickSlotsUI(); // 할당 후 UI 갱신 요청
        }
        else
        {
            Debug.LogWarning($"[QuickSlotUIController] {itemToAssign.itemName}을(를) 위한 고정 퀵슬롯 인덱스를 찾을 수 없거나 슬롯 범위를 벗어났습니다. 슬롯 개수: {quickSlotsInUse.Length}, 시도된 인덱스: {targetSlotIndex}. 아이템 이름 확인: '{itemToAssign.itemName}'");
        }
    }

    public void RefreshQuickSlotsUI()
    {
        for (int i = 0; i < quickSlots.Length; i++)
        {
            QuickSlotUI slot = quickSlots[i];
            if (slot == null)
            {
                Debug.LogWarning($"[QuickSlotUIController] QuickSlots 배열의 Element {i}가 null입니다. Inspector를 확인해주세요.");
                continue;
            }

            if (slot.currentButton == null || slot.currentImage == null || slot.currentText == null)
            {
                Debug.LogError($"[QuickSlotUIController] 슬롯 {i}의 UI 컴포넌트(버튼, 이미지, 텍스트) 중 일부가 누락되었습니다. Inspector를 확인해주세요.");
                SetDefaultQuickSlotUI(slot, "UI 오류", false);
                continue;
            }

            slot.currentButton.onClick.RemoveAllListeners();

            if (i < quickSlotsInUse.Length && quickSlotsInUse[i] != null)
            {
                InventorySystem.InventoryItem item = quickSlotsInUse[i];
                BaseItemData itemData = item.itemData;

                Debug.Log($"[QuickSlotUIController Debug] Slot {i}: Item Name='{item.itemName}', BaseItemData is {(itemData != null ? "NOT NULL" : "NULL")}. Icon is {(itemData != null && itemData.icon != null ? itemData.icon.name : "NULL")}.");

                if (itemData != null && itemData.icon != null)
                {
                    slot.currentImage.sprite = itemData.icon;
                    slot.currentImage.color = item.usedInCurrentScene ? Color.gray : Color.white;
                    slot.currentText.text = item.usedInCurrentScene ? "USED" : item.count.ToString();
                    slot.currentButton.interactable = !item.usedInCurrentScene;

                    int slotIndex = i; // 클로저를 위한 로컬 변수
                    slot.currentButton.onClick.AddListener(() =>
                    {
                        InventorySystem.Instance?.UseInventoryItem(quickSlotsInUse[slotIndex]);
                    });
                }
                else
                {
                    Debug.LogWarning($"[QuickSlotUIController] 슬롯 {i}의 아이템({item.itemName})에 할당된 BaseItemData 또는 아이콘이 없어 UI를 표시할 수 없습니다. (저장명: {item.itemDataName})");
                    SetDefaultQuickSlotUI(slot, "데이터 오류", false);
                }
            }
            else
            {
                SetDefaultQuickSlotUI(slot, "", false); // 아이템이 없는 슬롯은 기본으로
            }
        }
    }

    private void SetDefaultQuickSlotUI(QuickSlotUI slot, string text, bool interactable)
    {
        if (slot.currentImage != null)
        {
            slot.currentImage.sprite = defaultSlotSprite; // 기본 스프라이트 사용
            slot.currentImage.color = Color.gray; // 기본 색상
        }
        if (slot.currentText != null)
            slot.currentText.text = text;
        if (slot.currentButton != null)
        {
            slot.currentButton.interactable = interactable;
            slot.currentButton.onClick.RemoveAllListeners();
        }
    }

    public void ClearAllQuickSlotsUI()
    {
        for (int i = 0; i < quickSlotsInUse.Length; i++)
        {
            quickSlotsInUse[i] = null;
        }
        RefreshQuickSlotsUI();
        Debug.Log("[QuickSlotUIController] 모든 퀵슬롯 UI가 초기화되었습니다.");
    }

    public void RemoveItemFromQuickSlot(InventorySystem.InventoryItem itemToRemove)
    {
        if (itemToRemove == null) return;

        for (int i = 0; i < quickSlotsInUse.Length; i++)
        {
            if (quickSlotsInUse[i] == itemToRemove)
            {
                quickSlotsInUse[i] = null;
                Debug.Log($"[QuickSlotUIController] 퀵슬롯 {i}에서 '{itemToRemove.itemName}' 제거.");
                RefreshQuickSlotsUI();
                return;
            }
        }
    }
}