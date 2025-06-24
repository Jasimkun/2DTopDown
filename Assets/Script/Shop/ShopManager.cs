using System.Collections.Generic;
using UnityEngine;
using TMPro; // UI 텍스트 (골드 표시)
using System.Linq; // Dictionary와 List 변환을 위해 사용 (Select, ToList)
using System.IO; // 파일 저장/로드를 위해 사용
using UnityEngine.SceneManagement; // 씬 로드 이벤트 감지를 위해 추가

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; } // 싱글톤 인스턴스

    [Header("골드 설정")]
    public int currentGold = 0; // 플레이어가 현재 가진 골드
    // public TextMeshProUGUI goldDisplayText; // 이 필드는 Awake/OnSceneLoaded에서 동적으로 찾습니다.

    [Header("상점 업그레이드 정의")]
    [Tooltip("이 상점에서 제공할 모든 UpgradeDefinition 에셋을 할당하세요.")]
    public List<UpgradeDefinition> allUpgradeDefinitions; // 상점에서 제공할 모든 업그레이드 정의 에셋 목록

    // 각 아이템의 현재 업그레이드 레벨을 저장합니다. (아이템 이름 -> 레벨)
    private Dictionary<string, int> itemUpgradeLevels = new Dictionary<string, int>();

    // 상점 데이터를 저장할 경로
    private string shopSavePath;

    private TextMeshProUGUI _goldDisplayText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            shopSavePath = Application.persistentDataPath + "/shopData.json"; // 저장 경로 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 자신을 파괴
            return;
        }
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
        GameObject goldTextObj = GameObject.Find("GoldText"); // 씬에서 "GoldText"라는 이름의 GameObject를 찾습니다.
        if (goldTextObj != null)
        {
            _goldDisplayText = goldTextObj.GetComponent<TextMeshProUGUI>();
            if (_goldDisplayText == null)
            {
                Debug.LogWarning("[ShopManager] 'GoldText' GameObject에서 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("[ShopManager] 씬에서 'GoldText' GameObject를 찾을 수 없습니다. 골드 UI 갱신 불가.");
        }

        UpdateGoldUI(); // 씬 로드 시 골드 UI 갱신
        RefreshShopUI(); // ⬇️ [추가] 씬 로드 시 상점 UI 갱신
    }

    void Start()
    {
        LoadGoldAndUpgradeLevels();

        foreach (var upgradeDef in allUpgradeDefinitions)
        {
            if (upgradeDef.targetItemData != null && !itemUpgradeLevels.ContainsKey(upgradeDef.targetItemData.itemName))
            {
                itemUpgradeLevels[upgradeDef.targetItemData.itemName] = 0;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddGold(30);
            Debug.Log("[Cheat] 키보드 '1'번으로 골드 30개 추가됨.");
        }
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
        Debug.Log($"골드 획득: {amount}. 현재 골드: {currentGold}");
        SaveGoldAndUpgradeLevels(); // 골드 변경 시 저장
        RefreshShopUI(); // ⬇️ [추가] 골드 변경 시 상점 UI 갱신 요청
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldUI();
            Debug.Log($"골드 사용: {amount}. 현재 골드: {currentGold}");
            SaveGoldAndUpgradeLevels(); // 골드 변경 시 저장
            RefreshShopUI(); // ⬇️ [추가] 골드 사용 시 상점 UI 갱신 요청
            return true;
        }
        Debug.Log("골드가 부족합니다.");
        return false;
    }

    /// <summary>
    /// UI에 골드 표시를 갱신합니다.
    /// </summary>
    private void UpdateGoldUI()
    {
        if (_goldDisplayText != null)
        {
            _goldDisplayText.text = $"골드: {currentGold}";
        }
        else
        {
            Debug.LogWarning("[ShopManager] _goldDisplayText가 현재 씬에서 할당되지 않았습니다. 골드 UI를 갱신할 수 없습니다.");
        }
    }

    // ⬇️ [추가] 현재 씬의 ShopUI를 찾아 RefreshShopUI를 호출하는 헬퍼 메서드
    private void RefreshShopUI()
    {
        ShopUI shopUI = FindObjectOfType<ShopUI>(true); // 비활성화된 오브젝트에 붙은 컴포넌트도 찾도록
        if (shopUI != null)
        {
            shopUI.RefreshShopUI();
            Debug.Log("[ShopManager] ShopUI 갱신 요청됨.");
        }
        else
        {
            Debug.LogWarning("[ShopManager] 현재 씬에서 ShopUI를 찾을 수 없어 갱신할 수 없습니다.");
        }
    }

    public int GetItemUpgradeLevel(string itemName)
    {
        return itemUpgradeLevels.TryGetValue(itemName, out int level) ? level : 0;
    }

    /// <summary>
    /// 특정 업그레이드 정의에 해당하는 다음 레벨로 업그레이드를 시도합니다.
    /// </summary>
    /// <param name="upgradeDefinition">업그레이드할 UpgradeDefinition 에셋입니다.</param>
    /// <returns>업그레이드 성공 여부입니다.</returns>
    public bool TryUpgradeItem(UpgradeDefinition upgradeDefinition)
    {
        if (upgradeDefinition == null || upgradeDefinition.targetItemData == null)
        {
            Debug.LogWarning("유효하지 않은 업그레이드 정의 또는 대상 아이템 데이터입니다.");
            return false;
        }

        UpgradeDefinition upgradeDef = upgradeDefinition;

        Debug.Log($"[ShopManager] '{upgradeDef.upgradeTitle}' 업그레이드 시도. 대상 아이템: '{upgradeDef.targetItemData.itemName}'.");

        string itemName = upgradeDef.targetItemData.itemName;
        int currentLevel = GetItemUpgradeLevel(itemName);

        if (currentLevel >= upgradeDef.maxLevel)
        {
            Debug.Log($"'{itemName}'은(는) 이미 최대 레벨입니다. (레벨: {currentLevel}/{upgradeDef.maxLevel})");
            return false;
        }

        int upgradeCost = upgradeDef.GetCostForLevel(currentLevel + 1);
        if (upgradeCost == -1)
        {
            Debug.LogWarning($"'{itemName}'의 레벨 {currentLevel + 1} 업그레이드 비용이 정의되지 않았습니다.");
            return false;
        }

        if (SpendGold(upgradeCost))
        {
            itemUpgradeLevels[itemName] = currentLevel + 1;
            Debug.Log($"[ShopManager] '{itemName}'을(를) 레벨 {itemUpgradeLevels[itemName]}로 업그레이드했습니다! 비용: {upgradeCost}");
            SaveGoldAndUpgradeLevels();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 업그레이드 가능한 BaseItemData를 이름으로 찾습니다.
    /// </summary>
    /// <param name="itemName">찾을 아이템의 이름입니다.</param>
    /// <returns>해당하는 BaseItemData 또는 null.</returns>
    public BaseItemData GetUpgradeableItemDataByName(string itemName)
    {
        var upgradeDef = allUpgradeDefinitions.Find(def => def.targetItemData != null && def.targetItemData.itemName == itemName);
        return upgradeDef?.targetItemData;
    }

    // --- 저장/로드 시스템 ---

    [System.Serializable]
    private class ShopSaveData
    {
        public int gold;
        public List<UpgradeLevelEntry> upgradeLevels;
    }

    [System.Serializable]
    private class UpgradeLevelEntry
    {
        public string itemName;
        public int level;
    }

    public void SaveGoldAndUpgradeLevels()
    {
        List<UpgradeLevelEntry> entries = itemUpgradeLevels
            .Select(kv => new UpgradeLevelEntry { itemName = kv.Key, level = kv.Value })
            .ToList();

        ShopSaveData data = new ShopSaveData { gold = currentGold, upgradeLevels = entries };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(shopSavePath, json);
        Debug.Log("상점 데이터 및 업그레이드 레벨 저장됨.");
    }

    public void LoadGoldAndUpgradeLevels()
    {
        if (File.Exists(shopSavePath))
        {
            string json = File.ReadAllText(shopSavePath);
            ShopSaveData data = JsonUtility.FromJson<ShopSaveData>(json);

            currentGold = data.gold;
            itemUpgradeLevels.Clear();
            foreach (var entry in data.upgradeLevels)
            {
                itemUpgradeLevels[entry.itemName] = entry.level;
            }
            Debug.Log("상점 데이터 및 업그레이드 레벨 로드됨.");
        }
        else
        {
            Debug.Log("상점 저장 파일이 없습니다. 초기 골드 및 레벨로 시작합니다.");
            currentGold = 0;
        }
    }

    public void ResetShopData()
    {
        currentGold = 0;
        itemUpgradeLevels.Clear();

        foreach (var upgradeDef in allUpgradeDefinitions)
        {
            if (upgradeDef.targetItemData != null)
            {
                itemUpgradeLevels[upgradeDef.targetItemData.itemName] = 0;
            }
        }
        SaveGoldAndUpgradeLevels();
        Debug.Log("[ShopManager] 상점 데이터 및 업그레이드 레벨이 초기화되었습니다.");

        UpdateGoldUI(); // 골드 UI만 즉시 반영
        RefreshShopUI(); // ⬇️ [추가] 리셋 후 상점 UI 갱신 요청
    }
}
