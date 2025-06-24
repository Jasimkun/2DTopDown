using UnityEngine;
using TMPro; // Required for TextMeshProUGUI

// This script is responsible for updating the gold display in the shop UI.
// It subscribes to the OnGoldChanged event from CurrencyManager to react to real-time changes.
// It also provides a public method to manually refresh the gold display when needed (e.g., when shop panel opens).
public class ShopUIUpdater : MonoBehaviour
{
    // Reference to the TextMeshProUGUI component that displays the gold amount.
    [Tooltip("���� ��差�� ǥ���� TextMeshProUGUI ������Ʈ�� �Ҵ��ϼ���.")]
    [SerializeField] private TextMeshProUGUI _goldDisplayTxt;

    private void OnEnable()
    {
        // Ensure CurrencyManager exists before subscribing.
        if (CurrencyManager.Instance != null)
        {
            // Subscribe to the OnGoldChanged event.
            // When gold changes, UpdateGoldDisplay will be called.
            CurrencyManager.Instance.OnGoldChanged += UpdateGoldDisplayInternal; // �޼��� �̸� ���� (���ο�)
            // Also update the display immediately when this script becomes active (e.g., when scene loads or object is enabled)
            RefreshGoldDisplay(); // OnEnable ������ �ʱ� ������Ʈ�� public �޼��带 ���� ����
            Debug.Log("[ShopUIUpdater] OnGoldChanged �̺�Ʈ ���� �Ϸ� �� �ʱ� ��� ������Ʈ.");
        }
        else
        {
            Debug.LogError("[ShopUIUpdater] CurrencyManager �ν��Ͻ��� ã�� �� �����ϴ�. ��� UI ������Ʈ �Ұ�.");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnGoldChanged event to prevent memory leaks.
        // It's crucial to unsubscribe when the object is disabled or destroyed.
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGoldChanged -= UpdateGoldDisplayInternal; // �޼��� �̸� ���� (���ο�)
            Debug.Log("[ShopUIUpdater] OnGoldChanged �̺�Ʈ ���� ���� �Ϸ�.");
        }
    }

    // This internal method is called when the OnGoldChanged event is triggered.
    private void UpdateGoldDisplayInternal(int newGoldAmount)
    {
        UpdateGoldDisplayUI(newGoldAmount);
        Debug.Log($"[ShopUIUpdater] ���� �̺�Ʈ�� ���� UI ��� ������Ʈ: {newGoldAmount}");
    }

    // Public method to manually refresh the gold display.
    // This can be called when the shop panel is opened or becomes visible.
    public void RefreshGoldDisplay()
    {
        if (CurrencyManager.Instance != null)
        {
            int currentGold = CurrencyManager.Instance.GetGold();
            UpdateGoldDisplayUI(currentGold);
            Debug.Log($"[ShopUIUpdater] �ܺ� ȣ��� ���� UI ��� ���� ���ΰ�ħ: {currentGold}");
        }
        else
        {
            Debug.LogError("[ShopUIUpdater] CurrencyManager �ν��Ͻ��� ã�� �� ���� ��带 ���ΰ�ħ�� �� �����ϴ�.");
        }
    }

    // A private helper method to update the TextMeshProUGUI component.
    private void UpdateGoldDisplayUI(int goldAmount)
    {
        if (_goldDisplayTxt != null)
        {
            _goldDisplayTxt.text = $"���: {goldAmount}";
        }
        else
        {
            Debug.LogWarning("[ShopUIUpdater] ��� ǥ�� TextMeshProUGUI ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�! Inspector���� �Ҵ����ּ���.");
        }
    }
}
