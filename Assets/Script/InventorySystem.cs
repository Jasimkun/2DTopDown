using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq; // LINQ 사용을 위해 추가

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    public List<InventoryItem> items = new List<InventoryItem>();

    private string savePath;
    private const string saveFileName = "inventory.json";

    void Awake()
    {
        Debug.Log("[InventorySystem] Awake 호출됨.");
        if (Instance == null)
        {
            Instance = this;
            // 중요: DontDestroyOnLoad는 루트 GameObject에만 적용됩니다.
            // InventoryManager가 DontDestroyOnLoad 아래에 있다면, InventoryManager 자체에 스크립트가 붙어야 합니다.
            // 현재 Hierarchy (image_a130a6.png)상 DontDestroyOnLoad의 자식으로 InventoryManager가 있습니다.
            // InventoryManager에 이 스크립트가 붙어있고 InventoryManager 자체가 씬 전환 시 파괴되지 않도록 되어 있다면 괜찮습니다.
            // 만약 InventoryManager가 매 씬마다 생성된다면 문제가 됩니다.
            // 일단 DontDestroyOnLoad(gameObject);를 제거하고, InventoryManager GameObject 자체를 씬 전환 시 파괴되지 않도록 설정하거나
            // 아니면 이 스크립트가 붙어있는 GameObject를 DontDestroyOnLoad에 직접 추가해야 합니다.
            // 현재 DontDestroyOnLoad 오브젝트의 자식으로 InventoryManager가 있고, InventoryManager에 이 스크립트가 붙어있다고 가정합니다.
            // 따라서 DontDestroyOnLoad는 InventoryManager 부모에 붙어있어야 합니다.
            // 만약 이 스크립트가 DontDestroyOnLoad GameObject 자체에 붙어있다면 DontDestroyOnLoad(gameObject);는 괜찮습니다.
            // 이 문제는 현재 다른 오류들에 가려져 있지만, 씬 전환 시 다시 발생할 수 있습니다.
            // 일단은 현재 오류 해결에 집중합니다.

            savePath = Path.Combine(Application.persistentDataPath, saveFileName);
            Debug.Log($"[InventorySystem] 인벤토리 저장 경로: {savePath}");
        }
        else
        {
            // 중복 인스턴스 파괴. 이것이 중요합니다.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // LoadInventoryData를 Start에서 호출하여 QuickSlotUIController가 먼저 초기화되도록 보장
        LoadInventoryData();
        Debug.Log("[InventorySystem] Start에서 LoadInventoryData 호출 완료.");
    }

    void OnApplicationQuit()
    {
        SaveInventoryData();
        Debug.Log("[InventorySystem] 애플리케이션 종료 시 인벤토리 자동 저장.");
    }

    public void ResetInventoryData()
    {
        Debug.Log("[InventorySystem] 인벤토리 데이터 초기화 중...");
        // 모든 아이템의 상태를 초기화
        foreach (var item in items)
        {
            item.isAcquired = false;
            item.usedInCurrentScene = false;
            // item.count = 1; // 필요 시 초기화
        }

        if (QuickSlotUIController.Instance != null)
        {
            QuickSlotUIController.Instance.ClearAllQuickSlotsUI();
        }
        SaveInventoryData(); // 초기화 후 저장
        Debug.Log("[InventorySystem] 인벤토리 데이터 초기화 완료.");
    }

    // 아이템 획득 시 호출되는 메서드
    public void AddItem(BaseItemData itemDataToAcquire)
    {
        if (itemDataToAcquire == null)
        {
            Debug.LogWarning("[InventorySystem] 획득하려는 itemData가 null입니다.");
            return;
        }

        // itemData의 이름(에셋 파일 이름)과 일치하는 InventoryItem을 찾습니다.
        // FirstOrDefault를 사용하려면 using System.Linq; 가 필요합니다.
        InventoryItem targetItem = items.FirstOrDefault(item =>
            item.itemData != null && item.itemData.name == itemDataToAcquire.name);

        if (targetItem != null)
        {
            if (targetItem.isAcquired)
            {
                Debug.Log($"[InventorySystem] {targetItem.itemName} (이미 획득됨) 다시 추가하지 않습니다.");
                return;
            }

            // 아이템 활성화
            targetItem.isAcquired = true;
            targetItem.usedInCurrentScene = false;
            targetItem.count = 1;

            // itemDataName 필드를 BaseItemData의 이름(에셋 파일 이름)으로 설정 (이것이 핵심)
            targetItem.itemDataName = itemDataToAcquire.name;

            Debug.Log($"[InventorySystem] {targetItem.itemName} 획득 및 활성화됨.");

            if (QuickSlotUIController.Instance != null)
            {
                QuickSlotUIController.Instance.AssignItemToSpecificQuickSlot(targetItem);
                Debug.Log($"[InventorySystem] {targetItem.itemName} 고정 퀵슬롯 등록 요청 (획득).");
            }
            else
            {
                Debug.LogWarning("[InventorySystem] QuickSlotUIController 인스턴스를 찾을 수 없습니다. 퀵슬롯 등록에 실패했습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"[InventorySystem] 미리 정의되지 않은 아이템 '{itemDataToAcquire.name}'을(를) 획득하려 시도했습니다. InventoryManager Inspector에 추가되어 있는지 확인하세요.");
        }
        SaveInventoryData();
    }

    public List<InventoryItem> GetItems()
    {
        return items;
    }

    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;
        public int count = 1;
        public bool usedInCurrentScene;
        public string itemDataName; // Resources.Load를 위한 BaseItemData의 에셋 파일 이름

        //[NonSerialized] // JSON 저장 시 이 필드는 무시
        public BaseItemData itemData;

        // Inspector에서 초기 할당을 위한 필드. 런타임에는 itemDataName으로 로드
        [SerializeField] private BaseItemData _itemDataInspectorOnly;

        // 아이템 획득 여부를 나타내는 필드 (CS1061 오류의 원인이 되었던 부분)
        public bool isAcquired;

        // Inspector에서 _itemDataInspectorOnly가 있으면 itemData에 할당하고 itemDataName 설정
        public void InitializeFromInspector()
        {
            if (_itemDataInspectorOnly != null)
            {
                itemData = _itemDataInspectorOnly; // _itemDataInspectorOnly를 실제 itemData에 할당
                itemName = itemData.itemName; // BaseItemData의 itemName을 InventoryItem의 itemName에 할당
                itemDataName = itemData.name; // BaseItemData의 에셋 파일 이름을 itemDataName에 할당
            }
        }
    }

    public void UseInventoryItem(InventoryItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("[InventorySystem] 사용할 아이템이 null입니다.");
            return;
        }

        // 아이템 사용 전에 itemData가 null이면 다시 로드 시도 (견고성 강화)
        if (item.itemData == null && !string.IsNullOrEmpty(item.itemDataName))
        {
            // Resources/ItemData/ 경로에서 itemDataName으로 BaseItemData를 로드
            item.itemData = Resources.Load<BaseItemData>($"ItemData/{item.itemDataName}");
            if (item.itemData == null)
            {
                Debug.LogError($"[InventorySystem] 아이템 '{item.itemName}' (저장명: '{item.itemDataName}')에 해당하는 BaseItemData를 Resources에서 찾을 수 없습니다. 경로 확인: Assets/Resources/ItemData/{item.itemDataName}.asset");
                return;
            }
        }
        else if (item.itemData == null)
        {
            Debug.LogError($"[InventorySystem] '{item.itemName}'의 BaseItemData 또는 itemDataName이 없습니다. 아이템을 사용할 수 없습니다.");
            return;
        }

        if (item.usedInCurrentScene)
        {
            Debug.Log($"[InventorySystem] '{item.itemName}'은(는) 이미 사용되었습니다.");
            return;
        }

        item.itemData.UseItemEffect(); // BaseItemData의 사용 효과 호출
        item.usedInCurrentScene = true; // 현재 씬에서 사용됨으로 표시
        Debug.Log($"[InventorySystem] '{item.itemName}' 아이템 사용 완료. UsedInCurrentScene: {item.usedInCurrentScene}");

        if (QuickSlotUIController.Instance != null)
        {
            QuickSlotUIController.Instance.RefreshQuickSlotsUI(); // UI 갱신 요청
        }
        SaveInventoryData(); // 상태 변경 후 저장
    }

    public void RemoveItem(InventoryItem item)
    {
        if (items.Contains(item))
        {
            item.isAcquired = false;
            item.usedInCurrentScene = false;
            Debug.Log($"[InventorySystem] '{item.itemName}'이(가) 인벤토리에서 제거되었습니다 (획득 상태 해제).");
            if (QuickSlotUIController.Instance != null)
            {
                QuickSlotUIController.Instance.RemoveItemFromQuickSlot(item);
            }
        }
        SaveInventoryData();
    }

    // JSON 저장을 위한 도우미 클래스
    [System.Serializable]
    public class InventorySaveData
    {
        public List<InventoryItemSaveState> savedItemStates;

        public InventorySaveData(List<InventoryItem> currentItems)
        {
            savedItemStates = new List<InventoryItemSaveState>();
            foreach (var item in currentItems)
            {
                savedItemStates.Add(new InventoryItemSaveState
                {
                    itemDataName = item.itemDataName, // BaseItemData 에셋 이름을 저장
                    isAcquired = item.isAcquired,
                    usedInCurrentScene = item.usedInCurrentScene,
                    count = item.count
                });
            }
        }
    }

    [System.Serializable]
    public class InventoryItemSaveState
    {
        public string itemDataName;
        public bool isAcquired;
        public bool usedInCurrentScene;
        public int count;
    }

    public void SaveInventoryData()
    {
        InventorySaveData saveData = new InventorySaveData(items);
        string json = JsonUtility.ToJson(saveData, true);

        try
        {
            File.WriteAllText(savePath, json);
            Debug.Log("[InventorySystem] 인벤토리 데이터 저장 완료: " + savePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InventorySystem] 인벤토리 데이터 저장 실패: {e.Message}");
        }
    }

    public void LoadInventoryData()
    {
        // 먼저 현재 items 리스트의 모든 InventoryItem을 Inspector 설정에서 초기화합니다.
        foreach (var item in items)
        {
            item.InitializeFromInspector();
        }

        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                InventorySaveData loadedData = JsonUtility.FromJson<InventorySaveData>(json);

                if (loadedData != null && loadedData.savedItemStates != null)
                {
                    // 저장된 상태로 현재 items 리스트 갱신
                    foreach (var savedState in loadedData.savedItemStates)
                    {
                        // savedState의 itemDataName을 기준으로 현재 items 리스트에서 일치하는 아이템을 찾습니다.
                        // 이 때, item.itemData가 null일 수 있으므로 null 체크를 추가하고 item.itemDataName으로 비교
                        InventoryItem targetItem = items.FirstOrDefault(item =>
                            item.itemDataName == savedState.itemDataName);

                        if (targetItem != null)
                        {
                            targetItem.isAcquired = savedState.isAcquired;
                            targetItem.usedInCurrentScene = savedState.usedInCurrentScene;
                            targetItem.count = savedState.count;

                            // 로드 시점에 itemData가 null이면 다시 Resources.Load 시도 (견고성 강화)
                            if (targetItem.itemData == null && !string.IsNullOrEmpty(targetItem.itemDataName))
                            {
                                targetItem.itemData = Resources.Load<BaseItemData>($"ItemData/{targetItem.itemDataName}");
                                if (targetItem.itemData == null)
                                {
                                    Debug.LogWarning($"[InventorySystem] 저장된 아이템 '{targetItem.itemName}' (이름: {targetItem.itemDataName})의 BaseItemData를 Resources에서 찾을 수 없습니다. UI 표시에 문제가 있을 수 있습니다.");
                                }
                                else // 성공적으로 로드했다면 itemName도 다시 설정
                                {
                                    targetItem.itemName = targetItem.itemData.itemName;
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"[InventorySystem] 저장된 '{savedState.itemDataName}'에 해당하는 미리 정의된 InventoryItem을 찾을 수 없습니다. InventoryManager Inspector를 확인하세요.");
                        }
                    }
                    Debug.Log($"[InventorySystem] 인벤토리 데이터 불러오기 완료. 현재 활성 아이템 수: {items.Count(item => item.isAcquired)}");

                    if (QuickSlotUIController.Instance != null)
                    {
                        QuickSlotUIController.Instance.ClearAllQuickSlotsUI();
                        foreach (var item in items)
                        {
                            if (item.isAcquired)
                            {
                                QuickSlotUIController.Instance.AssignItemToSpecificQuickSlot(item);
                                Debug.Log($"[InventorySystem] 획득된 아이템 '{item.itemName}' 고정 퀵슬롯에 표시 요청.");
                            }
                        }
                        Debug.Log("[InventorySystem] 인벤토리 데이터 로드 후 퀵슬롯 UI 갱신 요청 완료.");
                    }
                }
                else
                {
                    Debug.LogWarning("[InventorySystem] 불러온 JSON 데이터가 비어있거나 유효하지 않습니다. 인벤토리를 초기화합니다.");
                    ResetInventoryData();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[InventorySystem] 인벤토리 데이터 불러오기 실패: {e.Message}. 인벤토리를 초기화합니다.");
                ResetInventoryData();
            }
        }
        else
        {
            Debug.Log("[InventorySystem] 저장된 인벤토리 파일이 없습니다. 새 인벤토리를 시작합니다.");
            ResetInventoryData();
        }
    }
}