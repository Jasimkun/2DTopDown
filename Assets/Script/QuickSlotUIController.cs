using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하므로 필요합니다.

public class QuickSlotUIController : MonoBehaviour
{
    // 퀵 슬롯 UI 요소들 (버튼, 이미지, 텍스트)을 연결할 배열
    public QuickSlotUI[] quickSlots;

    // 아이템이 없을 때 표시할 기본 스프라이트 (선택 사항)
    public Sprite defaultSlotSprite;

    void Start()
    {
        // 시작 시 퀵 슬롯 UI를 한 번 갱신합니다.
        RefreshQuickSlotsUI();
    }

    // 퀵 슬롯 UI를 인벤토리 상태에 따라 갱신하는 메서드
    public void RefreshQuickSlotsUI()
    {
        if (InventorySystem.Instance == null)
        {
            Debug.LogWarning("InventorySystem.Instance가 없습니다. 퀵 슬롯 UI를 갱신할 수 없습니다.");
            return;
        }

        List<InventoryItem> inventoryItems = InventorySystem.Instance.items;
        Debug.Log($"[QuickSlotUI] 인벤토리 시스템에 있는 아이템 개수: {inventoryItems.Count}");

        for (int i = 0; i < quickSlots.Length; i++)
        {
            QuickSlotUI currentSlot = quickSlots[i];

            // UI 요소들이 할당되었는지 확인합니다.
            if (currentSlot.currentImage == null || currentSlot.currentText == null || currentSlot.currentButton == null)
            {
                Debug.LogWarning($"퀵 슬롯 {i}의 버튼/이미지/텍스트 컴포넌트가 누락되었습니다. Inspector에서 할당해주세요!");
                continue;
            }

            if (i < inventoryItems.Count) // 인벤토리 아이템 개수보다 작으면 (즉, 해당 슬롯에 아이템이 있다면)
            {
                InventoryItem invItem = inventoryItems[i];
                BaseItemData itemData = InventorySystem.Instance.GetItemDataByName(invItem.itemName);

                //Debug.Log($"[QuickSlotUI] 슬롯 {i}: 아이템 이름 '{invItem.itemName}'. 아이템 데이터 존재 여부: {(itemData != null ? "있음" : "없음")}");

                if (itemData != null)
                {
                    // ⬇️ [수정] 아이콘이 null이면 defaultSlotSprite를 먼저 할당하도록 로직 변경
                    currentSlot.currentImage.sprite = itemData.icon;
                    if (currentSlot.currentImage.sprite == null) // 할당된 아이콘이 없다면 기본 스프라이트 사용
                    {
                        currentSlot.currentImage.sprite = defaultSlotSprite;
                        Debug.LogWarning($"아이템 '{itemData.itemName}'의 아이콘이 null입니다. 기본 슬롯 스프라이트를 사용합니다.");
                    }
                    currentSlot.currentImage.color = Color.white; // 기본적으로 하얀색 (사용됨 표시를 위해)

                    if (invItem.usedInCurrentScene)
                    {
                        currentSlot.currentImage.color = Color.gray; // 회색으로 표시
                        currentSlot.currentText.text = "USED"; // 사용됨 표시
                        currentSlot.currentButton.interactable = false; // 버튼 비활성화
                        Debug.Log($"[QuickSlotUI] 아이템 '{invItem.itemName}'은(는) 이번 씬에서 이미 사용됨.");
                    }
                    else
                    {
                        currentSlot.currentImage.color = Color.white; // 정상 색상
                        currentSlot.currentText.text = invItem.count.ToString(); // 개수 표시
                        currentSlot.currentButton.interactable = true; // 버튼 활성화
                    }
                }
                else // 아이템 데이터가 null일 경우 (InventorySystem의 allItemData에 할당되지 않았거나 이름이 잘못된 경우)
                {
                    currentSlot.currentImage.sprite = defaultSlotSprite;
                    currentSlot.currentImage.color = Color.gray;
                    currentSlot.currentText.text = "N/A"; // Not Available
                    currentSlot.currentButton.interactable = false;
                    Debug.LogWarning($"인벤토리 아이템 '{invItem.itemName}'에 해당하는 BaseItemData를 InventorySystem의 allItemData에서 찾을 수 없습니다. 아이콘과 이름 확인 필요.");
                }
            }
            else // 인벤토리 아이템 개수보다 크면 (즉, 빈 슬롯이라면)
            {
                currentSlot.currentImage.sprite = defaultSlotSprite; // 기본 스프라이트 (빈 슬롯이므로)
                currentSlot.currentImage.color = Color.white; // 정상 색상
                currentSlot.currentText.text = ""; // 텍스트 비움
                currentSlot.currentButton.interactable = false; // 버튼 비활성화
            }
        }
    }

    [System.Serializable]
    public class QuickSlotUI
    {
        public Button currentButton;
        public Image currentImage;
        public TextMeshProUGUI currentText;
    }
}
