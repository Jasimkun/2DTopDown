using System.Collections.Generic;
using UnityEngine;
using TMPro; // UI �ؽ�Ʈ (��� ǥ��)
using System.Linq; // Dictionary�� List ��ȯ�� ���� ��� (Select, ToList)
using System.IO; // ���� ����/�ε带 ���� ���

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    [Header("��� ����")]
    public int currentGold = 0; // �÷��̾ ���� ���� ���
    public TextMeshProUGUI goldDisplayText; // UI���� ��带 ǥ���� TextMeshProUGUI ������Ʈ

    [Header("���� ���׷��̵� ����")]
    [Tooltip("�� �������� ������ ��� UpgradeDefinition ������ �Ҵ��ϼ���.")]
    public List<UpgradeDefinition> allUpgradeDefinitions; // �������� ������ ��� ���׷��̵� ���� ���� ���

    // �� �������� ���� ���׷��̵� ������ �����մϴ�. (������ �̸� -> ����)
    // �� �����ʹ� ���� ����/�ε� �� �Բ� ����Ǿ�� �մϴ�.
    private Dictionary<string, int> itemUpgradeLevels = new Dictionary<string, int>();

    // ���� �����͸� ������ ���
    private string shopSavePath;

    void Awake()
    {
        // �̱��� ���� ����: �ߺ� �ν��Ͻ� ���� �� �� ��ȯ �� �ı� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            shopSavePath = Application.persistentDataPath + "/shopData.json"; // ���� ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� ������ �ڽ��� �ı�
            return;
        }
    }

    void Start()
    {
        LoadGoldAndUpgradeLevels(); // ���� ���� �� ����� ������ �ε�
        UpdateGoldUI(); // UI �ʱ�ȭ

        // ��� ���׷��̵� ������ �����ۿ� ���� �ʱ� ���� ���� (�ε���� ���� ���)
        foreach (var upgradeDef in allUpgradeDefinitions)
        {
            if (upgradeDef.targetItemData != null && !itemUpgradeLevels.ContainsKey(upgradeDef.targetItemData.itemName))
            {
                itemUpgradeLevels[upgradeDef.targetItemData.itemName] = 0; // �ʱ� ���� 0 ����
            }
        }
    }

    // --- ��� ���� �޼��� ---

    /// <summary>
    /// ������ �縸ŭ ��带 �߰��մϴ�.
    /// </summary>
    /// <param name="amount">�߰��� ����� ���Դϴ�.</param>
    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
        Debug.Log($"��� ȹ��: {amount}. ���� ���: {currentGold}");
        SaveGoldAndUpgradeLevels(); // ��� ���� �� ����
    }

    /// <summary>
    /// ������ �縸ŭ ��带 ����մϴ�. ��尡 ����ϸ� true�� ��ȯ�ϰ�, �ƴϸ� false�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="amount">����� ����� ���Դϴ�.</param>
    /// <returns>��� ��� ���� �����Դϴ�.</returns>
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldUI();
            Debug.Log($"��� ���: {amount}. ���� ���: {currentGold}");
            SaveGoldAndUpgradeLevels(); // ��� ���� �� ����
            return true;
        }
        Debug.Log("��尡 �����մϴ�.");
        return false;
    }

    /// <summary>
    /// UI�� ��� ǥ�ø� �����մϴ�.
    /// </summary>
    private void UpdateGoldUI()
    {
        if (goldDisplayText != null)
        {
            goldDisplayText.text = $"���: {currentGold}";
        }
    }

    // --- ���׷��̵� ���� �޼��� ---

    /// <summary>
    /// Ư�� �������� ���� ���׷��̵� ������ �����ɴϴ�.
    /// </summary>
    /// <param name="itemName">�������� �̸��Դϴ�.</param>
    /// <returns>�������� ���� ������ ��ȯ�ϰų�, �ش� �������� ���׷��̵� ��Ͽ� ������ 0�� ��ȯ�մϴ�.</returns>
    public int GetItemUpgradeLevel(string itemName)
    {
        // Dictionary���� ������ �̸����� ������ ã��, ������ �⺻�� 0�� ��ȯ�մϴ�.
        return itemUpgradeLevels.TryGetValue(itemName, out int level) ? level : 0;
    }

    /// <summary>
    /// Ư�� ���׷��̵� ���ǿ� �ش��ϴ� ���� ������ ���׷��̵带 �õ��մϴ�.
    /// </summary>
    /// <param name="upgradeDefinition">���׷��̵��� UpgradeDefinition �����Դϴ�.</param>
    /// <returns>���׷��̵� ���� �����Դϴ�.</returns>
    public bool TryUpgradeItem(UpgradeDefinition upgradeDefinition)
    {
        if (upgradeDefinition == null || upgradeDefinition.targetItemData == null)
        {
            Debug.LogWarning("��ȿ���� ���� ���׷��̵� ���� �Ǵ� ��� ������ �������Դϴ�.");
            return false;
        }

        string itemName = upgradeDefinition.targetItemData.itemName;
        int currentLevel = GetItemUpgradeLevel(itemName);

        // �ִ� ������ �����ߴ��� Ȯ��
        if (currentLevel >= upgradeDefinition.maxLevel)
        {
            Debug.Log($"'{itemName}'��(��) �̹� �ִ� �����Դϴ�. (����: {currentLevel}/{upgradeDefinition.maxLevel})");
            return false;
        }

        // ���� ���� ���׷��̵� ����� �����ɴϴ�.
        int upgradeCost = upgradeDefinition.GetCostForLevel(currentLevel + 1);
        if (upgradeCost == -1)
        {
            Debug.LogWarning($"'{itemName}'�� ���� {currentLevel + 1} ���׷��̵� ����� ���ǵ��� �ʾҽ��ϴ�.");
            return false;
        }

        // ��� ��� �õ�
        if (SpendGold(upgradeCost))
        {
            itemUpgradeLevels[itemName] = currentLevel + 1; // ���� ����
            Debug.Log($"'{itemName}'��(��) ���� {itemUpgradeLevels[itemName]}�� ���׷��̵��߽��ϴ�! ���: {upgradeCost}");
            SaveGoldAndUpgradeLevels(); // ���� ���� �� ����
            // ShopUI�� RefreshShopUI�� ȣ���Ͽ� UI�� �����ϵ��� �մϴ�.
            return true;
        }
        return false; // ��� �������� ���׷��̵� ����
    }

    /// <summary>
    /// ���׷��̵� ������ BaseItemData�� �̸����� ã���ϴ�.
    /// </summary>
    /// <param name="itemName">ã�� �������� �̸��Դϴ�.</param>
    /// <returns>�ش��ϴ� BaseItemData �Ǵ� null.</returns>
    public BaseItemData GetUpgradeableItemDataByName(string itemName)
    {
        // allUpgradeDefinitions ��Ͽ��� ��� ������ �̸��� ��ġ�ϴ� UpgradeDefinition�� ã��
        // �� ���ǿ� ����� targetItemData�� ��ȯ�մϴ�.
        // �̴� ShopUI ��� UpgradeDefinition�� ���� ItemData�� ������ �� ���˴ϴ�.
        var upgradeDef = allUpgradeDefinitions.Find(def => def.targetItemData != null && def.targetItemData.itemName == itemName);
        return upgradeDef?.targetItemData;
    }

    // --- ����/�ε� �ý��� ---

    // ���� �����͸� ���� ����ȭ ������ Ŭ����
    [System.Serializable]
    private class ShopSaveData
    {
        public int gold; // ���� ���
        public List<UpgradeLevelEntry> upgradeLevels; // �����ۺ� ���׷��̵� ���� ���
    }

    // �� �������� ���׷��̵� ���� ��Ʈ��
    [System.Serializable]
    private class UpgradeLevelEntry
    {
        public string itemName;
        public int level;
    }

    /// <summary>
    /// ���� ���� ��� �������� ���׷��̵� ������ ���Ͽ� �����մϴ�.
    /// </summary>
    public void SaveGoldAndUpgradeLevels()
    {
        // Dictionary�� ������ ����ȭ ������ List�� ��ȯ�մϴ�.
        List<UpgradeLevelEntry> entries = itemUpgradeLevels
            .Select(kv => new UpgradeLevelEntry { itemName = kv.Key, level = kv.Value })
            .ToList();

        ShopSaveData data = new ShopSaveData { gold = currentGold, upgradeLevels = entries };
        string json = JsonUtility.ToJson(data, true); // (true: ������ ���� ������)
        File.WriteAllText(shopSavePath, json);
        Debug.Log("���� ������ �� ���׷��̵� ���� �����.");
    }

    /// <summary>
    /// ����� ���� ������ ���׷��̵� ������ ���Ͽ��� �ҷ��ɴϴ�.
    /// </summary>
    public void LoadGoldAndUpgradeLevels()
    {
        if (File.Exists(shopSavePath))
        {
            string json = File.ReadAllText(shopSavePath);
            ShopSaveData data = JsonUtility.FromJson<ShopSaveData>(json);

            currentGold = data.gold;
            itemUpgradeLevels.Clear(); // ���� ������ �ʱ�ȭ

            // �ҷ��� ���� �����͸� Dictionary�� �ٽ� ä��ϴ�.
            foreach (var entry in data.upgradeLevels)
            {
                itemUpgradeLevels[entry.itemName] = entry.level;
            }
            Debug.Log("���� ������ �� ���׷��̵� ���� �ε��.");
        }
        else
        {
            Debug.Log("���� ���� ������ �����ϴ�. �ʱ� ��� �� ������ �����մϴ�.");
            currentGold = 0; // ���� ������ ������ ��带 0���� �ʱ�ȭ
        }
    }
}
