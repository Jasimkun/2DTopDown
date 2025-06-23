using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int count; // (현재는 항상 1이지만, 확장 시 사용 가능)

    [System.NonSerialized]
    public bool usedInCurrentScene; // 이 씬에서 아이템이 사용되었는지 여부

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
    public List<BaseItemData> allItemData; // 모든 아이템 데이터 에셋들의 리스트

    private string savePath;
    private QuickSlotUIController quickSlotUIController;
    private GameObject playerGameObject;

    private void Awake()
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
        // 씬 로드 시 모든 아이템의 사용 기록 초기화 (로그라이크 씬 당 1회 사용)
        foreach (var item in items)
        {
            item.usedInCurrentScene = false;
        }

        // 새 씬이 로드될 때마다 필요한 컴포넌트들을 다시 찾아 연결합니다.
        quickSlotUIController = FindObjectOfType<QuickSlotUIController>();
        if (quickSlotUIController != null)
        {
            quickSlotUIController.RefreshQuickSlotsUI();
        }
        else
        {
            Debug.LogWarning($"[InventorySystem] 씬 '{scene.name}'에서 QuickSlotUIController를 찾을 수 없습니다.");
        }

        // 씬 로드 시 플레이어 GameObject를 다시 찾습니다. (효과 적용을 위해)
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

    /// <summary>
    /// 인벤토리에 아이템을 추가합니다. BaseItemData 타입을 받도록 일반화되었습니다.
    /// </summary>
    /// <param name="itemData">추가할 아이템 데이터 (BaseItemData 상속).</param>
    public void AddItem(BaseItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("추가하려는 아이템 데이터가 null입니다.", this);
            return;
        }

        // 인벤토리 아이템 목록에 이미 존재하는지 확인 (이름으로)
        InventoryItem existing = items.Find(i => i.itemName == itemData.itemName);
        if (existing != null)
        {
            Debug.Log($"이미 인벤토리에 존재하는 아이템 '{itemData.itemName}'입니다.");
            // 아이템 개수를 늘리는 로직이 필요하다면 여기에서 existing.count++; 를 구현합니다.
            return;
        }
        else
        {
            // 새로운 InventoryItem 인스턴스 생성 및 추가
            items.Add(new InventoryItem(itemData.itemName));
            SaveInventory();
            Debug.Log($"인벤토리에 '{itemData.itemName}' 아이템을 저장했습니다.");

            // PlusTimeItemData의 경우에만 OnUseItem 델리게이트를 연결합니다.
            // LightItemData는 UseItemEffect 내에서 직접 효과를 적용하므로 이 델리게이트가 필요 없습니다.
            if (itemData is PlusTimeItemData plusTimeData)
            {
                // 델리게이트는 아이템 사용 시 실제 효과를 GameTimer에 적용합니다.
                plusTimeData.OnUseItem = () =>
                {
                    // ShopManager에서 아이템의 현재 업그레이드 레벨을 가져옵니다.
                    int currentLevel = ShopManager.Instance != null ? ShopManager.Instance.GetItemUpgradeLevel(plusTimeData.itemName) : 0;
                    float timeBonus = plusTimeData.GetEffectiveTimeToAdd(currentLevel);

                    Debug.Log($"'{plusTimeData.itemName}' 아이템을 사용했어. 시간이 {timeBonus}초 추가된 것 같아. (레벨: {currentLevel})");
                    GameTimer timer = FindObjectOfType<GameTimer>();
                    if (timer != null)
                    {
                        timer.AddTime(timeBonus);
                    }
                    else
                    {
                        Debug.LogWarning("씬에서 'GameTimer' 스크립트를 찾을 수 없습니다. 시간을 추가할 수 없습니다.");
                    }
                    // 아이템 사용 후 제거 (필요하다면 RemoveItem(plusTimeData.itemName); 호출)
                };
            }

            if (quickSlotUIController != null)
            {
                quickSlotUIController.RefreshQuickSlotsUI();
            }
        }
    }

    /// <summary>
    /// 인벤토리에서 아이템을 사용합니다.
    /// </summary>
    /// <param name="quickSlotIndex">퀵 슬롯 UI의 인덱스입니다.</param>
    public void UseInventoryItem(int quickSlotIndex)
    {
        if (quickSlotIndex >= 0 && quickSlotIndex < items.Count)
        {
            InventoryItem invItem = items[quickSlotIndex];
            BaseItemData itemData = GetItemDataByName(invItem.itemName);

            // 현재 씬에서 이미 사용되었는지 확인 (로그라이크 씬 당 1회 사용 제한)
            if (invItem.usedInCurrentScene)
            {
                Debug.Log($"'{invItem.itemName}' 아이템은 맵마다 한 번씩만 사용할 수 있는 것 같아.");
                return;
            }

            if (itemData != null)
            {
                // 플레이어 GameObject가 null이면 다시 찾기 시도
                if (playerGameObject == null)
                {
                    playerGameObject = GameObject.FindGameObjectWithTag("Player");
                }

                // BaseItemData의 UseItemEffect 메서드를 호출하여 해당 아이템의 고유한 효과를 실행합니다.
                // LightItemData의 UseItemEffect는 여기서 playerGameObject를 직접 넘겨 ScaleEffectHandler를 호출합니다.
                // PlusTimeItemData의 UseItemEffect는 OnUseItem 델리게이트를 호출하고, 그 델리게이트에 시간이 추가되는 로직이 연결되어 있습니다.
                itemData.UseItemEffect(playerGameObject);

                invItem.usedInCurrentScene = true; // 아이템 사용 후 현재 씬에서 사용됨으로 표시

                // UI 갱신 (아이템 사용 상태 변화를 반영하기 위함)
                if (quickSlotUIController != null)
                {
                    quickSlotUIController.RefreshQuickSlotsUI();
                }

                // 사용 후 아이템 제거가 필요하다면 여기서 RemoveItem 호출.
                // 현재는 'usedInCurrentScene'으로 씬 당 1회 제한이 걸리므로 제거는 하지 않습니다.
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

        // 불러온 아이템 데이터에 OnUseItem 델리게이트를 다시 연결합니다.
        foreach (var invItem in items)
        {
            BaseItemData itemData = GetItemDataByName(invItem.itemName);
            if (itemData != null)
            {
                if (itemData is PlusTimeItemData plusTimeData)
                {
                    plusTimeData.OnUseItem = () =>
                    {
                        // ShopManager에서 아이템의 현재 업그레이드 레벨을 가져옵니다.
                        int currentLevel = ShopManager.Instance != null ? ShopManager.Instance.GetItemUpgradeLevel(plusTimeData.itemName) : 0;
                        float timeBonus = plusTimeData.GetEffectiveTimeToAdd(currentLevel);

                        GameTimer timer = FindObjectOfType<GameTimer>();
                        if (timer != null)
                        {
                            timer.AddTime(timeBonus);
                        }
                        else
                        {
                            Debug.LogWarning("씬에서 'GameTimer' 스크립트를 찾을 수 없습니다. 시간을 추가할 수 없습니다.");
                        }
                    };
                }
            }
        }
    }

    /// <summary>
    /// 아이템 이름으로 BaseItemData를 찾아 반환합니다.
    /// </summary>
    /// <param name="itemName">찾을 아이템의 이름입니다.</param>
    /// <returns>해당 이름의 BaseItemData 또는 null.</returns>
    public BaseItemData GetItemDataByName(string itemName)
    {
        return allItemData.Find(x => x.itemName == itemName);
    }
}
