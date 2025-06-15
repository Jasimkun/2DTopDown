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
    public string itemName;  // PlusTimeItemData의 itemName과 동일해야 함
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

    // 에디터에서 아이템 데이터 전부 넣어놓기
    public List<PlusTimeItemData> allItemData;

    private string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/inventory.json";
        LoadInventory();
    }

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

        }
    }

    // --- 여기에 아이템 제거 메서드 추가 ---
    public void RemoveItem(string itemName) //
    {
        int removedCount = items.RemoveAll(i => i.itemName == itemName); //
        if (removedCount > 0) //
        {
            SaveInventory(); //
            Debug.Log($"[인벤토리] '{itemName}' 인벤토리 초기화.");
        }
        else //
        {
            Debug.LogWarning($"[인벤토리] '{itemName}' 아이템을 인벤토리에서 찾을 수 없어 제거할 수 없습니다."); //
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
