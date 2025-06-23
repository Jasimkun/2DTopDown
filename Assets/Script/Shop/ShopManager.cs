using System.Collections.Generic;
using UnityEngine;
using TMPro; // UI 텍스트 (골드 표시)
using System.Linq; // Dictionary와 List 변환을 위해 사용 (Select, ToList)
using System.IO; // 파일 저장/로드를 위해 사용

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; } // 싱글톤 인스턴스

    [Header("골드 설정")]
    public int currentGold = 0; // 플레이어가 현재 가진 골드
    public TextMeshProUGUI goldDisplayText; // UI에서 골드를 표시할 TextMeshProUGUI 컴포넌트

    [Header("상점 업그레이드 정의")]
    [Tooltip("이 상점에서 제공할 모든 UpgradeDefinition 에셋을 할당하세요.")]
    public List<UpgradeDefinition> allUpgradeDefinitions; // 상점에서 제공할 모든 업그레이드 정의 에셋 목록

    // 각 아이템의 현재 업그레이드 레벨을 저장합니다. (아이템 이름 -> 레벨)
    // 이 데이터는 게임 저장/로드 시 함께 저장되어야 합니다.
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
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 자신을 파괴
            return;
        }
    }

    void Start()
    {
        LoadGoldAndUpgradeLevels(); // 게임 시작 시 저장된 데이터 로드
        UpdateGoldUI(); // UI 초기화

        // 모든 업그레이드 가능한 아이템에 대해 초기 레벨 설정 (로드되지 않은 경우)
        foreach (var upgradeDef in allUpgradeDefinitions)
        {
            if (upgradeDef.targetItemData != null && !itemUpgradeLevels.ContainsKey(upgradeDef.targetItemData.itemName))
            {
                itemUpgradeLevels[upgradeDef.targetItemData.itemName] = 0; // 초기 레벨 0 설정
            }
        }
    }

    // --- 골드 관리 메서드 ---

    /// <summary>
    /// 지정된 양만큼 골드를 추가합니다.
    /// </summary>
    /// <param name="amount">추가할 골드의 양입니다.</param>
    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
        Debug.Log($"골드 획득: {amount}. 현재 골드: {currentGold}");
        SaveGoldAndUpgradeLevels(); // 골드 변경 시 저장
    }

    /// <summary>
    /// 지정된 양만큼 골드를 사용합니다. 골드가 충분하면 true를 반환하고, 아니면 false를 반환합니다.
    /// </summary>
    /// <param name="amount">사용할 골드의 양입니다.</param>
    /// <returns>골드 사용 성공 여부입니다.</returns>
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldUI();
            Debug.Log($"골드 사용: {amount}. 현재 골드: {currentGold}");
            SaveGoldAndUpgradeLevels(); // 골드 변경 시 저장
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
        if (goldDisplayText != null)
        {
            goldDisplayText.text = $"골드: {currentGold}";
        }
    }

    // --- 업그레이드 관리 메서드 ---

    /// <summary>
    /// 특정 아이템의 현재 업그레이드 레벨을 가져옵니다.
    /// </summary>
    /// <param name="itemName">아이템의 이름입니다.</param>
    /// <returns>아이템의 현재 레벨을 반환하거나, 해당 아이템이 업그레이드 목록에 없으면 0을 반환합니다.</returns>
    public int GetItemUpgradeLevel(string itemName)
    {
        // Dictionary에서 아이템 이름으로 레벨을 찾고, 없으면 기본값 0을 반환합니다.
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

        string itemName = upgradeDefinition.targetItemData.itemName;
        int currentLevel = GetItemUpgradeLevel(itemName);

        // 최대 레벨에 도달했는지 확인
        if (currentLevel >= upgradeDefinition.maxLevel)
        {
            Debug.Log($"'{itemName}'은(는) 이미 최대 레벨입니다. (레벨: {currentLevel}/{upgradeDefinition.maxLevel})");
            return false;
        }

        // 다음 레벨 업그레이드 비용을 가져옵니다.
        int upgradeCost = upgradeDefinition.GetCostForLevel(currentLevel + 1);
        if (upgradeCost == -1)
        {
            Debug.LogWarning($"'{itemName}'의 레벨 {currentLevel + 1} 업그레이드 비용이 정의되지 않았습니다.");
            return false;
        }

        // 골드 사용 시도
        if (SpendGold(upgradeCost))
        {
            itemUpgradeLevels[itemName] = currentLevel + 1; // 레벨 증가
            Debug.Log($"'{itemName}'을(를) 레벨 {itemUpgradeLevels[itemName]}로 업그레이드했습니다! 비용: {upgradeCost}");
            SaveGoldAndUpgradeLevels(); // 레벨 변경 시 저장
            // ShopUI가 RefreshShopUI를 호출하여 UI를 갱신하도록 합니다.
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
        // allUpgradeDefinitions 목록에서 대상 아이템 이름과 일치하는 UpgradeDefinition을 찾아
        // 그 정의에 연결된 targetItemData를 반환합니다.
        // 이는 ShopUI 등에서 UpgradeDefinition을 통해 ItemData에 접근할 때 사용됩니다.
        var upgradeDef = allUpgradeDefinitions.Find(def => def.targetItemData != null && def.targetItemData.itemName == itemName);
        return upgradeDef?.targetItemData;
    }

    // --- 저장/로드 시스템 ---

    // 저장 데이터를 위한 직렬화 가능한 클래스
    [System.Serializable]
    private class ShopSaveData
    {
        public int gold; // 현재 골드
        public List<UpgradeLevelEntry> upgradeLevels; // 아이템별 업그레이드 레벨 목록
    }

    // 각 아이템의 업그레이드 레벨 엔트리
    [System.Serializable]
    private class UpgradeLevelEntry
    {
        public string itemName;
        public int level;
    }

    /// <summary>
    /// 현재 골드와 모든 아이템의 업그레이드 레벨을 파일에 저장합니다.
    /// </summary>
    public void SaveGoldAndUpgradeLevels()
    {
        // Dictionary의 내용을 직렬화 가능한 List로 변환합니다.
        List<UpgradeLevelEntry> entries = itemUpgradeLevels
            .Select(kv => new UpgradeLevelEntry { itemName = kv.Key, level = kv.Value })
            .ToList();

        ShopSaveData data = new ShopSaveData { gold = currentGold, upgradeLevels = entries };
        string json = JsonUtility.ToJson(data, true); // (true: 가독성 좋게 포맷팅)
        File.WriteAllText(shopSavePath, json);
        Debug.Log("상점 데이터 및 업그레이드 레벨 저장됨.");
    }

    /// <summary>
    /// 저장된 골드와 아이템 업그레이드 레벨을 파일에서 불러옵니다.
    /// </summary>
    public void LoadGoldAndUpgradeLevels()
    {
        if (File.Exists(shopSavePath))
        {
            string json = File.ReadAllText(shopSavePath);
            ShopSaveData data = JsonUtility.FromJson<ShopSaveData>(json);

            currentGold = data.gold;
            itemUpgradeLevels.Clear(); // 기존 데이터 초기화

            // 불러온 레벨 데이터를 Dictionary에 다시 채웁니다.
            foreach (var entry in data.upgradeLevels)
            {
                itemUpgradeLevels[entry.itemName] = entry.level;
            }
            Debug.Log("상점 데이터 및 업그레이드 레벨 로드됨.");
        }
        else
        {
            Debug.Log("상점 저장 파일이 없습니다. 초기 골드 및 레벨로 시작합니다.");
            currentGold = 0; // 저장 파일이 없으면 골드를 0으로 초기화
        }
    }
}
