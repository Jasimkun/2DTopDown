using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("UI 참조")]
    public GameObject shopPanel;
    public TextMeshProUGUI goldText;

    [Header("업그레이드 슬롯 UI")]
    public List<ShopUpgradeSlotUI> upgradeSlots;

    void Start()
    {
        Debug.Log("[ShopUI] Start 메서드 시작. shopPanel 비활성화.");
        shopPanel.SetActive(false); // 게임 시작 시 상점 UI는 숨김 (OnEnable에서 갱신)
        // RefreshShopUI(); // ⬇️ [수정] Start에서는 RefreshShopUI 호출하지 않음 (OnEnable에서 할 것임)
        Debug.Log("[ShopUI] Start 메서드 완료.");
    }

    void OnEnable()
    {
        // ⬇️ [추가] GameObject가 활성화될 때마다 UI를 갱신합니다. (씬 로드 후에도 확실하게 갱신)
        Debug.Log("[ShopUI] OnEnable 메서드 시작. RefreshShopUI 호출.");
        RefreshShopUI();
        Debug.Log("[ShopUI] OnEnable 메서드 완료.");
    }

    public void OpenShop()
    {
        Debug.Log("[ShopUI] OpenShop 메서드 호출됨.");
        shopPanel.SetActive(true);
        RefreshShopUI(); // 상점 열릴 때도 갱신
        Debug.Log("[ShopUI] 상점 열림.");
    }

    public void CloseShop()
    {
        Debug.Log("[ShopUI] CloseShop 메서드 호출됨.");
        shopPanel.SetActive(false);
        Debug.Log("[ShopUI] 상점 닫힘.");
        Time.timeScale = 1f; // 게임 시간 재개
    }

    public void RefreshShopUI()
    {
        Debug.Log("[ShopUI] RefreshShopUI 메서드 시작.");

        if (ShopManager.Instance == null)
        {
            Debug.LogError("ShopManager.Instance가 Null입니다. ShopManager GameObject가 씬에 있는지, 스크립트 실행 순서가 올바른지 확인하세요.");
            return;
        }
        Debug.Log($"[ShopUI] ShopManager.Instance 찾음. 현재 골드: {ShopManager.Instance.currentGold}");

        if (goldText != null)
        {
            goldText.text = $"골드: {ShopManager.Instance.currentGold}";
        }
        else
        {
            Debug.LogWarning("[ShopUI] goldText (골드 표시 텍스트)가 ShopUI Inspector에 할당되지 않았습니다.");
        }

        if (upgradeSlots == null || upgradeSlots.Count == 0)
        {
            Debug.LogError("ShopUI의 'Upgrade Slots' 리스트가 비어있거나 Null입니다. Inspector에서 슬롯을 추가하고 연결해주세요!");
            return;
        }
        Debug.Log($"[ShopUI] 'Upgrade Slots' 리스트에 {upgradeSlots.Count}개의 슬롯이 있습니다.");


        for (int i = 0; i < upgradeSlots.Count; i++)
        {
            ShopUpgradeSlotUI slot = upgradeSlots[i];
            Debug.Log($"\n[ShopUI] --- 슬롯 {i} 처리 시작 ---");

            if (slot.itemNameText == null) { Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Item Name Text'가 Null입니다. Inspector에서 연결해주세요! 경로: {GetGameObjectPath(slot.itemNameText?.gameObject)}"); continue; }
            if (slot.levelText == null) { Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Level Text'가 Null입니다. Inspector에서 연결해주세요! 경로: {GetGameObjectPath(slot.levelText?.gameObject)}"); continue; }
            if (slot.costText == null) { Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Cost Text'가 Null입니다. Inspector에서 연결해주세요! 경로: {GetGameObjectPath(slot.costText?.gameObject)}"); continue; }
            if (slot.effectDescriptionText == null) { Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Effect Description Text'가 Null입니다. Inspector에서 연결해주세요! 경로: {GetGameObjectPath(slot.effectDescriptionText?.gameObject)}"); continue; }
            if (slot.upgradeButton == null) { Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Upgrade Button'이 Null입니다. Inspector에서 연결해주세요! 경로: {GetGameObjectPath(slot.upgradeButton?.gameObject)}"); continue; }

            if (slot.linkedUpgradeDefinition == null)
            {
                Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Linked Upgrade Definition'이 Null입니다. Inspector에서 UpgradeDefinition 에셋을 할당해주세요!");
                slot.itemNameText.text = "오류: 정의 없음";
                slot.levelText.text = "";
                slot.costText.text = "";
                slot.effectDescriptionText.text = "";
                slot.upgradeButton.interactable = false;
                continue;
            }

            UpgradeDefinition upgradeDef = slot.linkedUpgradeDefinition;
            Debug.Log($"[ShopUI] 슬롯 {i}에 연결된 UpgradeDefinition: '{upgradeDef.name}'.");

            if (upgradeDef.targetItemData == null)
            {
                Debug.LogError($"[ShopUI] 오류: UpgradeDefinition '{upgradeDef.name}'의 'Target Item Data'가 Null입니다. BaseItemData 에셋을 할당해주세요!");
                slot.itemNameText.text = "오류: 아이템 데이터 없음";
                slot.levelText.text = "";
                slot.costText.text = "";
                slot.effectDescriptionText.text = "";
                slot.upgradeButton.interactable = false;
                continue;
            }

            string itemName = upgradeDef.targetItemData.itemName;
            int currentLevel = ShopManager.Instance.GetItemUpgradeLevel(itemName);
            int nextCost = upgradeDef.GetCostForLevel(currentLevel + 1);

            Debug.Log($"[ShopUI] 슬롯 {i} ({itemName}) - 현재 골드: {ShopManager.Instance.currentGold}, 다음 비용: {nextCost}");
            Debug.Log($"[ShopUI] 슬롯 {i} ({itemName}) - 현재 레벨: {currentLevel}, 최대 레벨: {upgradeDef.maxLevel}");

            slot.itemNameText.text = upgradeDef.upgradeTitle;
            slot.levelText.text = $"레벨: {currentLevel}/{upgradeDef.maxLevel}";

            string currentEffect = upgradeDef.GetCurrentEffectValueString(currentLevel);
            string nextEffect = upgradeDef.GetNextEffectValueString(currentLevel);
            if (currentLevel < upgradeDef.maxLevel)
            {
                slot.effectDescriptionText.text = string.Format(upgradeDef.effectDescriptionFormat, currentEffect, nextEffect);
            }
            else
            {
                slot.effectDescriptionText.text = $"{string.Format(upgradeDef.effectDescriptionFormat.Replace("{0} -> {1}", "{0}"), currentEffect)} (최대 레벨)";
            }

            if (currentLevel >= upgradeDef.maxLevel)
            {
                slot.costText.text = "MAX";
                slot.upgradeButton.interactable = false;
                Debug.Log($"[ShopUI] 슬롯 {i} ({itemName}): 최대 레벨 도달. 버튼 비활성화.");
            }
            else if (ShopManager.Instance.currentGold >= nextCost)
            {
                slot.costText.text = $"비용: {nextCost} 골드";
                slot.upgradeButton.interactable = true;
                Debug.Log($"[ShopUI] 슬롯 {i} ({itemName}): 골드 충분. 버튼 활성화!");
            }
            else
            {
                slot.costText.text = $"비용: {nextCost} 골드";
                slot.upgradeButton.interactable = false;
                Debug.Log($"[ShopUI] 슬롯 {i} ({itemName}): 골드 부족. 버튼 비활성화.");
            }

            slot.upgradeButton.onClick.RemoveAllListeners();
            slot.upgradeButton.onClick.AddListener(() => OnUpgradeButtonClicked(upgradeDef));
            Debug.Log($"[ShopUI] --- 슬롯 {i} 처리 완료 ---\n");
        }
        Debug.Log("[ShopUI] RefreshShopUI 메서드 완료.");
    }

    public void OnUpgradeButtonClicked(UpgradeDefinition upgradeDef)
    {
        Debug.Log($"[ShopUI] 업그레이드 버튼 클릭됨: '{upgradeDef.name}'. 대상 아이템: '{upgradeDef.targetItemData?.itemName ?? "없음"}'.");

        if (ShopManager.Instance == null)
        {
            Debug.LogError("ShopManager.Instance를 찾을 수 없습니다. 업그레이드 불가.");
            return;
        }

        if (ShopManager.Instance.TryUpgradeItem(upgradeDef))
        {
            RefreshShopUI();
            Debug.Log($"[ShopUI] 업그레이드 성공! UI 갱신됨.");
        }
        else
        {
            Debug.LogWarning($"[ShopUI] 업그레이드 실패. 현재 골드: {ShopManager.Instance.currentGold}.");
        }
    }

    [System.Serializable]
    public class ShopUpgradeSlotUI
    {
        public UpgradeDefinition linkedUpgradeDefinition;
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI costText;
        public TextMeshProUGUI effectDescriptionText;
        public Button upgradeButton;
    }

    private string GetGameObjectPath(GameObject obj)
    {
        if (obj == null) return "Null GameObject";
        string path = obj.name;
        Transform current = obj.transform;
        while (current.parent != null)
        {
            current = current.parent;
            path = current.name + "/" + path;
        }
        return path;
    }
}
