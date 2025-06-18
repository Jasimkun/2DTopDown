// QuickSlotUIController.cs

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용한다면 필요

public class QuickSlotUIController : MonoBehaviour
{
    [Header("UI References")]
    public Button[] quickSlotButtons; // 각 퀵 슬롯의 Button 컴포넌트 추가
    public Image[] quickSlotImages;
    public TextMeshProUGUI[] quickSlotCountTexts;

    [Header("Item Sprites")]
    public Sprite defaultSlotSprite;

    private InventorySystem inventorySystem;

    void Awake()
    {
        inventorySystem = FindObjectOfType<InventorySystem>();
        if (inventorySystem == null)
        {
            Debug.LogError("씬에 InventorySystem이 없습니다. 퀵 슬롯 UI를 갱신할 수 없습니다.");
        }

        // --- 추가: 각 버튼에 클릭 이벤트 리스너 할당 ---
        for (int i = 0; i < quickSlotButtons.Length; i++)
        {
            int slotIndex = i; // 클로저 문제 방지를 위해 로컬 변수 사용
            quickSlotButtons[i].onClick.AddListener(() => OnQuickSlotButtonClick(slotIndex));
            // 초기에는 버튼 비활성화 (아이템이 없으면 클릭 불가)
            quickSlotButtons[i].interactable = false;
        }
        // --- 추가 끝 ---

        RefreshQuickSlotsUI(); // Awake에서 초기 UI 갱신
    }

    // 퀵 슬롯 버튼 클릭 시 호출될 메서드
    private void OnQuickSlotButtonClick(int slotIndex)
    {
        if (inventorySystem != null)
        {
            inventorySystem.UseInventoryItem(slotIndex);
        }
    }


    public void RefreshQuickSlotsUI()
    {
        if (inventorySystem == null)
        {
            Debug.LogWarning("InventorySystem이 초기화되지 않아 퀵 슬롯 UI를 갱신할 수 없습니다.");
            return;
        }

        // 퀵 슬롯 개수만큼 반복하며 UI 업데이트
        for (int i = 0; i < quickSlotImages.Length; i++)
        {
            // --- 추가: 버튼 참조 확인 ---
            Button currentButton = (i < quickSlotButtons.Length) ? quickSlotButtons[i] : null;
            Image currentImage = quickSlotImages[i];
            TextMeshProUGUI currentText = quickSlotCountTexts[i];

            if (currentButton == null || currentImage == null || currentText == null)
            {
                Debug.LogWarning($"퀵 슬롯 UI 설정 오류: {i}번 슬롯의 버튼/이미지/텍스트 컴포넌트가 누락되었습니다.");
                continue; // 다음 슬롯으로 넘어감
            }
            // --- 추가 끝 ---


            if (i < inventorySystem.items.Count) // 인벤토리 아이템이 슬롯 개수보다 적으면
            {
                InventoryItem invItem = inventorySystem.items[i];
                PlusTimeItemData itemData = inventorySystem.GetItemDataByName(invItem.itemName);

                if (itemData != null)
                {
                    currentImage.sprite = itemData.icon; // PlusTimeItemData에 icon 필드가 있다고 가정
                    currentImage.color = Color.white;
                    //currentText.text = invItem.count.ToString();
                    currentButton.interactable = true; // 아이템이 있으면 버튼 활성화
                }
                else
                {
                    currentImage.sprite = defaultSlotSprite;
                    currentImage.color = new Color(1, 1, 1, 0.5f);
                    currentText.text = "";
                    currentButton.interactable = false; // 아이템 데이터 없으면 비활성화
                    Debug.LogWarning($"아이템 '{invItem.itemName}'에 대한 PlusTimeItemData를 찾을 수 없습니다.");
                }
            }
            else // 해당 슬롯에 표시할 인벤토리 아이템이 없는 경우
            {
                currentImage.sprite = defaultSlotSprite;
                currentImage.color = new Color(1, 1, 1, 0.5f);
                currentText.text = "";
                currentButton.interactable = false; // 아이템 없으면 버튼 비활성화
            }
        }
        //Debug.Log("퀵 슬롯 UI가 갱신되었습니다.");
    }
}