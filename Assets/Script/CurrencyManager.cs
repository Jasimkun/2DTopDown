using UnityEngine;
using System; // Required for Action
using System.Collections; // Required for IEnumerator

// This script manages all in-game currencies (e.g., Gold, Gems).
// It uses the Singleton pattern to ensure only one instance exists and is easily accessible.
// It also provides events for other scripts to subscribe to when currency values change.
public class CurrencyManager : MonoBehaviour
{
    // Singleton instance
    public static CurrencyManager Instance { get; private set; }

    // Event that broadcasts the new gold amount whenever it changes.
    // Other scripts can subscribe to this event to update their UI.
    public event Action<int> OnGoldChanged;

    // Current amount of gold
    private int _currentGold = 0; // Initialize with 0 gold

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Crucial: This ensures the CurrencyManager persists across scene changes.
            Debug.Log("[CurrencyManager] CurrencyManager initialized.");
            Debug.Log($"[CurrencyManager] ÇöÀç °ñµå: {_currentGold}"); // Initial gold display
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Method to add gold
    public void AddGold(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("[CurrencyManager] Attempted to add negative gold amount. Use SpendGold for spending.");
            return;
        }

        _currentGold += amount;
        Debug.Log($"[CurrencyManager] °ñµå {amount}°³ Ãß°¡µÊ. ÇöÀç °ñµå: {_currentGold}");
        OnGoldChanged?.Invoke(_currentGold); // Invoke the event with the new gold amount
    }

    // Method to spend gold
    public bool SpendGold(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("[CurrencyManager] Attempted to spend negative gold amount.");
            return false;
        }

        if (_currentGold >= amount)
        {
            _currentGold -= amount;
            Debug.Log($"[CurrencyManager] °ñµå {amount}°³ ¼Ò¸ðµÊ. ÇöÀç °ñµå: {_currentGold}");
            OnGoldChanged?.Invoke(_currentGold); // Invoke the event with the new gold amount
            return true;
        }
        else
        {
            Debug.Log("[CurrencyManager] °ñµå°¡ ºÎÁ·ÇÏ¿© ¼Ò¸ðÇÒ ¼ö ¾ø½À´Ï´Ù.");
            return false;
        }
    }

    // Method to check if the player can afford a certain amount
    public bool CanAfford(int amount)
    {
        return _currentGold >= amount;
    }

    // Method to get the current gold amount
    public int GetGold()
    {
        return _currentGold;
    }
}
