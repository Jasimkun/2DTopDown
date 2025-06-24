using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickSlotUIController : MonoBehaviour
{
    public QuickSlotUI[] quickSlots;
    public Sprite defaultSlotSprite; // ⬇️ 이 필드에 스프라이트가 할당되어야 합니다.

    void Start()
    {
        Debug.Log("[QuickSlotUI] Start 메서드 시작. UI 갱신.");
        RefreshQuickSlotsUI();
    }

    public void RefreshQuickSlotsUI()
    {
        Debug.Log("[QuickSlotUI] RefreshQuickSlotsUI 메서드 시작.");

        if (InventorySystem.Instance == null)
        {
            Debug.LogWarning("InventorySystem.Instance가 없습니다. 퀵 슬롯 UI를 갱신할 수 없습니다.");
            return;
        }

        List<InventoryItem> inventoryItems = InventorySystem.Instance.items;
        Debug.Log($"[QuickSlotUI] 현재 인벤토리 아이템 개수: {inventoryItems.Count}");

        for (int i = 0; i < quickSlots.Length; i++)
        {
            QuickSlotUI currentSlot = quickSlots[i];
            Debug.Log($"\n[QuickSlotUI] --- 슬롯 {i} 처리 중 ---");

            // UI 요소 할당 확인 (기존 로직)
            if (currentSlot.currentImage == null || currentSlot.currentText == null || currentSlot.currentButton == null)
            {
                Debug.LogError($"[QuickSlotUI] 오류: 퀵 슬롯 {i}의 버튼/이미지/텍스트 컴포넌트 중 하나가 누락되었습니다. Inspector에서 할당해주세요!");
                if (currentSlot.currentImage != null) currentSlot.currentImage.sprite = defaultSlotSprite;
                if (currentSlot.currentImage != null) currentSlot.currentImage.color = Color.gray;
                if (currentSlot.currentText != null) currentSlot.currentText.text = "오류";
                if (currentSlot.currentButton != null) currentSlot.currentButton.interactable = false;
                continue;
            }

            Debug.Log($"[QuickSlotUI] 슬롯 {i} - Button Instance ID: {currentSlot.currentButton.GetInstanceID()}");
            Debug.Log($"[QuickSlotUI] 슬롯 {i} - Image Instance ID: {currentSlot.currentImage.GetInstanceID()}");
            Debug.Log($"[QuickSlotUI] 슬롯 {i} - Text Instance ID: {currentSlot.currentText.GetInstanceID()}");

            currentSlot.currentButton.onClick.RemoveAllListeners();

            BaseItemData requiredItemData = currentSlot.requiredItemData;

            if (requiredItemData == null)
            {
                Debug.LogWarning($"[QuickSlotUI] 슬롯 {i}에 'Required Item Data'가 할당되지 않았습니다. 빈 슬롯으로 처리합니다.");
                currentSlot.currentImage.sprite = defaultSlotSprite;
                currentSlot.currentImage.color = Color.gray;
                currentSlot.currentText.text = "";
                currentSlot.currentButton.interactable = false;
                continue;
            }

            InventoryItem invItem = inventoryItems.Find(item => item.itemName == requiredItemData.itemName);

            if (invItem != null)
            {
                Debug.Log($"[QuickSlotUI] 슬롯 {i}: 인벤토리 아이템 '{invItem.itemName}' (Required: '{requiredItemData.itemName}'). Used In Scene: {invItem.usedInCurrentScene}");

                currentSlot.currentImage.sprite = requiredItemData.icon; // ⬇️ 여기에서 아이템의 아이콘을 할당합니다.

                // ⬇️ [핵심] 아이콘이 없거나 defaultSprite도 없을 때 투명하게 만드는 로직
                if (currentSlot.currentImage.sprite == null)
                {
                    Debug.LogWarning($"[QuickSlotUI] 아이템 '{requiredItemData.itemName}'의 아이콘이 null입니다! defaultSlotSprite 사용 시도.");
                    currentSlot.currentImage.sprite = defaultSlotSprite; // defaultSlotSprite 할당 시도
                }

                if (currentSlot.currentImage.sprite == null) // ⬇️ 여전히 null이면 심각한 오류, 투명하게 만듭니다.
                {
                    Debug.LogError($"[QuickSlotUI] 심각: 아이템 '{requiredItemData.itemName}'의 아이콘과 defaultSlotSprite 모두 null입니다. 이미지가 표시될 수 없습니다.");
                    currentSlot.currentImage.color = Color.clear; // 이미지를 투명하게 만들어 사라지게 함
                }
                else // ⬇️ 스프라이트가 성공적으로 할당되었다면 정상적인 색상 로직 적용
                {
                    currentSlot.currentImage.color = Color.white; // 기본적으로 하얀색 (투명하지 않음)
                }


                if (invItem.usedInCurrentScene)
                {
                    currentSlot.currentImage.color = Color.gray; // ⬇️ 사용 시 회색으로 변경
                    currentSlot.currentText.text = "USED";
                    currentSlot.currentButton.interactable = false;
                    Debug.Log($"[QuickSlotUI] 슬롯 {i} ({requiredItemData.itemName}): 이번 씬에서 사용됨. UI 회색, 텍스트 'USED', 버튼 비활성화.");
                }
                else
                {
                    currentSlot.currentImage.color = Color.white; // ⬇️ 사용하지 않았으면 흰색 유지
                    currentSlot.currentText.text = invItem.count.ToString();
                    currentSlot.currentButton.interactable = true;

                    int actualInventoryIndex = InventorySystem.Instance.items.FindIndex(item => item.itemName == requiredItemData.itemName);
                    if (actualInventoryIndex != -1)
                    {
                        currentSlot.currentButton.onClick.AddListener(() => InventorySystem.Instance.UseInventoryItem(actualInventoryIndex));
                        Debug.Log($"[QuickSlotUI] 슬롯 {i} ({requiredItemData.itemName}): UseInventoryItem({actualInventoryIndex}) 리스너 추가됨.");
                    }
                    else
                    {
                        Debug.LogWarning($"[QuickSlotUI] 슬롯 {i} ({requiredItemData.itemName}): 인벤토리에서 아이템의 실제 인덱스를 찾을 수 없습니다! 버튼 비활성화.");
                        currentSlot.currentButton.interactable = false;
                    }
                }
                Debug.Log($"[QuickSlotUI] 슬롯 {i} ({requiredItemData.itemName}) 최종 스프라이트: {currentSlot.currentImage.sprite?.name ?? "없음"}, 최종 색상: {currentSlot.currentImage.color}, 최종 텍스트: '{currentSlot.currentText.text}', 버튼 인터랙터블: {currentSlot.currentButton.interactable}");

            }
            else // 인벤토리에 이 아이템이 없다면 (빈 슬롯 처리)
            {
                currentSlot.currentImage.sprite = defaultSlotSprite;
                currentSlot.currentImage.color = Color.gray; // 없으면 회색 (투명하지 않음)
                currentSlot.currentText.text = "";
                currentSlot.currentButton.interactable = false;
                Debug.Log($"[QuickSlotUI] 슬롯 {i} ({requiredItemData.itemName}): 인벤토리에 없음. 빈 슬롯으로 설정됨.");
            }
        }
        Debug.Log("[QuickSlotUI] RefreshQuickSlotsUI 메서드 완료.");
    }

    [System.Serializable]
    public class QuickSlotUI
    {
        public Button currentButton;
        public Image currentImage;
        public TextMeshProUGUI currentText;
        [Tooltip("이 퀵슬롯에 고정적으로 표시될 BaseItemData 에셋을 할당하세요. (예: PlusTimeItemData)")]
        public BaseItemData requiredItemData;
    }
}
