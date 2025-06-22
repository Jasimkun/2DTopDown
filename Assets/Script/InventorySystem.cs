using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System; // Action 델리게이트 사용을 위해

// InventoryItem 클래스는 그대로 유지됩니다.
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

// 인벤토리 저장용 래퍼 클래스는 그대로 유지됩니다.
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

    // ⬇️ [수정] 모든 아이템 데이터를 BaseItemData 타입으로 저장합니다.
    // 에디터에서 모든 PlusTimeItemData와 LightItemData 에셋을 여기에 할당하세요.
    public List<BaseItemData> allItemData;

    private string savePath;
    private QuickSlotUIController quickSlotUIController;
    private GameObject playerGameObject; // ⬇️ [추가] 플레이어 GameObject를 저장할 변수

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        savePath = Application.persistentDataPath + "/inventory.json";
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
        // 씬 로드 시 모든 아이템의 사용 기록 초기화
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

        // ⬇️ [추가] 씬 로드 시 플레이어 GameObject를 다시 찾습니다.
        // 플레이어에 "Player" 태그가 붙어있다고 가정합니다.
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
    /// <param name="itemData">추가할 아이템 데이터 (BaseItemData 상속)</param>
    public void AddItem(BaseItemData itemData) // ⬇️ [수정] PlusTimeItemData -> BaseItemData로 변경
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
            // 한 번 먹은 아이템은 다시 추가 안 함 (개수 증가 로직이 필요하다면 여기 수정)
            Debug.Log($"이미 인벤토리에 존재하는 아이템이야.");
            return;
        }
        else
        {
            // 새로운 InventoryItem 인스턴스 생성 및 추가
            items.Add(new InventoryItem(itemData.itemName));
            SaveInventory(); // 인벤토리 변경 사항 저장
            Debug.Log($"새로운 아이템을 인벤토리에 저장했어.");

            // ⬇️ [수정] BaseItemData의 OnUseItem 델리게이트 연결 (plusTimeItemData에만 연결)
            // LightItemData는 OnUseItem 델리게이트를 사용하지 않으므로 (ApplyEffect를 직접 호출)
            // PlusTimeItemData 타입일 경우에만 해당 델리게이트를 연결합니다.
            if (itemData is PlusTimeItemData plusTimeData)
            {
                plusTimeData.OnUseItem = () =>
                {
                    Debug.Log($"'{plusTimeData.itemName}' 아이템을 사용했어. 시간이 {plusTimeData.timeToAdd}초 추가된 것 같아.");
                    GameTimer timer = FindObjectOfType<GameTimer>(); // 씬에서 타이머를 찾아 시간을 추가
                    if (timer != null)
                    {
                        timer.AddTime(plusTimeData.timeToAdd);
                    }
                    else
                    {
                        Debug.LogWarning("씬에서 'GameTimer' 스크립트를 찾을 수 없습니다. 시간을 추가할 수 없습니다.");
                    }
                    // 아이템 사용 후 제거 (필요하다면)
                    // RemoveItem(plusTimeData.itemName);
                };
            }
            // ⬇️ [추가] LightItemData는 AddItem 시 별도의 OnUseItem 연결 필요 없음 (UseItemEffect 내부에서 직접 효과 적용)

            // UI 갱신 (퀵 슬롯에 새 아이템이 추가되도록)
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
            BaseItemData itemData = GetItemDataByName(invItem.itemName); // ⬇️ [수정] BaseItemData로 받음

            if (invItem.usedInCurrentScene)
            {
                Debug.Log($"'{invItem.itemName}' 아이템은 맵마다 한 번씩만 사용할 수 있는 것 같아.");
                return;
            }

            if (itemData != null)
            {
                if (playerGameObject == null)
                {
                    playerGameObject = GameObject.FindGameObjectWithTag("Player"); // 플레이어 다시 찾기 시도
                }

                // ⬇️ [수정] BaseItemData의 UseItemEffect 메서드 호출 시 플레이어 GameObject 전달
                // 각 아이템 데이터의 UseItemEffect는 오버라이드되어 해당 아이템의 고유한 효과를 실행합니다.
                itemData.UseItemEffect(playerGameObject); // playerGameObject를 user로 전달

                invItem.usedInCurrentScene = true; // 현재 씬에서 사용됨으로 표시

                // UI 갱신 (아이템 사용 상태 변화를 반영)
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

    // 인벤토리 저장 및 불러오기 메서드는 그대로 유지됩니다.
    public void SaveInventory()
    {
        string json = JsonUtility.ToJson(new InventorySaveData(items));
        File.WriteAllText(savePath, json);
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

        // 불러온 아이템 데이터에 UseItemEffect 델리게이트 다시 연결 (특히 PlusTimeItemData의 경우)
        foreach (var invItem in items)
        {
            BaseItemData itemData = GetItemDataByName(invItem.itemName); // ⬇️ [수정] BaseItemData로 받음
            if (itemData != null)
            {
                // ⬇️ [수정] PlusTimeItemData 타입일 경우에만 델리게이트 연결 (Load 시에도 필요)
                if (itemData is PlusTimeItemData plusTimeData)
                {
                    plusTimeData.OnUseItem = () =>
                    {
                        GameTimer timer = FindObjectOfType<GameTimer>();
                        if (timer != null)
                        {
                            timer.AddTime(plusTimeData.timeToAdd);
                        }
                        else
                        {
                            Debug.LogWarning("씬에서 'GameTimer' 스크립트를 찾을 수 없습니다. 시간을 추가할 수 없습니다.");
                        }
                    };
                }
                // LightItemData는 OnUseItem 델리게이트를 직접 사용하지 않으므로 연결 불필요
            }
        }
    }

    /// <summary>
    /// 아이템 이름으로 BaseItemData를 찾아 반환합니다.
    /// </summary>
    /// <param name="itemName">찾을 아이템의 이름입니다.</param>
    /// <returns>해당 이름의 BaseItemData 또는 null.</returns>
    public BaseItemData GetItemDataByName(string itemName) // ⬇️ [수정] PlusTimeItemData -> BaseItemData로 변경
    {
        return allItemData.Find(x => x.itemName == itemName);
    }

    // InventorySaveData 래퍼 클래스는 여기에 있었으므로 그대로 유지됩니다.
    // [System.Serializable]
    // private class InventorySaveData
    // {
    //     public List<InventoryItem> items;
    //
    //     public InventorySaveData(List<InventoryItem> items)
    //     {
    //         this.items = items;
    //     }
    // }
}
