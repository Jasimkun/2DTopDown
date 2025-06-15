using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    public InventoryItem(string name)
    {
        itemName = name;
        count = 1;
    }
}

public class InventorySystem : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>();

    // �����Ϳ��� ������ ������ ���� �־����
    public List<PlusTimeItemData> allItemData;

    private string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/inventory.json";
        LoadInventory();
    }

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

        }
    }

    // --- ���⿡ ������ ���� �޼��� �߰� ---
    public void RemoveItem(string itemName) //
    {
        int removedCount = items.RemoveAll(i => i.itemName == itemName); //
        if (removedCount > 0) //
        {
            SaveInventory(); //
            Debug.Log($"[�κ��丮] '{itemName}' �κ��丮 �ʱ�ȭ.");
        }
        else //
        {
            Debug.LogWarning($"[�κ��丮] '{itemName}' �������� �κ��丮���� ã�� �� ���� ������ �� �����ϴ�."); //
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
