using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Item
{
    public string itemName;
    public int itemID;
}

[System.Serializable]
public class InventoryData
{
    public List<Item> items = new List<Item>();
}

[System.Serializable]
public class InventoryItem
{
    public string itemName;  // PlusTimeItemData의 itemName과 동일해야 함
    public int count;

    [System.NonSerialized] // 이 필드는 JSON 저장 시 제외 (씬마다 초기화되어야 함)
    public bool usedInCurrentScene; // 이 씬에서 사용되었는지 여부

    public InventoryItem(string name)
    {
        itemName = name;
        count = 1;
        usedInCurrentScene = false; // [추가] 초기화 시 false로 설정
    }
}

// 전체 인벤토리 데이터를 JSON으로 저장하기 위한 컨테이너
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

    // 에디터에서 아이템 데이터 전부 넣어놓기
    public List<PlusTimeItemData> allItemData;

    private string savePath;
    private QuickSlotUIController quickSlotUIController;

    private void Awake()
    {
        // --- [수정] 싱글톤 인스턴스 초기화 및 DontDestroyOnLoad 적용 ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 게임 오브젝트를 씬 전환 시 파괴하지 않음
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // --- DontDestroyOnLoad 적용 끝 ---

        savePath = Application.persistentDataPath + "/inventory.json";

        // [수정] Awake에서는 QuickSlotUIController를 찾지 않습니다. OnSceneLoaded에서 매 씬 로드 시마다 연결할 것입니다.
        // quickSlotUIController = FindObjectOfType<QuickSlotUIController>();
        // if (quickSlotUIController == null)
        // {
        //     Debug.LogWarning("씬에 QuickSlotUIController가 없습니다. 퀵 슬롯 UI를 갱신할 수 없습니다.");
        // }

        LoadInventory(); // 게임 시작 시 저장된 인벤토리 불러오기
    }
    // --- [추가] OnEnable/OnDisable 및 OnSceneLoaded 메서드 ---
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드될 때마다 이벤트 구독
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 오브젝트 비활성화 또는 파괴 시 이벤트 구독 해제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log($"[InventorySystem] 씬 '{scene.name}' 로드 완료.");

        // --- [추가] 중요: 씬 로드 시 모든 아이템의 사용 기록 초기화 ---
        foreach (var item in items)
        {
            item.usedInCurrentScene = false;
            //Debug.Log($"[InventorySystem] 아이템 '{item.itemName}'의 씬 사용 기록 초기화됨.");
        }
        // --- 초기화 끝 ---

        // 새 씬이 로드될 때마다 QuickSlotUIController를 다시 찾아 연결합니다.
        quickSlotUIController = FindObjectOfType<QuickSlotUIController>();
        if (quickSlotUIController != null)
        {
            quickSlotUIController.RefreshQuickSlotsUI(); // 퀵슬롯 UI를 다시 갱신하여 현재 인벤토리 상태 반영
            
        }
        else
        {
            Debug.LogWarning($"[InventorySystem] 씬 '{scene.name}'에서 QuickSlotUIController를 찾을 수 없습니다.");
        }
    }
    // --- OnEnable/OnDisable 및 OnSceneLoaded 메서드 끝 ---


    // 아이템 추가 (한 번 먹은 아이템은 다시 추가 안 함)
    public void AddItem(PlusTimeItemData itemData)
    {
        InventoryItem existing = items.Find(i => i.itemName == itemData.itemName);
        if (existing != null)
        {
            Debug.Log($"이미 인벤토리에 존재하는 아이템이야.");
            return; // 이미 있는 아이템은 추가하지 않고 저장하지도 않음
        }
        else
        {
            items.Add(new InventoryItem(itemData.itemName));
            SaveInventory();
            Debug.Log($"...아이템을 인벤토리에 저장했어.");

            itemData.OnUseItem = () =>
            {
                Debug.Log($" 아이템을 사용했어. 시간이 {itemData.timeToAdd}초 추가된 것 같아.");
                GameTimer timer = FindObjectOfType<GameTimer>();
                if (timer != null)
                {
                    timer.AddTime(itemData.timeToAdd);
                }
                else
                {
                    Debug.LogWarning("씬에서 'GameTimer' 스크립트를 찾을 수 없습니다. 시간을 추가할 수 없습니다.");
                }
                //RemoveItem(itemData.itemName); // 사용 후 제거
            };
            // --- 추가 끝 ---

            if (quickSlotUIController != null)
            {
                quickSlotUIController.RefreshQuickSlotsUI();
            }

        }
    }

    // --- 여기에 아이템 제거 메서드 추가 ---
    //public void RemoveItem(string itemName) //
    //{
        //int initialCount = items.Count;
        // itemName과 일치하는 모든 아이템을 제거 (현재는 1개만 있을 것이므로 사실상 1개 제거)
        //items.RemoveAll(i => i.itemName == itemName);

        //if (items.Count < initialCount) // 아이템이 실제로 제거되었다면
        //{
            //SaveInventory(); // 제거 후 즉시 저장
            //Debug.Log($"[인벤토리] 아이템이 인벤토리에서 제거되고 저장되었습니다.");

            // UI 갱신 (퀵 슬롯에서 아이템이 사라지도록)
            //if (quickSlotUIController != null)
            //{
                //quickSlotUIController.RefreshQuickSlotsUI();
            //}
        //}
        //else
        //{
            //Debug.LogWarning($"[인벤토리] '{itemName}' 아이템을 인벤토리에서 찾을 수 없어 제거할 수 없습니다.");
        //}
    //}
    // 퀵 슬롯 버튼 클릭 시 호출될 아이템 사용 메서드
    public void UseInventoryItem(int quickSlotIndex)
    {
        // quickSlotIndex는 퀵 슬롯 UI의 인덱스이며, 인벤토리 List의 인덱스와 동일하다고 가정합니다.
        if (quickSlotIndex >= 0 && quickSlotIndex < items.Count)
        {
            InventoryItem invItem = items[quickSlotIndex];
            PlusTimeItemData itemData = GetItemDataByName(invItem.itemName);

            // --- [추가] 현재 씬에서 이미 사용되었는지 확인 ---
            if (invItem.usedInCurrentScene)
            {
                Debug.Log($"아이템은 맵마다 한 번씩만 사용할 수 있는 것 같아.");
                return; // 사용 불가
            }
            // --- 추가 끝 ---

            if (itemData != null)
            {
                //Debug.Log($"[인벤토리] 퀵 슬롯 {quickSlotIndex}의 '{itemData.itemName}' 아이템 사용 시도.");
                itemData.UseItemEffect(); // PlusTimeItemData에 정의된 효과 실행
                // --- [추가] 아이템 사용 후 현재 씬 사용 기록 설정 ---
                // 아이템이 인벤토리에서 '제거되지 않고' 씬 당 1회 사용 제한만 걸리는 경우에 아래 코드를 활성화하세요.
                // 만약 아이템 사용 즉시 인벤토리에서 제거된다면 (RemoveItem 호출), 이 부분은 필요 없습니다.
                // invItem.usedInCurrentScene = true;
                // SaveInventory(); // 사용 기록 변경 시 저장 (필요시)
                // Debug.Log($"[인벤토리] '{invItem.itemName}' 씬 사용 기록: true로 설정.");
                // --- 추가 끝 ---
                invItem.usedInCurrentScene = true;

                // UI 갱신 (usedInCurrentScene 상태 변화를 반영하기 위해 필요)
                // 현재는 RemoveItem이 RefreshQuickSlotsUI를 호출하므로 중복될 수 있습니다.
                if (quickSlotUIController != null)
                {
                    quickSlotUIController.RefreshQuickSlotsUI();
                }
                // RemoveItem은 UseItemEffect 내부에서 호출되므로 여기서 따로 호출하지 않습니다.
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


    // 저장
    public void SaveInventory()
    {
        string json = JsonUtility.ToJson(new InventorySaveData(items));
        File.WriteAllText(savePath, json);
    }

    // 불러오기
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

        foreach (var invItem in items)
        {
            PlusTimeItemData itemData = GetItemDataByName(invItem.itemName);
            if (itemData != null)
            {
                itemData.OnUseItem = () =>
                {
                    //Debug.Log($"[인벤토리 - Load - OnUseItem] '{itemData.itemName}' 아이템 효과 실행!");
                    GameTimer timer = FindObjectOfType<GameTimer>();
                    if (timer != null)
                    {
                        timer.AddTime(itemData.timeToAdd);
                    }
                    else
                    {
                        Debug.LogWarning("씬에서 'GameTimer' 스크립트를 찾을 수 없습니다. 시간을 추가할 수 없습니다.");
                    }
                    //RemoveItem(itemData.itemName);
                };
            }
        }
    }

    // 아이템 이름으로 PlusTimeItemData 찾기
    public PlusTimeItemData GetItemDataByName(string itemName)
    {
        return allItemData.Find(x => x.itemName == itemName);
    }

    // 인벤토리 저장용 래퍼 클래스
    [System.Serializable]
    private class InventorySaveData
    {
        public List<InventoryItem> items;

        public InventorySaveData(List<InventoryItem> items)
        {
            this.items = items;
        }
    }
}
