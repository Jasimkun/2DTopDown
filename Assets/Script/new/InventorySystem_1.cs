using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq; // FirstOrDefault 사용을 위해 필요
using System; // [NonSerialized] 사용을 위해 필요

public class InventorySystem_1 : MonoBehaviour
{
    [Header("인벤토리에서 관리할 아이템 데이터들")]
    public BaseItemData_1[] allItemData;  // 인스펙터에서 ScriptableObject 에셋을 쭉 넣어주세요.
    public static InventorySystem_1 Instance { get; private set; }

    // 인벤토리에 관리될 아이템 목록 (Inspector에서 미리 설정할 아이템들)
    public List<InventoryItem_1> items = new List<InventoryItem_1>();
    public InventorySystem_1.InventoryItem_1[] quickSlotsInUse;

    public InventoryItem_1 FindItemByName(string itemName)
    {
        return items.FirstOrDefault(item => item.itemName == itemName && item.isAcquired);
    }

    private string savePath;
    private const string saveFileName = "inventory_1.json"; // 저장 파일 이름 변경

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 이 게임 오브젝트가 씬 전환 시 파괴되지 않도록 설정합니다.
            // InventoryManager GameObject에 이 스크립트가 붙어있다고 가정합니다.
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Application.persistentDataPath, saveFileName);
            Debug.Log($"[InventorySystem_1] 인벤토리 저장 경로: {savePath}");
        }
        else if (Instance != this)
        {
            // 이미 인스턴스가 존재하면 새로 생성된 오브젝트는 파괴
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 게임 시작 시 인벤토리 데이터 로드
        LoadInventoryData();
        Debug.Log("[InventorySystem_1] Start에서 LoadInventoryData 호출 완료.");
    }



    void OnApplicationQuit()
    {
        // 애플리케이션 종료 시 인벤토리 데이터 자동 저장
        SaveInventoryData();
        Debug.Log("[InventorySystem_1] 애플리케이션 종료 시 인벤토리 자동 저장.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseQuickSlotItem(0); // 시간아이템
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseQuickSlotItem(1); // 빛아이템
        }
    }
    private void UseQuickSlotItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < quickSlotsInUse.Length)
        {
            var item = quickSlotsInUse[slotIndex];
            if (item != null && !item.usedInCurrentScene)
            {
                InventorySystem_1.Instance?.UseInventoryItem(item);
            }
        }
    }

    private void UseInventoryItemByIndex(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            var item = items[index];
            if (item.isAcquired && !item.usedInCurrentScene)
            {
                UseInventoryItem(item);
                Debug.Log($"[InventorySystem_1] 인벤토리 아이템 '{item.itemName}' 키보드 {index}번으로 사용됨.");
            }
            else
            {
                Debug.Log($"[InventorySystem_1] 인벤토리 아이템 {index}번이 없거나 이미 사용됨.");
            }
        }
        else
        {
            Debug.Log($"[InventorySystem_1] 인벤토리 인덱스 {index}가 범위를 벗어남.");
        }
    }


    // --- 인벤토리 아이템 관리 메서드 ---

    // 아이템 획득 (BaseItemData_1 에셋을 인자로 받음)
    public void AddItem(BaseItemData_1 newItemData)
    {
        if (newItemData == null)
            return;

        // 인벤토리에 같은 아이템이 이미 있는지 체크
        InventoryItem_1 existingItem = items.Find(i => i.itemData == newItemData);

        if (existingItem != null)
        {
            // 이미 있으니까 아무 작업도 안 함
            Debug.Log($"[InventorySystem_1] 아이템 '{existingItem.itemName}' 이미 인벤토리에 존재함, 추가 안 함");
            return;
        }

        // 없으면 새 아이템 추가
        InventoryItem_1 newItem = new InventoryItem_1
        {
            itemName = newItemData.itemName,
            count = 1,
            itemData = newItemData,
            usedInCurrentScene = false
        };

        items.Add(newItem);
        Debug.Log($"[InventorySystem_1] 새 아이템 추가: {newItem.itemName}");

        // 퀵슬롯에 아이템 할당
        QuickSlotUIController_1.Instance?.AssignItemToSpecificQuickSlot(newItem);

        // 필요하면 인벤토리 UI 갱신 호출
        //RefreshInventoryUI();
    }


    // 아이템 사용
    public void UseInventoryItem(InventoryItem_1 item)
    {
        if (item == null)
        {
            Debug.LogWarning("[InventorySystem_1] 사용할 아이템이 null입니다.");
            return;
        }

        // 아이템 사용 시점에 BaseItemData_1가 없으면 Resources에서 로드 시도
        if (item.itemData == null && !string.IsNullOrEmpty(item.itemDataName))
        {
            item.itemData = Resources.Load<BaseItemData_1>($"ItemData/{item.itemDataName}");
            if (item.itemData == null)
            {
                Debug.LogError($"[InventorySystem_1] 아이템 '{item.itemName}' (저장명: '{item.itemDataName}')에 해당하는 BaseItemData_1를 Resources에서 찾을 수 없습니다.");
                return;
            }
        }
        else if (item.itemData == null)
        {
            Debug.LogError($"[InventorySystem_1] '{item.itemName}'의 BaseItemData_1 또는 itemDataName이 없습니다. 아이템을 사용할 수 없습니다.");
            return;
        }

        if (item.usedInCurrentScene)
        {
            Debug.Log($"[InventorySystem_1] '{item.itemName}'은(는) 이미 사용되었습니다.");
            return;
        }

        item.itemData.UseItemEffect(); // BaseItemData_1의 사용 효과 실행
        item.usedInCurrentScene = true; // 현재 씬에서 사용됨으로 표시
        Debug.Log($"[InventorySystem_1] '{item.itemName}' 아이템 사용 완료. UsedInCurrentScene: {item.usedInCurrentScene}");

        // 퀵슬롯 UI 갱신 요청
        if (QuickSlotUIController_1.Instance != null)
        {
            QuickSlotUIController_1.Instance.RefreshQuickSlotsUI();
        }
        SaveInventoryData(); // 상태 변경 후 저장
    }

    // 아이템 제거 (획득 상태 해제)
    public void RemoveItem(InventoryItem_1 item)
    {
        if (items.Contains(item))
        {
            item.isAcquired = false;
            item.usedInCurrentScene = false;
            Debug.Log($"[InventorySystem_1] '{item.itemName}'이(가) 인벤토리에서 제거되었습니다 (획득 상태 해제).");
            // 퀵슬롯 UI에서 제거 요청
            if (QuickSlotUIController_1.Instance != null)
            {
                QuickSlotUIController_1.Instance.RemoveItemFromQuickSlot(item);
            }
        }
        SaveInventoryData();
    }

    // --- InventoryItem_1 내부 클래스 ---

    [System.Serializable]
    public class InventoryItem_1
    {
        public string itemName; // UI 표시용 이름
        public int count = 1; // 아이템 개수 (현재는 1로 고정)
        public bool usedInCurrentScene; // 현재 씬에서 사용되었는지 여부
        public string itemDataName; // Resources.Load를 위한 BaseItemData_1 에셋 파일 이름

        [NonSerialized] // JSON 저장 시 이 필드는 제외 (오류 방지)
        public BaseItemData_1 itemData; // 런타임에 로드될 실제 BaseItemData_1 인스턴스

        // Inspector에서 초기 할당을 위한 필드. 런타임에는 itemDataName으로 로드됩니다.
        [SerializeField]
        private BaseItemData_1 _itemDataInspectorOnly;

        // 아이템 획득 여부 (핵심 필드)
        public bool isAcquired;

        // Inspector에서 설정된 _itemDataInspectorOnly로부터 초기 데이터를 가져와 필드들을 초기화
        public void InitializeFromInspector()
        {
            if (_itemDataInspectorOnly != null)
            {
                itemData = _itemDataInspectorOnly; // Inspector에 연결된 에셋 할당
                itemName = itemData.itemName; // 에셋의 itemName 가져오기
                itemDataName = itemData.name; // 에셋의 파일 이름 가져오기
                Debug.Log($"[InventoryItem_1] Inspector 초기화: {itemName}, itemDataName: {itemDataName}, isAcquired: {isAcquired}");
            }
            else
            {
                Debug.LogWarning($"[InventoryItem_1] '_itemDataInspectorOnly'가 할당되지 않은 InventoryItem_1 발견. itemName: '{itemName}'. BaseItemData_1 연결이 필요합니다.");
                // 만약 _itemDataInspectorOnly가 비어있고 itemDataName은 있다면, Resources.Load를 시도할 수도 있습니다.
                // 이 시나리오는 LoadInventoryData에서 처리됩니다.
            }
        }
    }

    // --- 데이터 저장/로드 로직 ---

    [System.Serializable]
    public class InventorySaveData_1
    {
        public List<InventoryItemSaveState_1> savedItemStates;

        public InventorySaveData_1(List<InventoryItem_1> currentItems)
        {
            savedItemStates = new List<InventoryItemSaveState_1>();
            foreach (var item in currentItems)
            {
                savedItemStates.Add(new InventoryItemSaveState_1
                {
                    itemDataName = item.itemDataName, // BaseItemData_1 에셋 이름을 저장 (중요)
                    isAcquired = item.isAcquired,
                    usedInCurrentScene = item.usedInCurrentScene,
                    count = item.count
                });
            }
        }
    }

    [System.Serializable]
    public class InventoryItemSaveState_1
    {
        public string itemDataName; // BaseItemData_1 에셋 파일 이름
        public bool isAcquired;
        public bool usedInCurrentScene;
        public int count;
    }

    public void SaveInventoryData()
    {
        InventorySaveData_1 saveData = new InventorySaveData_1(items);
        string json = JsonUtility.ToJson(saveData, true); // Unity 기본 JsonUtility 사용

        try
        {
            File.WriteAllText(savePath, json);
            Debug.Log("[InventorySystem_1] 인벤토리 데이터 저장 완료: " + savePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InventorySystem_1] 인벤토리 데이터 저장 실패: {e.Message}");
        }
    }

    public void LoadInventoryData()
    {
        // Inspector에서 설정된 초기 값으로 먼저 모든 InventoryItem_1을 초기화합니다.
        // (이 과정에서 _itemDataInspectorOnly가 itemData, itemName, itemDataName을 채웁니다.)
        foreach (var item in items)
        {
            item.InitializeFromInspector();
        }

        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                InventorySaveData_1 loadedData = JsonUtility.FromJson<InventorySaveData_1>(json);

                if (loadedData != null && loadedData.savedItemStates != null)
                {
                    foreach (var savedState in loadedData.savedItemStates)
                    {
                        // 저장된 itemDataName을 기준으로 인스펙터의 items 리스트에서 일치하는 아이템을 찾습니다.
                        InventoryItem_1 targetItem = items.FirstOrDefault(item =>
                            item.itemDataName == savedState.itemDataName);

                        if (targetItem != null)
                        {
                            targetItem.isAcquired = savedState.isAcquired;
                            targetItem.usedInCurrentScene = savedState.usedInCurrentScene;
                            targetItem.count = savedState.count;

                            // 로드 시점에 itemData가 null이면 Resources에서 다시 로드 시도
                            if (targetItem.itemData == null && !string.IsNullOrEmpty(targetItem.itemDataName))
                            {
                                targetItem.itemData = Resources.Load<BaseItemData_1>($"ItemData/{targetItem.itemDataName}");
                                if (targetItem.itemData == null)
                                {
                                    Debug.LogWarning($"[InventorySystem_1] 저장된 아이템 '{targetItem.itemName}' (이름: {targetItem.itemDataName})의 BaseItemData_1를 Resources에서 찾을 수 없습니다. UI 표시에 문제가 있을 수 있습니다.");
                                }
                                else // 성공적으로 로드했다면 itemName도 다시 설정
                                {
                                    targetItem.itemName = targetItem.itemData.itemName;
                                    Debug.Log($"[InventorySystem_1] Resources.Load 성공: {targetItem.itemName}, 아이콘: {(targetItem.itemData.icon != null ? targetItem.itemData.icon.name : "NULL")}");
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"[InventorySystem_1] 저장된 '{savedState.itemDataName}'에 해당하는 미리 정의된 InventoryItem_1을 찾을 수 없습니다. InventoryManager Inspector의 'Items' 리스트를 확인하세요.");
                        }
                    }
                    Debug.Log($"[InventorySystem_1] 인벤토리 데이터 불러오기 완료. 현재 활성 아이템 수: {items.Count(item => item.isAcquired)}");

                    // 퀵슬롯 UI 갱신 요청
                    if (QuickSlotUIController_1.Instance != null)
                    {
                        QuickSlotUIController_1.Instance.ClearAllQuickSlotsUI();
                        foreach (var item in items)
                        {
                            if (item.isAcquired)
                            {
                                QuickSlotUIController_1.Instance.AssignItemToSpecificQuickSlot(item);
                                Debug.Log($"[InventorySystem_1] 획득된 아이템 '{item.itemName}' 고정 퀵슬롯에 표시 요청.");
                            }
                        }
                        Debug.Log("[InventorySystem_1] 인벤토리 데이터 로드 후 퀵슬롯 UI 갱신 요청 완료.");
                    }
                    else
                    {
                        Debug.LogWarning("[InventorySystem_1] QuickSlotUIController_1 인스턴스를 찾을 수 없습니다. 퀵슬롯 UI를 갱신할 수 없습니다.");
                    }
                }
                else
                {
                    Debug.LogWarning("[InventorySystem_1] 불러온 JSON 데이터가 비어있거나 유효하지 않습니다. 인벤토리를 초기화합니다.");
                    ResetInventoryData();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[InventorySystem_1] 인벤토리 데이터 불러오기 실패: {e.Message}. 인벤토리를 초기화합니다.");
                ResetInventoryData();
            }
        }
        else
        {
            Debug.Log("[InventorySystem_1] 저장된 인벤토리 파일이 없습니다. 새 인벤토리를 시작합니다.");
            ResetInventoryData();
        }
    }

    public void ResetInventoryData()
    {
        Debug.Log("[InventorySystem_1] 인벤토리 데이터 초기화 중...");
        // 모든 아이템의 상태를 초기화
        foreach (var item in items)
        {
            item.isAcquired = false;
            item.usedInCurrentScene = false;
            item.count = 1; // 카운트도 1로 초기화 (필요시)
        }

        if (QuickSlotUIController_1.Instance != null)
        {
            QuickSlotUIController_1.Instance.ClearAllQuickSlotsUI();
        }
        SaveInventoryData(); // 초기화 후 저장
        Debug.Log("[InventorySystem_1] 인벤토리 데이터 초기화 완료.");
    }
}