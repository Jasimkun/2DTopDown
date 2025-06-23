using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하므로 필요합니다.

public class ShopUI : MonoBehaviour
{
    [Header("UI 참조")]
    public GameObject shopPanel; // 상점 전체 패널 (활성화/비활성화)
    public TextMeshProUGUI goldText; // ShopManager의 goldDisplayText와 동일한 TextMeshProUGUI

    [Header("업그레이드 슬롯 UI")]
    [Tooltip("Inspector에서 각 업그레이드 옵션에 해당하는 UI 요소를 연결하세요.")]
    public List<ShopUpgradeSlotUI> upgradeSlots; // 각 업그레이드 옵션 UI 요소 목록

    void Start()
    {
        shopPanel.SetActive(false); // 게임 시작 시 상점 UI는 숨김
        RefreshShopUI(); // 초기 UI 갱신
    }

    /// <summary>
    /// 상점 UI를 열고 갱신합니다.
    /// </summary>
    public void OpenShop()
    {
        shopPanel.SetActive(true); // 상점 패널 활성화
        RefreshShopUI(); // 상점 UI 내용 갱신
        Debug.Log("상점 열림.");
    }

    /// <summary>
    /// 상점 UI를 닫습니다.
    /// </summary>
    public void CloseShop()
    {
        shopPanel.SetActive(false); // 상점 패널 비활성화
        Debug.Log("상점 닫힘.");
    }

    /// <summary>
    /// 상점 UI의 모든 요소를 현재 골드와 아이템 업그레이드 레벨에 맞춰 갱신합니다.
    /// </summary>
    public void RefreshShopUI()
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogWarning("ShopManager.Instance를 찾을 수 없습니다. 상점 UI를 갱신할 수 없습니다.");
            return;
        }

        // 1. 골드 UI 갱신 (ShopManager가 이미 TextMeshProUGUI를 직접 관리하므로 여기서는 생략 가능)
        // ShopManager.Instance.UpdateGoldUI()가 호출될 때마다 자동으로 갱신됩니다.
        // 하지만 혹시 모를 경우를 대비해 직접 참조할 수도 있습니다.
        if (goldText != null)
        {
            goldText.text = $"골드: {ShopManager.Instance.currentGold}";
        }


        // 2. 각 업그레이드 슬롯 UI 갱신
        foreach (ShopUpgradeSlotUI slot in upgradeSlots)
        {
            if (slot.linkedUpgradeDefinition == null)
            {
                Debug.LogWarning("ShopUpgradeSlotUI에 연결된 UpgradeDefinition이 없습니다.");
                continue;
            }

            UpgradeDefinition upgradeDef = slot.linkedUpgradeDefinition;
            string itemName = upgradeDef.targetItemData?.itemName;

            if (itemName == null)
            {
                Debug.LogWarning($"UpgradeDefinition '{upgradeDef.name}'에 유효한 targetItemData가 연결되지 않았습니다.");
                slot.upgradeButton.interactable = false;
                slot.itemNameText.text = "오류: 아이템 없음";
                slot.levelText.text = "";
                slot.costText.text = "";
                slot.effectDescriptionText.text = "";
                continue;
            }

            int currentLevel = ShopManager.Instance.GetItemUpgradeLevel(itemName);
            int nextCost = upgradeDef.GetCostForLevel(currentLevel + 1);

            // UI 텍스트 갱신
            slot.itemNameText.text = upgradeDef.upgradeTitle;
            slot.levelText.text = $"레벨: {currentLevel}/{upgradeDef.maxLevel}";

            // 효과 설명 텍스트 갱신 (현재 값 -> 다음 값)
            string currentEffect = upgradeDef.GetCurrentEffectValueString(currentLevel);
            string nextEffect = upgradeDef.GetNextEffectValueString(currentLevel);
            if (currentLevel < upgradeDef.maxLevel)
            {
                slot.effectDescriptionText.text = string.Format(upgradeDef.effectDescriptionFormat, currentEffect, nextEffect);
            }
            else
            {
                slot.effectDescriptionText.text = $"{upgradeDef.effectDescriptionFormat.Replace("{0} -> {1}", currentEffect)} (최대 레벨)";
            }


            // 버튼 상호작용 가능 여부 설정
            if (currentLevel >= upgradeDef.maxLevel)
            {
                slot.costText.text = "MAX";
                slot.upgradeButton.interactable = false; // 최대 레벨이면 비활성화
            }
            else if (ShopManager.Instance.currentGold >= nextCost)
            {
                slot.costText.text = $"비용: {nextCost} 골드";
                slot.upgradeButton.interactable = true; // 골드가 충분하면 활성화
            }
            else
            {
                slot.costText.text = $"비용: {nextCost} 골드"; // 골드 부족 시에도 비용은 표시
                slot.upgradeButton.interactable = false; // 골드 부족 시 비활성화
            }

            // 버튼 클릭 리스너 추가 (런타임에 동적으로 추가)
            // 기존 리스너 제거 후 추가하여 중복 호출 방지
            slot.upgradeButton.onClick.RemoveAllListeners();
            slot.upgradeButton.onClick.AddListener(() => OnUpgradeButtonClicked(upgradeDef));
        }
    }

    /// <summary>
    /// 업그레이드 버튼이 클릭되었을 때 호출됩니다.
    /// </summary>
    /// <param name="upgradeDef">클릭된 버튼에 연결된 UpgradeDefinition 에셋입니다.</param>
    public void OnUpgradeButtonClicked(UpgradeDefinition upgradeDef)
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogWarning("ShopManager.Instance를 찾을 수 없습니다.");
            return;
        }

        // ShopManager를 통해 업그레이드를 시도합니다.
        if (ShopManager.Instance.TryUpgradeItem(upgradeDef))
        {
            // 업그레이드 성공 시 UI를 다시 갱신하여 변경된 레벨, 비용 등을 반영합니다.
            RefreshShopUI();
        }
        // 실패 시 (골드 부족, 최대 레벨 등) ShopManager에서 이미 로그를 출력합니다.
    }

    // 퀵 슬롯 UI의 각 요소에 대한 구조체 (Inspector에서 연결하기 용이하도록)
    // 이 클래스는 ShopUI 스크립트의 하위로 정의됩니다.
    [System.Serializable]
    public class ShopUpgradeSlotUI
    {
        [Tooltip("이 UI 슬롯이 어떤 업그레이드 정의에 연결될지 할당하세요.")]
        public UpgradeDefinition linkedUpgradeDefinition; // 연결될 UpgradeDefinition 에셋

        public TextMeshProUGUI itemNameText;     // 아이템 이름/업그레이드 제목 표시
        public TextMeshProUGUI levelText;        // 현재 레벨 표시 (예: "Lv. 1/3")
        public TextMeshProUGUI costText;         // 다음 레벨 업그레이드 비용 표시
        public TextMeshProUGUI effectDescriptionText; // 효과 변경 설명 (예: "시간: 5초 -> 6초")
        public Button upgradeButton;              // 업그레이드 버튼
    }
}
