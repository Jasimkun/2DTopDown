using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq; // FirstOrDefault ����� ���� �ʿ�
using System; // [NonSerialized] ����� ���� �ʿ�

public class InventorySystem_1 : MonoBehaviour
{
    [Header("�κ��丮���� ������ ������ �����͵�")]
    public BaseItemData_1[] allItemData;  // �ν����Ϳ��� ScriptableObject ������ �� �־��ּ���.
    public static InventorySystem_1 Instance { get; private set; }

    // �κ��丮�� ������ ������ ��� (Inspector���� �̸� ������ �����۵�)
    public List<InventoryItem_1> items = new List<InventoryItem_1>();
    public InventorySystem_1.InventoryItem_1[] quickSlotsInUse;

    public InventoryItem_1 FindItemByName(string itemName)
    {
        return items.FirstOrDefault(item => item.itemName == itemName && item.isAcquired);
    }

    private string savePath;
    private const string saveFileName = "inventory_1.json"; // ���� ���� �̸� ����

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // �� ���� ������Ʈ�� �� ��ȯ �� �ı����� �ʵ��� �����մϴ�.
            // InventoryManager GameObject�� �� ��ũ��Ʈ�� �پ��ִٰ� �����մϴ�.
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Application.persistentDataPath, saveFileName);
            Debug.Log($"[InventorySystem_1] �κ��丮 ���� ���: {savePath}");
        }
        else if (Instance != this)
        {
            // �̹� �ν��Ͻ��� �����ϸ� ���� ������ ������Ʈ�� �ı�
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // ���� ���� �� �κ��丮 ������ �ε�
        LoadInventoryData();
        Debug.Log("[InventorySystem_1] Start���� LoadInventoryData ȣ�� �Ϸ�.");
    }



    void OnApplicationQuit()
    {
        // ���ø����̼� ���� �� �κ��丮 ������ �ڵ� ����
        SaveInventoryData();
        Debug.Log("[InventorySystem_1] ���ø����̼� ���� �� �κ��丮 �ڵ� ����.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseQuickSlotItem(0); // �ð�������
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseQuickSlotItem(1); // ��������
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
                Debug.Log($"[InventorySystem_1] �κ��丮 ������ '{item.itemName}' Ű���� {index}������ ����.");
            }
            else
            {
                Debug.Log($"[InventorySystem_1] �κ��丮 ������ {index}���� ���ų� �̹� ����.");
            }
        }
        else
        {
            Debug.Log($"[InventorySystem_1] �κ��丮 �ε��� {index}�� ������ ���.");
        }
    }


    // --- �κ��丮 ������ ���� �޼��� ---

    // ������ ȹ�� (BaseItemData_1 ������ ���ڷ� ����)
    public void AddItem(BaseItemData_1 newItemData)
    {
        if (newItemData == null)
            return;

        // �κ��丮�� ���� �������� �̹� �ִ��� üũ
        InventoryItem_1 existingItem = items.Find(i => i.itemData == newItemData);

        if (existingItem != null)
        {
            // �̹� �����ϱ� �ƹ� �۾��� �� ��
            Debug.Log($"[InventorySystem_1] ������ '{existingItem.itemName}' �̹� �κ��丮�� ������, �߰� �� ��");
            return;
        }

        // ������ �� ������ �߰�
        InventoryItem_1 newItem = new InventoryItem_1
        {
            itemName = newItemData.itemName,
            count = 1,
            itemData = newItemData,
            usedInCurrentScene = false
        };

        items.Add(newItem);
        Debug.Log($"[InventorySystem_1] �� ������ �߰�: {newItem.itemName}");

        // �����Կ� ������ �Ҵ�
        QuickSlotUIController_1.Instance?.AssignItemToSpecificQuickSlot(newItem);

        // �ʿ��ϸ� �κ��丮 UI ���� ȣ��
        //RefreshInventoryUI();
    }


    // ������ ���
    public void UseInventoryItem(InventoryItem_1 item)
    {
        if (item == null)
        {
            Debug.LogWarning("[InventorySystem_1] ����� �������� null�Դϴ�.");
            return;
        }

        // ������ ��� ������ BaseItemData_1�� ������ Resources���� �ε� �õ�
        if (item.itemData == null && !string.IsNullOrEmpty(item.itemDataName))
        {
            item.itemData = Resources.Load<BaseItemData_1>($"ItemData/{item.itemDataName}");
            if (item.itemData == null)
            {
                Debug.LogError($"[InventorySystem_1] ������ '{item.itemName}' (�����: '{item.itemDataName}')�� �ش��ϴ� BaseItemData_1�� Resources���� ã�� �� �����ϴ�.");
                return;
            }
        }
        else if (item.itemData == null)
        {
            Debug.LogError($"[InventorySystem_1] '{item.itemName}'�� BaseItemData_1 �Ǵ� itemDataName�� �����ϴ�. �������� ����� �� �����ϴ�.");
            return;
        }

        if (item.usedInCurrentScene)
        {
            Debug.Log($"[InventorySystem_1] '{item.itemName}'��(��) �̹� ���Ǿ����ϴ�.");
            return;
        }

        item.itemData.UseItemEffect(); // BaseItemData_1�� ��� ȿ�� ����
        item.usedInCurrentScene = true; // ���� ������ �������� ǥ��
        Debug.Log($"[InventorySystem_1] '{item.itemName}' ������ ��� �Ϸ�. UsedInCurrentScene: {item.usedInCurrentScene}");

        // ������ UI ���� ��û
        if (QuickSlotUIController_1.Instance != null)
        {
            QuickSlotUIController_1.Instance.RefreshQuickSlotsUI();
        }
        SaveInventoryData(); // ���� ���� �� ����
    }

    // ������ ���� (ȹ�� ���� ����)
    public void RemoveItem(InventoryItem_1 item)
    {
        if (items.Contains(item))
        {
            item.isAcquired = false;
            item.usedInCurrentScene = false;
            Debug.Log($"[InventorySystem_1] '{item.itemName}'��(��) �κ��丮���� ���ŵǾ����ϴ� (ȹ�� ���� ����).");
            // ������ UI���� ���� ��û
            if (QuickSlotUIController_1.Instance != null)
            {
                QuickSlotUIController_1.Instance.RemoveItemFromQuickSlot(item);
            }
        }
        SaveInventoryData();
    }

    // --- InventoryItem_1 ���� Ŭ���� ---

    [System.Serializable]
    public class InventoryItem_1
    {
        public string itemName; // UI ǥ�ÿ� �̸�
        public int count = 1; // ������ ���� (����� 1�� ����)
        public bool usedInCurrentScene; // ���� ������ ���Ǿ����� ����
        public string itemDataName; // Resources.Load�� ���� BaseItemData_1 ���� ���� �̸�

        [NonSerialized] // JSON ���� �� �� �ʵ�� ���� (���� ����)
        public BaseItemData_1 itemData; // ��Ÿ�ӿ� �ε�� ���� BaseItemData_1 �ν��Ͻ�

        // Inspector���� �ʱ� �Ҵ��� ���� �ʵ�. ��Ÿ�ӿ��� itemDataName���� �ε�˴ϴ�.
        [SerializeField]
        private BaseItemData_1 _itemDataInspectorOnly;

        // ������ ȹ�� ���� (�ٽ� �ʵ�)
        public bool isAcquired;

        // Inspector���� ������ _itemDataInspectorOnly�κ��� �ʱ� �����͸� ������ �ʵ���� �ʱ�ȭ
        public void InitializeFromInspector()
        {
            if (_itemDataInspectorOnly != null)
            {
                itemData = _itemDataInspectorOnly; // Inspector�� ����� ���� �Ҵ�
                itemName = itemData.itemName; // ������ itemName ��������
                itemDataName = itemData.name; // ������ ���� �̸� ��������
                Debug.Log($"[InventoryItem_1] Inspector �ʱ�ȭ: {itemName}, itemDataName: {itemDataName}, isAcquired: {isAcquired}");
            }
            else
            {
                Debug.LogWarning($"[InventoryItem_1] '_itemDataInspectorOnly'�� �Ҵ���� ���� InventoryItem_1 �߰�. itemName: '{itemName}'. BaseItemData_1 ������ �ʿ��մϴ�.");
                // ���� _itemDataInspectorOnly�� ����ְ� itemDataName�� �ִٸ�, Resources.Load�� �õ��� ���� �ֽ��ϴ�.
                // �� �ó������� LoadInventoryData���� ó���˴ϴ�.
            }
        }
    }

    // --- ������ ����/�ε� ���� ---

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
                    itemDataName = item.itemDataName, // BaseItemData_1 ���� �̸��� ���� (�߿�)
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
        public string itemDataName; // BaseItemData_1 ���� ���� �̸�
        public bool isAcquired;
        public bool usedInCurrentScene;
        public int count;
    }

    public void SaveInventoryData()
    {
        InventorySaveData_1 saveData = new InventorySaveData_1(items);
        string json = JsonUtility.ToJson(saveData, true); // Unity �⺻ JsonUtility ���

        try
        {
            File.WriteAllText(savePath, json);
            Debug.Log("[InventorySystem_1] �κ��丮 ������ ���� �Ϸ�: " + savePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InventorySystem_1] �κ��丮 ������ ���� ����: {e.Message}");
        }
    }

    public void LoadInventoryData()
    {
        // Inspector���� ������ �ʱ� ������ ���� ��� InventoryItem_1�� �ʱ�ȭ�մϴ�.
        // (�� �������� _itemDataInspectorOnly�� itemData, itemName, itemDataName�� ä��ϴ�.)
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
                        // ����� itemDataName�� �������� �ν������� items ����Ʈ���� ��ġ�ϴ� �������� ã���ϴ�.
                        InventoryItem_1 targetItem = items.FirstOrDefault(item =>
                            item.itemDataName == savedState.itemDataName);

                        if (targetItem != null)
                        {
                            targetItem.isAcquired = savedState.isAcquired;
                            targetItem.usedInCurrentScene = savedState.usedInCurrentScene;
                            targetItem.count = savedState.count;

                            // �ε� ������ itemData�� null�̸� Resources���� �ٽ� �ε� �õ�
                            if (targetItem.itemData == null && !string.IsNullOrEmpty(targetItem.itemDataName))
                            {
                                targetItem.itemData = Resources.Load<BaseItemData_1>($"ItemData/{targetItem.itemDataName}");
                                if (targetItem.itemData == null)
                                {
                                    Debug.LogWarning($"[InventorySystem_1] ����� ������ '{targetItem.itemName}' (�̸�: {targetItem.itemDataName})�� BaseItemData_1�� Resources���� ã�� �� �����ϴ�. UI ǥ�ÿ� ������ ���� �� �ֽ��ϴ�.");
                                }
                                else // ���������� �ε��ߴٸ� itemName�� �ٽ� ����
                                {
                                    targetItem.itemName = targetItem.itemData.itemName;
                                    Debug.Log($"[InventorySystem_1] Resources.Load ����: {targetItem.itemName}, ������: {(targetItem.itemData.icon != null ? targetItem.itemData.icon.name : "NULL")}");
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"[InventorySystem_1] ����� '{savedState.itemDataName}'�� �ش��ϴ� �̸� ���ǵ� InventoryItem_1�� ã�� �� �����ϴ�. InventoryManager Inspector�� 'Items' ����Ʈ�� Ȯ���ϼ���.");
                        }
                    }
                    Debug.Log($"[InventorySystem_1] �κ��丮 ������ �ҷ����� �Ϸ�. ���� Ȱ�� ������ ��: {items.Count(item => item.isAcquired)}");

                    // ������ UI ���� ��û
                    if (QuickSlotUIController_1.Instance != null)
                    {
                        QuickSlotUIController_1.Instance.ClearAllQuickSlotsUI();
                        foreach (var item in items)
                        {
                            if (item.isAcquired)
                            {
                                QuickSlotUIController_1.Instance.AssignItemToSpecificQuickSlot(item);
                                Debug.Log($"[InventorySystem_1] ȹ��� ������ '{item.itemName}' ���� �����Կ� ǥ�� ��û.");
                            }
                        }
                        Debug.Log("[InventorySystem_1] �κ��丮 ������ �ε� �� ������ UI ���� ��û �Ϸ�.");
                    }
                    else
                    {
                        Debug.LogWarning("[InventorySystem_1] QuickSlotUIController_1 �ν��Ͻ��� ã�� �� �����ϴ�. ������ UI�� ������ �� �����ϴ�.");
                    }
                }
                else
                {
                    Debug.LogWarning("[InventorySystem_1] �ҷ��� JSON �����Ͱ� ����ְų� ��ȿ���� �ʽ��ϴ�. �κ��丮�� �ʱ�ȭ�մϴ�.");
                    ResetInventoryData();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[InventorySystem_1] �κ��丮 ������ �ҷ����� ����: {e.Message}. �κ��丮�� �ʱ�ȭ�մϴ�.");
                ResetInventoryData();
            }
        }
        else
        {
            Debug.Log("[InventorySystem_1] ����� �κ��丮 ������ �����ϴ�. �� �κ��丮�� �����մϴ�.");
            ResetInventoryData();
        }
    }

    public void ResetInventoryData()
    {
        Debug.Log("[InventorySystem_1] �κ��丮 ������ �ʱ�ȭ ��...");
        // ��� �������� ���¸� �ʱ�ȭ
        foreach (var item in items)
        {
            item.isAcquired = false;
            item.usedInCurrentScene = false;
            item.count = 1; // ī��Ʈ�� 1�� �ʱ�ȭ (�ʿ��)
        }

        if (QuickSlotUIController_1.Instance != null)
        {
            QuickSlotUIController_1.Instance.ClearAllQuickSlotsUI();
        }
        SaveInventoryData(); // �ʱ�ȭ �� ����
        Debug.Log("[InventorySystem_1] �κ��丮 ������ �ʱ�ȭ �Ϸ�.");
    }
}