using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class QuickSlotUIController : MonoBehaviour
{
   
    // 각 퀵 슬롯 버튼과 그 안의 UI 요소들을 연결
    [System.Serializable] // Inspector에서 쉽게 설정하기 위함
    public class QuickSlot
    {
        public Button slotButton;
        public Image itemIcon; // 아이템 아이콘 표시용
        public TextMeshProUGUI itemCountText; // 아이템 개수 표시용 (선택 사항)
        public KeyCode hotkey; // 해당 슬롯에 할당할 단축키 (예: KeyCode.Alpha1)
    }

    public QuickSlot[] quickSlots; // 퀵 슬롯 배열
    [Header("UI References")]
    [Header("Dependencies")]
    public InventorySystem inventorySystem; // InventorySystem 스크립트 연결

    void Awake()
    {
        // 인벤토리 시스템이 연결되어 있지 않다면 찾아서 연결
        if (inventorySystem == null)
        {
            inventorySystem = FindObjectOfType<InventorySystem>();
            if (inventorySystem == null)
            {
                Debug.LogError("InventorySystem이 씬에 없습니다. QuickSlotUIController가 작동하지 않습니다.");
            }
        }

        // 각 퀵 슬롯에 이벤트 리스너 추가
        for (int i = 0; i < quickSlots.Length; i++)
        {
            int slotIndex = i; // 클로저 문제 방지
            if (quickSlots[slotIndex].slotButton != null)
            {
                quickSlots[slotIndex].slotButton.onClick.AddListener(() => OnQuickSlotClicked(slotIndex));
            }
        }

        // InventorySystem의 아이템 목록 변경 시 UI를 갱신하도록 이벤트 구독 (선택 사항, AddItem 후 RefreshUI 호출이 더 나을 수 있음)
        // inventorySystem.OnInventoryChanged += RefreshQuickSlotsUI; // 만약 InventorySystem에 이런 이벤트가 있다면
    }

    void Start()
    {
        // 게임 시작 시 퀵 슬롯 UI 초기화
        RefreshQuickSlotsUI();
    }

    void Update()
    {
        // 단축키 입력 처리
        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i].slotButton != null && quickSlots[i].hotkey != KeyCode.None && Input.GetKeyDown(quickSlots[i].hotkey))
            {
                // 해당 슬롯에 아이템이 있고, 사용 가능할 때만 클릭 이벤트 트리거
                if (i < inventorySystem.items.Count) // 아이템이 슬롯에 할당되어 있을 때
                {
                    OnQuickSlotClicked(i);
                }
            }
        }
    }

    public void RefreshQuickSlotsUI()
    {
        // 모든 슬롯 초기화 (빈 상태로 만듦)
        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i].itemIcon != null)
            {
                quickSlots[i].itemIcon.sprite = null; // 아이콘 제거
                quickSlots[i].itemIcon.color = new Color(1, 1, 1, 0); // 투명하게 만듦 (아이콘 숨김)
            }
            if (quickSlots[i].itemCountText != null)
            {
                quickSlots[i].itemCountText.text = ""; // 개수 텍스트 제거
            }
            if (quickSlots[i].slotButton != null)
            {
                quickSlots[i].slotButton.interactable = false; // 기본적으로 비활성화
            }
        }

        // 현재 인벤토리 아이템 목록을 기반으로 퀵 슬롯 채우기
        // (현재 inventorySystem.items는 모든 획득 아이템을 가지고 있으므로, 퀵 슬롯에 어떤 아이템을 배치할지 결정하는 로직이 필요함)
        // 여기서는 인벤토리의 첫 번째, 두 번째 아이템을 퀵 슬롯에 할당한다고 가정합니다.
        for (int i = 0; i < inventorySystem.items.Count && i < quickSlots.Length; i++)
        {
            InventoryItem invItem = inventorySystem.items[i];
            PlusTimeItemData itemData = inventorySystem.GetItemDataByName(invItem.itemName);

            if (itemData != null)
            {
                if (quickSlots[i].itemIcon != null && itemData.itemIcon != null) // itemData에 아이콘 스프라이트가 있다면
                {
                    quickSlots[i].itemIcon.sprite = itemData.itemIcon;
                    quickSlots[i].itemIcon.color = new Color(1, 1, 1, 1); // 불투명하게 만듦 (아이콘 표시)
                }
                if (quickSlots[i].itemCountText != null)
                {
                    quickSlots[i].itemCountText.text = invItem.count.ToString(); // 아이템 개수 표시
                }
                if (quickSlots[i].slotButton != null)
                {
                    quickSlots[i].slotButton.interactable = true; // 아이템이 있으면 활성화
                }
            }
        }
    }

    // 퀵 슬롯 클릭 시 호출
    void OnQuickSlotClicked(int slotIndex)
    {
        // 해당 슬롯에 아이템이 있는지 확인
        if (slotIndex >= 0 && slotIndex < inventorySystem.items.Count)
        {
            InventoryItem invItemToUse = inventorySystem.items[slotIndex];
            PlusTimeItemData itemData = inventorySystem.GetItemDataByName(invItemToUse.itemName);

            if (itemData != null)
            {
                UseItemEffect(itemData); // 아이템 효과 적용

                // 아이템 사용 후 인벤토리에서 제거 (만약 사용하면 사라지는 아이템이라면)
                // 현재 count가 항상 1인 ItemSystem이므로, RemoveAll 사용
                inventorySystem.items.RemoveAll(i => i.itemName == itemData.itemName);

                inventorySystem.SaveInventory(); // 변경 사항 저장
                RefreshQuickSlotsUI(); // UI 갱신 (제거된 아이템 반영)
            }
            else
            {
                Debug.LogWarning($"슬롯 {slotIndex + 1}에 할당된 '{invItemToUse.itemName}'의 데이터가 없습니다.");
            }
        }
        else
        {
            Debug.Log("빈 퀵 슬롯입니다.");
        }
    }

    // 실제 아이템 효과를 적용하는 함수 (여기에 게임 로직 추가)
    private void UseItemEffect(PlusTimeItemData itemData)
    {
        // TODO: 여기서 실제로 아이템 효과를 적용하는 로직을 구현합니다.
        // 예: 플레이어의 시간을 추가하는 함수 호출
        Debug.Log($"아이템 {itemData.itemName} 사용! (효과: {itemData.timeToAdd}초 추가)");

        // 예시: TimeManager 같은 곳에 시간을 추가하는 함수 호출
        // FindObjectOfType<TimeManager>()?.AddTime(itemData.timeToAdd);
        // 또는 특정 플레이어 스크립트에 함수 호출
        // FindObjectOfType<PlayerController>()?.ApplyTimeEffect(itemData.timeToAdd);
    }
}