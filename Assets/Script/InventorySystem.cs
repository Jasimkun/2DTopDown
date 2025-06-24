using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
// System; 네임스페이스는 이제 필요 없으므로 제거합니다.

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int count;

    [System.NonSerialized]
    public bool usedInCurrentScene;

    public InventoryItem(string name)
    {
        itemName = name;
        count = 1;
        usedInCurrentScene = false;
    }
}

[System.Serializable]
public class InventorySaveData
{
    public List<InventoryItem> items;

    public InventorySaveData(List<InventoryItem> itemsToSave)
    {
        items = itemsToSave;
    }
}

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    public List<InventoryItem> items = new List<InventoryItem>();

    [Tooltip("모든 BaseItemData 에셋을 여기에 할당하세요. 상점 및 인벤토리에서 참조됩니다.")]
    public List<BaseItemData> allItemData;

    private string savePath;
    private QuickSlotUIController quickSlotUIController;
    private GameObject playerGameObject;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/inventory.json";
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadInventory();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var item in items)
        {
            item.usedInCurrentScene = false;
        }

        quickSlotUIController = FindObjectOfType<QuickSlotUIController>();
        if (quickSlotUIController != null)
        {
            quickSlotUIController.RefreshQuickSlotsUI();
        }
        else
        {
            Debug.LogWarning($"[InventorySystem] 씬 '{scene.name}'에서 QuickSlotUIController를 찾을 수 없습니다.");
        }

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            playerGameObject = foundPlayer;
        }
        else
        {
            Debug.LogWarning($"[InventorySystem] 씬 '{scene.name}'에서 'Player' 태그를 가진 GameObject를 찾을 수 없습니다. 아이템 효과 적용에 문제가 있을 수 있습니다.");
            playerGameObject = null;
        }
    }

    public void AddItem(BaseItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("추가하려는 아이템 데이터가 null입니다.", this);
            return;
        }

        InventoryItem existing = items.Find(i => i.itemName == itemData.itemName);
        if (existing != null)
        {
            Debug.Log($"이미 인벤토리에 존재하는 아이템 '{itemData.itemName}'입니다.");
            return;
        }
        else
        {
            items.Add(new InventoryItem(itemData.itemName));
            SaveInventory();
            Debug.Log($"인벤토리에 '{itemData.itemName}' 아이템을 저장했습니다.");

            // ⬇️ [제거] OnUseItem 델리게이트 연결 로직을 제거합니다. 이제 각 아이템 데이터가 자체적으로 효과를 처리합니다.
            // if (itemData is PlusTimeItemData plusTimeData) { ... }

            if (quickSlotUIController != null)
            {
                quickSlotUIController.RefreshQuickSlotsUI();
            }
        }
    }

    public void UseInventoryItem(int quickSlotIndex)
    {
        if (quickSlotIndex >= 0 && quickSlotIndex < items.Count)
        {
            InventoryItem invItem = items[quickSlotIndex];
            BaseItemData itemData = GetItemDataByName(invItem.itemName);

            if (invItem.usedInCurrentScene)
            {
                Debug.Log($"'{invItem.itemName}' 아이템은 맵마다 한 번씩만 사용할 수 있는 것 같아.");
                return;
            }

            if (itemData != null)
            {
                if (playerGameObject == null)
                {
                    playerGameObject = GameObject.FindGameObjectWithTag("Player");
                }

                // ⬇️ [수정] 아이템의 UseItemEffect 메서드를 직접 호출합니다.
                itemData.UseItemEffect(playerGameObject);

                invItem.usedInCurrentScene = true; // 아이템 사용 후 현재 씬에서 사용됨으로 표시

                if (quickSlotUIController != null)
                {
                    quickSlotUIController.RefreshQuickSlotsUI();
                }
            }
            else
            {
                Debug.LogWarning($"[인벤토리] 슬롯 {quickSlotIndex}의 아이템 데이터('{invItem.itemName}')를 찾을 수 없어 사용할 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"[인벤토리] 유효하지 않은 퀵 슬롯 인덱스: {quickSlotIndex} (현재 인벤토리 아이템 수: {items.Count})");
        }
    }

    public void SaveInventory()
    {
        string json = JsonUtility.ToJson(new InventorySaveData(items));
        File.WriteAllText(savePath, json);
        Debug.Log("인벤토리 데이터 저장됨.");
    }

    public void LoadInventory()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            InventorySaveData loadedData = JsonUtility.FromJson<InventorySaveData>(json);
            items = loadedData.items;
            if (items.Count == 0)
            {
                Debug.Log("저장된 인벤토리가 없는 것 같아.");
            }
            else
            {
                Debug.Log($"...이전의 인벤토리를 불러왔어.");
            }
        }
        else
        {
            Debug.Log("저장된 인벤토리가 없는 것 같아.");
        }

        // ⬇️ [제거] OnUseItem 델리게이트 연결 로직을 제거합니다. 이제 각 아이템 데이터가 자체적으로 효과를 처리합니다.
        // foreach (var invItem in items) { ... }
    }

    public BaseItemData GetItemDataByName(string itemName)
    {
        return allItemData.Find(x => x.itemName == itemName);
    }

    public void ResetInventoryData()
    {
        items.Clear();
        SaveInventory();
        Debug.Log("[InventorySystem] 인벤토리 데이터가 초기화되었습니다.");

        if (quickSlotUIController != null)
        {
            quickSlotUIController.RefreshQuickSlotsUI();
        }
    }
}
