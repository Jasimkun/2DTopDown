using UnityEngine;
using UnityEngine.UI; // UI 관련 클래스를 사용하기 위해 추가
using TMPro; // TextMeshPro를 사용한다면 추가 (사용하지 않는다면 제거)

/// <summary>
/// 개별 퀵 슬롯 UI 버튼을 제어하는 스크립트입니다.
/// 아이템 아이콘, 수량, 버튼 클릭 이벤트를 관리합니다.
/// 이 스크립트는 퀵 슬롯의 각 UI 버튼 GameObject에 연결되어야 합니다.
/// </summary>
public class QuickSlotButton : MonoBehaviour
{
    [Tooltip("아이템 아이콘을 표시할 Image 컴포넌트입니다. 이 Image 컴포넌트가 활성화되어 있고, Color의 Alpha 값이 0이 아닌지 확인하세요.")]
    public Image itemIcon; // 아이템 아이콘 표시 Image

    [Tooltip("아이템 수량을 표시할 Text 또는 TextMeshProUGUI 컴포넌트입니다.")]
    public TextMeshProUGUI quantityText; // 아이템 수량 표시 Text (TMPro 사용 시)
    // public Text quantityText; // 기본 Text 사용 시 (TMPro를 사용하지 않는다면 위 줄은 주석 처리하고 이 줄을 사용)

    // 이 슬롯에 현재 할당된 아이템 데이터 (사용을 위해 저장)
    private BaseItemData _currentItemData;
    // 이 슬롯에 현재 할당된 아이템 수량 (UI 업데이트용)
    private int _currentQuantity;

    // 현재 퀵 슬롯의 인덱스를 저장하기 위한 변수 (QuickSlotUIController에서 설정)
    private int _slotIndex;

    /// <summary>
    /// 이 슬롯의 인덱스를 설정합니다.
    /// </summary>
    /// <param name="index">이 퀵 슬롯 버튼의 인덱스입니다.</param>
    public void SetSlotIndex(int index)
    {
        _slotIndex = index;
    }

    /// <summary>
    /// 스크립트가 로드될 때 호출됩니다. 버튼 클릭 리스너를 추가합니다.
    /// </summary>
    private void Awake()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnQuickSlotClick); // 버튼 클릭 시 OnQuickSlotClick 메서드 호출
        }
        else
        {
            Debug.LogWarning($"QuickSlotButton 스크립트가 붙은 {gameObject.name}에 Button 컴포넌트가 없습니다.");
        }
        ClearQuickSlot(); // 초기화 시 슬롯을 비움 (아이콘 숨김)
    }

    /// <summary>
    /// 이 퀵 슬롯에 아이템을 설정하고 UI를 업데이트합니다.
    /// BaseItemData에 itemIcon이 할당되어 있어야 아이콘이 보입니다.
    /// </summary>
    /// <param name="data">이 슬롯에 할당할 BaseItemData 객체입니다.</param>
    /// <param name="quantity">이 슬롯에 할당할 아이템의 수량입니다.</param>
    public void SetQuickSlotItem(BaseItemData data, int quantity)
    {
        _currentItemData = data;
        _currentQuantity = quantity;

        if (_currentItemData != null)
        {
            // 아이콘 Image 컴포넌트가 할당되어 있고, BaseItemData에 아이콘 스프라이트가 할당되어 있다면
            if (itemIcon != null && _currentItemData.icon != null)
            {
                itemIcon.sprite = _currentItemData.icon; // BaseItemData의 아이콘 사용
                itemIcon.enabled = true; // 아이콘 Image 컴포넌트 활성화
                // Debug.Log($"아이콘 설정: {itemIcon.sprite.name} for {_currentItemData.itemName}");
            }
            else
            {
                // 아이콘 Image 컴포넌트가 없거나, BaseItemData에 아이콘이 할당되지 않은 경우
                if (itemIcon != null) itemIcon.enabled = false; // 아이콘 비활성화
                Debug.LogWarning($"퀵 슬롯 {_slotIndex}: 아이콘 Image 컴포넌트가 없거나, {_currentItemData.itemName}의 itemIcon이 할당되지 않았습니다. 아이콘이 보이지 않을 수 있습니다.");
            }

            if (quantityText != null)
            {
                // 수량이 1보다 크면 표시, 아니면 숨김
                quantityText.text = (quantity > 1) ? quantity.ToString() : "";
                quantityText.enabled = (quantity > 1); // 수량 텍스트 활성화/비활성화
            }
        }
        else
        {
            ClearQuickSlot(); // 데이터가 null이면 슬롯 비움
        }
    }

    /// <summary>
    /// 이 퀵 슬롯을 비우고 UI를 초기화합니다.
    /// 아이콘 Image와 수량 Text를 비활성화합니다.
    /// </summary>
    public void ClearQuickSlot()
    {
        _currentItemData = null;
        _currentQuantity = 0;

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false; // 아이콘 비활성화
        }
        if (quantityText != null)
        {
            quantityText.text = "";
            quantityText.enabled = false; // 수량 텍스트 비활성화
        }
    }

    /// <summary>
    /// 퀵 슬롯 버튼이 클릭되었을 때 호출됩니다.
    /// </summary>
    private void OnQuickSlotClick()
    {
        // Debug.Log($"퀵 슬롯 {_slotIndex} 클릭됨.");

        if (_currentItemData != null && _currentQuantity > 0)
        {
            Debug.Log($"퀵 슬롯 {_slotIndex}의 아이템 [{_currentItemData.itemName}] 사용 시도. 현재 수량: {_currentQuantity}");

            // 여기에 실제 아이템 사용 로직을 추가합니다.
            // 예를 들어, InventorySystem에 아이템 사용을 요청하고, 사용이 성공하면 수량을 업데이트합니다.
            // 이 로직은 InventorySystem 또는 다른 게임 관리 스크립트에서 처리하는 것이 일반적입니다.

            // 예시: InventorySystem에 아이템 사용 요청 (InventorySystem에 UseItem() 메서드가 있다고 가정)
            // if (InventorySystem.Instance != null)
            // {
            //     bool success = InventorySystem.Instance.UseItem(_currentItemData, 1);
            //     if (success)
            //     {
            //         // UI 업데이트를 위해 QuickSlotUIController에 갱신 요청
            //         // QuickSlotUIController가 싱글톤이거나 쉽게 접근 가능해야 함
            //         // 또는 이벤트 시스템을 사용하여 UI 업데이트를 알릴 수 있음
            //         // FindObjectOfType<QuickSlotUIController>()?.RefreshQuickSlotsUI();
            //     }
            // }

            // 중요한: 아이템 사용 후 InventorySystem의 데이터를 변경하고,
            // QuickSlotUIController의 RefreshQuickSlotsUI()를 호출하여 UI를 갱신해야 합니다.
            // 일반적으로 Item을 사용하면 해당 Item의 Effect를 발동하고, InventorySystem에서 Item을 제거해야 합니다.
        }
        else
        {
            Debug.Log($"퀵 슬롯 {_slotIndex}이 비어있거나 아이템 수량이 부족하여 사용할 수 없습니다.");
            // 비어있는 슬롯을 클릭했을 때의 추가 로직 (예: 아이템 할당 UI 팝업)
        }
    }
}
