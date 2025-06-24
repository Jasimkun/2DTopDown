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

    // 씬의 TextMeshProUGUI 컴포넌트 참조.
    // DontDestroyOnLoad를 사용하는 경우, 이 필드는 씬이 로드될 때마다 스크립트에서 재할당되어야 합니다.
    // 인스펙터에서 직접 할당하지 마세요. (씬이 바뀔 때 참조가 끊어집니다)
    private TextMeshProUGUI goldDisplayTextUI; // <-- private로 유지하고 Inspector에서 직접 할당하지 않음

    [Header("상점 업그레이드 정의")]
    [Tooltip("이 상점에서 제공할 모든 UpgradeDefinition 에셋을 할당하세요.")]
    public List<UpgradeDefinition> allUpgradeDefinitions; // 상점에서 제공할 모든 업그레이드 정의 에셋 목록

    // 각 아이템의 현재 업그레이드 레벨을 저장합니다. (아이템 이름 -> 레벨)
    private Dictionary<string, int> itemUpgradeLevels = new Dictionary<string, int>();

    // 상점 데이터를 저장할 경로
    private string shopSavePath;

    void Awake()
    {
        // 싱글톤 패턴 구현: 중복 인스턴스 방지 및 씬 전환 시 파괴 방지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            shopSavePath = Application.persistentDataPath + "/shopData.json"; // 저장 경로 설정
            Debug.Log($"[ShopManager] Awake: ShopManager 인스턴스 초기화됨. 저장 경로: {shopSavePath}");
        }
        else
        {
            Debug.LogWarning("[ShopManager] Awake: 이미 ShopManager 인스턴스가 존재합니다. 중복 인스턴스를 파괴합니다.");
            Destroy(gameObject); // 이미 인스턴스가 있으면 자신을 파괴
            return;
        }
    }

    void OnEnable()
    {
        // 씬 로드 이벤트를 구독하여 씬이 바뀔 때마다 UI 참조를 다시 찾도록 합니다.
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("[ShopManager] OnEnable: 씬 로드 이벤트를 구독했습니다.");
    }

    void OnDisable()
    {
        // 씬 언로드 시 이벤트 구독을 해제합니다.
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("[ShopManager] OnDisable: 씬 로드 이벤트를 구독 해제했습니다.");
    }

    // 씬이 로드될 때 호출됩니다.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[ShopManager] OnSceneLoaded: 씬 '{scene.name}'이(가) 로드되었습니다. UI 참조를 갱신합니다.");

        // goldDisplayTextUI의 기존 참조와 관계없이 항상 새로운 씬에서 GoldText를 찾습니다.
        // 이것은 DontDestroyOnLoad를 사용하는 싱글톤이 씬 별 UI를 처리하는 가장 견고한 방법입니다.
        GameObject goldTextObj = GameObject.Find("GoldText"); // 현재 씬에서 "GoldText" GameObject를 찾습니다.
        if (goldTextObj != null)
        {
            goldDisplayTextUI = goldTextObj.GetComponent<TextMeshProUGUI>();
            if (goldDisplayTextUI != null)
            {
                Debug.Log($"[ShopManager] OnSceneLoaded: 씬 '{scene.name}'에서 'GoldText' GameObject의 TextMeshProUGUI 컴포넌트를 성공적으로 찾았습니다.");
            }
            else
            {
                Debug.LogWarning($"[ShopManager] OnSceneLoaded: 'GoldText' GameObject에서 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다. 이름 철자와 컴포넌트 추가 여부를 확인하세요.");
            }
        }
        else
        {
            Debug.LogWarning($"[ShopManager] OnSceneLoaded: 씬 '{scene.name}'에서 'GoldText' GameObject를 찾을 수 없습니다. 이 씬에서는 골드 UI를 갱신할 수 없습니다.");
        }

        UpdateGoldUI(); // UI 참조를 얻었거나 이미 있다면 갱신합니다.
        RefreshShopUI(); // 씬 로드 시 상점 UI 갱신 요청
    }

    void Start()
    {
        LoadGoldAndUpgradeLevels(); // 게임 시작 시 저장된 데이터 로드
        Debug.Log($"[ShopManager] Start: 현재 골드: {currentGold}. 업그레이드 레벨 로드 완료.");

        // 모든 업그레이드 가능한 아이템에 대해 초기 레벨 설정 (로드되지 않은 경우)
        foreach (var upgradeDef in allUpgradeDefinitions)
        {
            if (upgradeDef.targetItemData != null && !itemUpgradeLevels.ContainsKey(upgradeDef.targetItemData.itemName))
            {
                itemUpgradeLevels[upgradeDef.targetItemData.itemName] = 0; // 초기 레벨 0 설정
                Debug.Log($"[ShopManager] Start: '{upgradeDef.targetItemData.itemName}'의 초기 업그레이드 레벨 0으로 설정.");
            }
        }
        // Start에서도 UI를 한 번 더 업데이트하여 초기 상태를 보장합니다.
        UpdateGoldUI();
    }

    void Update()
    {
        // 테스트용 치트키: 키보드 '1'번 누르면 골드 30개 추가
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddGold(30);
            Debug.Log("[Cheat] 키보드 '1'번으로 골드 30개 추가됨.");
        }
    }

    /// <summary>
    /// 플레이어의 골드를 증가시키고 UI를 갱신합니다.
    /// </summary>
    /// <param name="amount">추가할 골드 양.</param>
    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI(); // 골드 UI 갱신
        Debug.Log($"골드 획득: {amount}. 현재 골드: {currentGold}");
        SaveGoldAndUpgradeLevels(); // 골드 변경 시 저장
        RefreshShopUI(); // 골드 변경 시 상점 UI 갱신 요청
    }

    /// <summary>
    /// 플레이어의 골드를 감소시킵니다. 골드가 부족하면 false를 반환합니다.
    /// </summary>
    /// <param name="amount">사용할 골드 양.</param>
    /// <returns>골드 사용 성공 여부.</returns>
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldUI(); // 골드 UI 갱신
            Debug.Log($"골드 사용: {amount}. 현재 골드: {currentGold}");
            SaveGoldAndUpgradeLevels(); // 골드 변경 시 저장
            RefreshShopUI(); // 골드 사용 시 상점 UI 갱신 요청
            return true;
        }
        Debug.Log("골드가 부족합니다.");
        return false;
    }

    /// <summary>
    /// UI에 골드 표시를 갱신합니다.
    /// 이 메서드는 public으로 변경되어 외부에서 상점 UI를 열 때 호출될 수 있습니다.
    /// </summary>
    public void UpdateGoldUI()
    {
        if (goldDisplayTextUI != null)
        {
            goldDisplayTextUI.text = $"골드: {currentGold}";
            Debug.Log($"[ShopManager] 골드 UI 갱신됨: {goldDisplayTextUI.text}"); // 추가 로그
        }
        else
        {
            Debug.LogWarning("[ShopManager] goldDisplayTextUI가 할당되지 않았거나 찾을 수 없습니다. 골드 UI를 갱신할 수 없습니다.");
        }
    }

    /// <summary>
    /// 현재 씬의 ShopUI를 찾아 RefreshShopUI를 호출하는 헬퍼 메서드입니다.
    /// </summary>
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

    /// <summary>
    /// 특정 아이템의 현재 업그레이드 레벨을 가져옵니다.
    /// </summary>
    /// <param name="itemName">아이템의 이름입니다.</param>
    /// <returns>현재 업그레이드 레벨 (정의되지 않은 경우 0).</returns>
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

        Debug.Log($"[ShopManager] '{upgradeDefinition.upgradeTitle}' 업그레이드 시도. 대상 아이템: '{upgradeDefinition.targetItemData.itemName}'.");

        string itemName = upgradeDefinition.targetItemData.itemName;
        int currentLevel = GetItemUpgradeLevel(itemName);

        if (currentLevel >= upgradeDefinition.maxLevel)
        {
            Debug.Log($"'{itemName}'은(는) 이미 최대 레벨입니다. (레벨: {currentLevel}/{upgradeDefinition.maxLevel})");
            return false;
        }

        int upgradeCost = upgradeDefinition.GetCostForLevel(currentLevel + 1);
        if (upgradeCost == -1)
        {
            Debug.LogWarning($"'{itemName}'의 레벨 {currentLevel + 1} 업그레이드 비용이 정의되지 않았습니다.");
            return false;
        }

        if (SpendGold(upgradeCost)) // 골드 사용 시도
        {
            itemUpgradeLevels[itemName] = currentLevel + 1; // 레벨 증가
            Debug.Log($"[ShopManager] '{itemName}'을(를) 레벨 {itemUpgradeLevels[itemName]}로 업그레이드했습니다! 비용: {upgradeCost}");
            SaveGoldAndUpgradeLevels(); // 변경사항 저장
            return true;
        }
        return false; // 골드 부족으로 업그레이드 실패
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

    // --- 저장/로드 시스템을 위한 내부 클래스 ---

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

    /// <summary>
    /// 현재 골드와 아이템 업그레이드 레벨을 JSON 파일로 저장합니다.
    /// </summary>
    public void SaveGoldAndUpgradeLevels()
    {
        List<UpgradeLevelEntry> entries = itemUpgradeLevels
            .Select(kv => new UpgradeLevelEntry { itemName = kv.Key, level = kv.Value })
            .ToList();

        ShopSaveData data = new ShopSaveData { gold = currentGold, upgradeLevels = entries };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(shopSavePath, json);
        Debug.Log("[ShopManager] 상점 데이터 및 업그레이드 레벨 저장됨.");
    }

    /// <summary>
    /// JSON 파일에서 골드와 아이템 업그레이드 레벨을 로드합니다.
    /// </summary>
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
            Debug.Log("[ShopManager] 상점 데이터 및 업그레이드 레벨 로드됨.");
        }
        else
        {
            Debug.Log("[ShopManager] 상점 저장 파일이 없습니다. 초기 골드 및 레벨로 시작합니다.");
            currentGold = 0;
        }
    }

    /// <summary>
    /// 상점 데이터(골드 및 업그레이드 레벨)를 초기화합니다.
    /// </summary>
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

        UpdateGoldUI();
        RefreshShopUI();
    }
}
