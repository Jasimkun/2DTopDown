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
    public string itemName;  // PlusTimeItemData�� itemName�� �����ؾ� ��
    public int count;

    [System.NonSerialized] // �� �ʵ�� JSON ���� �� ���� (������ �ʱ�ȭ�Ǿ�� ��)
    public bool usedInCurrentScene; // �� ������ ���Ǿ����� ����

    public InventoryItem(string name)
    {
        itemName = name;
        count = 1;
        usedInCurrentScene = false; // [�߰�] �ʱ�ȭ �� false�� ����
    }
}

// ��ü �κ��丮 �����͸� JSON���� �����ϱ� ���� �����̳�
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

    // �����Ϳ��� ������ ������ ���� �־����
    public List<PlusTimeItemData> allItemData;

    private string savePath;
    private QuickSlotUIController quickSlotUIController;

    private void Awake()
    {
        // --- [����] �̱��� �ν��Ͻ� �ʱ�ȭ �� DontDestroyOnLoad ���� ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ���� ������Ʈ�� �� ��ȯ �� �ı����� ����
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // --- DontDestroyOnLoad ���� �� ---

        savePath = Application.persistentDataPath + "/inventory.json";

        // [����] Awake������ QuickSlotUIController�� ã�� �ʽ��ϴ�. OnSceneLoaded���� �� �� �ε� �ø��� ������ ���Դϴ�.
        // quickSlotUIController = FindObjectOfType<QuickSlotUIController>();
        // if (quickSlotUIController == null)
        // {
        //     Debug.LogWarning("���� QuickSlotUIController�� �����ϴ�. �� ���� UI�� ������ �� �����ϴ�.");
        // }

        LoadInventory(); // ���� ���� �� ����� �κ��丮 �ҷ�����
    }
    // --- [�߰�] OnEnable/OnDisable �� OnSceneLoaded �޼��� ---
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε�� ������ �̺�Ʈ ����
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // ������Ʈ ��Ȱ��ȭ �Ǵ� �ı� �� �̺�Ʈ ���� ����
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log($"[InventorySystem] �� '{scene.name}' �ε� �Ϸ�.");

        // --- [�߰�] �߿�: �� �ε� �� ��� �������� ��� ��� �ʱ�ȭ ---
        foreach (var item in items)
        {
            item.usedInCurrentScene = false;
            //Debug.Log($"[InventorySystem] ������ '{item.itemName}'�� �� ��� ��� �ʱ�ȭ��.");
        }
        // --- �ʱ�ȭ �� ---

        // �� ���� �ε�� ������ QuickSlotUIController�� �ٽ� ã�� �����մϴ�.
        quickSlotUIController = FindObjectOfType<QuickSlotUIController>();
        if (quickSlotUIController != null)
        {
            quickSlotUIController.RefreshQuickSlotsUI(); // ������ UI�� �ٽ� �����Ͽ� ���� �κ��丮 ���� �ݿ�
            
        }
        else
        {
            Debug.LogWarning($"[InventorySystem] �� '{scene.name}'���� QuickSlotUIController�� ã�� �� �����ϴ�.");
        }
    }
    // --- OnEnable/OnDisable �� OnSceneLoaded �޼��� �� ---


    // ������ �߰� (�� �� ���� �������� �ٽ� �߰� �� ��)
    public void AddItem(PlusTimeItemData itemData)
    {
        InventoryItem existing = items.Find(i => i.itemName == itemData.itemName);
        if (existing != null)
        {
            Debug.Log($"�̹� �κ��丮�� �����ϴ� �������̾�.");
            return; // �̹� �ִ� �������� �߰����� �ʰ� ���������� ����
        }
        else
        {
            items.Add(new InventoryItem(itemData.itemName));
            SaveInventory();
            Debug.Log($"...�������� �κ��丮�� �����߾�.");

            itemData.OnUseItem = () =>
            {
                Debug.Log($" �������� ����߾�. �ð��� {itemData.timeToAdd}�� �߰��� �� ����.");
                GameTimer timer = FindObjectOfType<GameTimer>();
                if (timer != null)
                {
                    timer.AddTime(itemData.timeToAdd);
                }
                else
                {
                    Debug.LogWarning("������ 'GameTimer' ��ũ��Ʈ�� ã�� �� �����ϴ�. �ð��� �߰��� �� �����ϴ�.");
                }
                //RemoveItem(itemData.itemName); // ��� �� ����
            };
            // --- �߰� �� ---

            if (quickSlotUIController != null)
            {
                quickSlotUIController.RefreshQuickSlotsUI();
            }

        }
    }

    // --- ���⿡ ������ ���� �޼��� �߰� ---
    //public void RemoveItem(string itemName) //
    //{
        //int initialCount = items.Count;
        // itemName�� ��ġ�ϴ� ��� �������� ���� (����� 1���� ���� ���̹Ƿ� ��ǻ� 1�� ����)
        //items.RemoveAll(i => i.itemName == itemName);

        //if (items.Count < initialCount) // �������� ������ ���ŵǾ��ٸ�
        //{
            //SaveInventory(); // ���� �� ��� ����
            //Debug.Log($"[�κ��丮] �������� �κ��丮���� ���ŵǰ� ����Ǿ����ϴ�.");

            // UI ���� (�� ���Կ��� �������� ���������)
            //if (quickSlotUIController != null)
            //{
                //quickSlotUIController.RefreshQuickSlotsUI();
            //}
        //}
        //else
        //{
            //Debug.LogWarning($"[�κ��丮] '{itemName}' �������� �κ��丮���� ã�� �� ���� ������ �� �����ϴ�.");
        //}
    //}
    // �� ���� ��ư Ŭ�� �� ȣ��� ������ ��� �޼���
    public void UseInventoryItem(int quickSlotIndex)
    {
        // quickSlotIndex�� �� ���� UI�� �ε����̸�, �κ��丮 List�� �ε����� �����ϴٰ� �����մϴ�.
        if (quickSlotIndex >= 0 && quickSlotIndex < items.Count)
        {
            InventoryItem invItem = items[quickSlotIndex];
            PlusTimeItemData itemData = GetItemDataByName(invItem.itemName);

            // --- [�߰�] ���� ������ �̹� ���Ǿ����� Ȯ�� ---
            if (invItem.usedInCurrentScene)
            {
                Debug.Log($"�������� �ʸ��� �� ������ ����� �� �ִ� �� ����.");
                return; // ��� �Ұ�
            }
            // --- �߰� �� ---

            if (itemData != null)
            {
                //Debug.Log($"[�κ��丮] �� ���� {quickSlotIndex}�� '{itemData.itemName}' ������ ��� �õ�.");
                itemData.UseItemEffect(); // PlusTimeItemData�� ���ǵ� ȿ�� ����
                // --- [�߰�] ������ ��� �� ���� �� ��� ��� ���� ---
                // �������� �κ��丮���� '���ŵ��� �ʰ�' �� �� 1ȸ ��� ���Ѹ� �ɸ��� ��쿡 �Ʒ� �ڵ带 Ȱ��ȭ�ϼ���.
                // ���� ������ ��� ��� �κ��丮���� ���ŵȴٸ� (RemoveItem ȣ��), �� �κ��� �ʿ� �����ϴ�.
                // invItem.usedInCurrentScene = true;
                // SaveInventory(); // ��� ��� ���� �� ���� (�ʿ��)
                // Debug.Log($"[�κ��丮] '{invItem.itemName}' �� ��� ���: true�� ����.");
                // --- �߰� �� ---
                invItem.usedInCurrentScene = true;

                // UI ���� (usedInCurrentScene ���� ��ȭ�� �ݿ��ϱ� ���� �ʿ�)
                // ����� RemoveItem�� RefreshQuickSlotsUI�� ȣ���ϹǷ� �ߺ��� �� �ֽ��ϴ�.
                if (quickSlotUIController != null)
                {
                    quickSlotUIController.RefreshQuickSlotsUI();
                }
                // RemoveItem�� UseItemEffect ���ο��� ȣ��ǹǷ� ���⼭ ���� ȣ������ �ʽ��ϴ�.
            }
            else
            {
                Debug.LogWarning($"[�κ��丮] ���� {quickSlotIndex}�� ������ ������('{invItem.itemName}')�� ã�� �� ���� ����� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning($"[�κ��丮] ��ȿ���� ���� �� ���� �ε���: {quickSlotIndex} (���� �κ��丮 ������ ��: {items.Count})");
        }
    }


    // ����
    public void SaveInventory()
    {
        string json = JsonUtility.ToJson(new InventorySaveData(items));
        File.WriteAllText(savePath, json);
    }

    // �ҷ�����
    public void LoadInventory()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            InventorySaveData loadedData = JsonUtility.FromJson<InventorySaveData>(json);
            items = loadedData.items;
            if (items.Count == 0)
            {
                Debug.Log("����� �κ��丮�� ���� �� ����.");
            }
            else
            {
                Debug.Log($"...������ �κ��丮�� �ҷ��Ծ�.");
            }
        }
        else
        {
            Debug.Log("����� �κ��丮�� ���� �� ����.");
        }

        foreach (var invItem in items)
        {
            PlusTimeItemData itemData = GetItemDataByName(invItem.itemName);
            if (itemData != null)
            {
                itemData.OnUseItem = () =>
                {
                    //Debug.Log($"[�κ��丮 - Load - OnUseItem] '{itemData.itemName}' ������ ȿ�� ����!");
                    GameTimer timer = FindObjectOfType<GameTimer>();
                    if (timer != null)
                    {
                        timer.AddTime(itemData.timeToAdd);
                    }
                    else
                    {
                        Debug.LogWarning("������ 'GameTimer' ��ũ��Ʈ�� ã�� �� �����ϴ�. �ð��� �߰��� �� �����ϴ�.");
                    }
                    //RemoveItem(itemData.itemName);
                };
            }
        }
    }

    // ������ �̸����� PlusTimeItemData ã��
    public PlusTimeItemData GetItemDataByName(string itemName)
    {
        return allItemData.Find(x => x.itemName == itemName);
    }

    // �κ��丮 ����� ���� Ŭ����
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
