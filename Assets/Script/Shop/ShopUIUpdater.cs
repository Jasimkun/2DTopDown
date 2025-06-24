using UnityEngine;
using TMPro; // Required for TextMeshProUGUI

// This script is responsible for updating the gold display in the shop UI.
// It subscribes to the OnGoldChanged event from CurrencyManager to react to real-time changes.
// It also provides a public method to manually refresh the gold display when needed (e.g., when shop panel opens).
public class ShopUIUpdater : MonoBehaviour
{
    // Reference to the TextMeshProUGUI component that displays the gold amount.
    [Tooltip("현재 골드량을 표시할 TextMeshProUGUI 컴포넌트를 할당하세요.")]
    [SerializeField] private TextMeshProUGUI _goldDisplayTxt;

    private void OnEnable()
    {
        // Ensure CurrencyManager exists before subscribing.
        if (CurrencyManager.Instance != null)
        {
            // Subscribe to the OnGoldChanged event.
            // When gold changes, UpdateGoldDisplay will be called.
            CurrencyManager.Instance.OnGoldChanged += UpdateGoldDisplayInternal; // 메서드 이름 변경 (내부용)
            // Also update the display immediately when this script becomes active (e.g., when scene loads or object is enabled)
            RefreshGoldDisplay(); // OnEnable 시점에 초기 업데이트를 public 메서드를 통해 수행
            Debug.Log("[ShopUIUpdater] OnGoldChanged 이벤트 구독 완료 및 초기 골드 업데이트.");
        }
        else
        {
            Debug.LogError("[ShopUIUpdater] CurrencyManager 인스턴스를 찾을 수 없습니다. 골드 UI 업데이트 불가.");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnGoldChanged event to prevent memory leaks.
        // It's crucial to unsubscribe when the object is disabled or destroyed.
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGoldChanged -= UpdateGoldDisplayInternal; // 메서드 이름 변경 (내부용)
            Debug.Log("[ShopUIUpdater] OnGoldChanged 이벤트 구독 해제 완료.");
        }
    }

    // This internal method is called when the OnGoldChanged event is triggered.
    private void UpdateGoldDisplayInternal(int newGoldAmount)
    {
        UpdateGoldDisplayUI(newGoldAmount);
        Debug.Log($"[ShopUIUpdater] 내부 이벤트로 상점 UI 골드 업데이트: {newGoldAmount}");
    }

    // Public method to manually refresh the gold display.
    // This can be called when the shop panel is opened or becomes visible.
    public void RefreshGoldDisplay()
    {
        if (CurrencyManager.Instance != null)
        {
            int currentGold = CurrencyManager.Instance.GetGold();
            UpdateGoldDisplayUI(currentGold);
            Debug.Log($"[ShopUIUpdater] 외부 호출로 상점 UI 골드 강제 새로고침: {currentGold}");
        }
        else
        {
            Debug.LogError("[ShopUIUpdater] CurrencyManager 인스턴스를 찾을 수 없어 골드를 새로고침할 수 없습니다.");
        }
    }

    // A private helper method to update the TextMeshProUGUI component.
    private void UpdateGoldDisplayUI(int goldAmount)
    {
        if (_goldDisplayTxt != null)
        {
            _goldDisplayTxt.text = $"골드: {goldAmount}";
        }
        else
        {
            Debug.LogWarning("[ShopUIUpdater] 골드 표시 TextMeshProUGUI 컴포넌트가 할당되지 않았습니다! Inspector에서 할당해주세요.");
        }
    }
}
