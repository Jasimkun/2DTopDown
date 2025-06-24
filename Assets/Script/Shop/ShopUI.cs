using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하므로 필요합니다.

// 상점 업그레이드 슬롯의 UI 요소를 묶어 관리하는 직렬화 가능한 클래스
[System.Serializable]
public class ShopUpgradeSlotUI
{
    [Tooltip("이 UI 슬롯이 어떤 업그레이드 정의에 연결될지 할당하세요.")]
    public UpgradeDefinition linkedUpgradeDefinition; // 연결될 UpgradeDefinition 에셋

    [Tooltip("아이템 아이콘을 표시할 Image 컴포넌트를 연결하세요.")]
    public Image itemIconImage; // 아이템 아이콘 이미지 (추가)

    public TextMeshProUGUI itemNameText;     // 아이템 이름/업그레이드 제목 표시
    public TextMeshProUGUI levelText;        // 현재 레벨 표시 (예: "Lv. 1/3")
    public TextMeshProUGUI costText;         // 다음 레벨 업그레이드 비용 표시
    public TextMeshProUGUI effectDescriptionText; // 효과 변경 설명 (예: "시간: 5초 -> 6초")
    public Button upgradeButton;              // 업그레이드 버튼
}

// 상점 UI를 전체적으로 관리하는 스크립트
public class ShopUI : MonoBehaviour
{
    [Header("UI 참조")]
    [Tooltip("상점 전체 패널 GameObject를 연결하세요.")]
    public GameObject shopPanel; // 상점 전체 패널
    [Tooltip("골드를 표시할 TextMeshProUGUI 컴포넌트를 연결하세요.")]
    public TextMeshProUGUI goldText; // 골드 표시 텍스트
    [Tooltip("상점 닫기 버튼을 연결하세요.")]
    public Button closeButton; // 닫기 버튼 참조

    [Header("업그레이드 슬롯 UI")]
    [Tooltip("각 업그레이드 슬롯의 UI 컴포넌트들을 연결하세요.")]
    public List<ShopUpgradeSlotUI> upgradeSlots; // 상점의 업그레이드 슬롯 목록

    [Header("상점 슬롯 기본 스프라이트")]
    [Tooltip("아이템 아이콘이 없을 경우 상점 슬롯에 표시될 기본 스프라이트입니다. (투명하지 않아야 합니다!)")]
    public Sprite defaultShopSlotSprite; // ⬇️ [추가] 상점 슬롯 기본 스프라이트 필드

    void Start()
    {
        Debug.Log("[ShopUI] Start 메서드 시작. shopPanel 비활성화.");
        shopPanel.SetActive(false); // 게임 시작 시 상점 UI는 숨김

        // 닫기 버튼에 클릭 리스너를 연결합니다.
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners(); // 기존 리스너 제거 (안전성)
            closeButton.onClick.AddListener(CloseShop); // CloseShop 메서드 연결
            Debug.Log("[ShopUI] 닫기 버튼에 CloseShop 리스너 연결됨.");
        }
        else
        {
            Debug.LogWarning("[ShopUI] 'Close Button'이 ShopUI Inspector에 할당되지 않았습니다. 상점 닫기 버튼이 작동하지 않을 수 있습니다.");
        }

        RefreshShopUI(); // 초기 UI 갱신
        Debug.Log("[ShopUI] Start 메서드 완료.");
    }

    void OnEnable()
    {
        Debug.Log("[ShopUI] OnEnable 메서드 시작. RefreshShopUI 호출.");
        RefreshShopUI(); // 상점 패널이 활성화될 때마다 UI 내용 갱신
        Debug.Log("[ShopUI] OnEnable 메서드 완료.");
    }

    /// <summary>
    /// 상점 UI를 엽니다.
    /// </summary>
    public void OpenShop()
    {
        Debug.Log("[ShopUI] OpenShop 메서드 호출됨.");
        shopPanel.SetActive(true); // 패널 활성화
        RefreshShopUI(); // UI 내용 갱신
        Debug.Log("[ShopUI] 상점 열림.");
        Time.timeScale = 0f; // 상점 열면 게임 시간 정지
    }

    /// <summary>
    /// 상점 UI를 닫습니다.
    /// </summary>
    public void CloseShop()
    {
        Debug.Log("[ShopUI] CloseShop 메서드 호출됨.");
        shopPanel.SetActive(false); // 패널 비활성화
        Debug.Log("[ShopUI] 상점 닫힘.");
        Time.timeScale = 1f; // 상점 닫으면 게임 시간 재개
    }

    /// <summary>
    /// 상점 UI의 모든 내용을 최신 상태로 갱신합니다.
    /// 골드 표시, 각 업그레이드 슬롯의 텍스트와 버튼 상태를 업데이트합니다.
    /// </summary>
    public void RefreshShopUI()
    {
        Debug.Log("[ShopUI] RefreshShopUI 메서드 시작.");

        if (ShopManager.Instance == null)
        {
            Debug.LogError("ShopManager.Instance가 Null입니다. ShopManager GameObject가 씬에 있는지, 스크립트 실행 순서가 올바른지 확인하세요.");
            return;
        }
        Debug.Log($"[ShopUI] ShopManager.Instance 찾음. 현재 골드: {ShopManager.Instance.currentGold}");

        // 골드 텍스트 갱신
        if (goldText != null)
        {
            goldText.text = $"골드: {ShopManager.Instance.currentGold}";
        }
        else
        {
            Debug.LogWarning("[ShopUI] goldText (골드 표시 텍스트)가 ShopUI Inspector에 할당되지 않았습니다.");
        }

        // 업그레이드 슬롯 리스트 유효성 검사
        if (upgradeSlots == null || upgradeSlots.Count == 0)
        {
            Debug.LogError("ShopUI의 'Upgrade Slots' 리스트가 비어있거나 Null입니다. Inspector에서 슬롯을 추가하고 연결해주세요!");
            return;
        }
        Debug.Log($"[ShopUI] 'Upgrade Slots' 리스트에 {upgradeSlots.Count}개의 슬롯이 있습니다.");

        // 각 업그레이드 슬롯을 갱신합니다.
        for (int i = 0; i < upgradeSlots.Count; i++)
        {
            ShopUpgradeSlotUI slot = upgradeSlots[i];
            Debug.Log($"\n[ShopUI] --- 슬롯 {i} 처리 시작 ---");

            // 슬롯의 UI 컴포넌트들이 할당되었는지 확인합니다.
            if (slot.itemNameText == null) { Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Item Name Text'가 Null입니다. Inspector에서 연결해주세요! 경로: {GetGameObjectPath(slot.itemNameText?.gameObject)}"); continue; }
            if (slot.levelText == null) { Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Level Text'가 Null입니다. Inspector에서 연결해주세요! 경로: {GetGameObjectPath(slot.levelText?.gameObject)}"); continue; }
            if (slot.costText == null) { Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Cost Text'가 Null입니다. Inspector에서 연결해주세요! 경로: {GetGameObjectPath(slot.costText?.gameObject)}"); continue; }
            if (slot.effectDescriptionText == null) { Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Effect Description Text'가 Null입니다. Inspector에서 연결해주세요! 경로: {GetGameObjectPath(slot.effectDescriptionText?.gameObject)}"); continue; }
            if (slot.upgradeButton == null) { Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Upgrade Button'이 Null입니다. Inspector에서 연결해주세요! 경로: {GetGameObjectPath(slot.upgradeButton?.gameObject)}"); continue; }
            // ⬇️ [추가] 아이템 아이콘 이미지 컴포넌트 Null 체크
            if (slot.itemIconImage == null) { Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Item Icon Image'가 Null입니다. Inspector에서 연결해주세요! 경로: {GetGameObjectPath(slot.itemIconImage?.gameObject)}"); continue; }

            // 슬롯에 연결된 UpgradeDefinition 에셋이 유효한지 확인합니다.
            if (slot.linkedUpgradeDefinition == null)
            {
                Debug.LogError($"[ShopUI] 오류: 슬롯 {i}의 'Linked Upgrade Definition'이 Null입니다. Inspector에서 UpgradeDefinition 에셋을 할당해주세요!");
                slot.itemNameText.text = "오류: 정의 없음";
                slot.levelText.text = "";
                slot.costText.text = "";
                slot.effectDescriptionText.text = "";
                slot.upgradeButton.interactable = false;
                slot.itemIconImage.sprite = defaultShopSlotSprite; // 기본 이미지로 설정
                slot.itemIconImage.color = Color.gray; // 비활성화 느낌
                continue;
            }

            UpgradeDefinition upgradeDef = slot.linkedUpgradeDefinition;
            Debug.Log($"[ShopUI] 슬롯 {i}에 연결된 UpgradeDefinition: '{upgradeDef.name}'.");

            // UpgradeDefinition의 대상 아이템 데이터가 유효한지 확인합니다.
            if (upgradeDef.targetItemData == null)
            {
                Debug.LogError($"[ShopUI] 오류: UpgradeDefinition '{upgradeDef.name}'의 'Target Item Data'가 Null입니다. BaseItemData 에셋을 할당해주세요!");
                slot.itemNameText.text = "오류: 아이템 데이터 없음";
                slot.levelText.text = "";
                slot.costText.text = "";
                slot.effectDescriptionText.text = "";
                slot.upgradeButton.interactable = false;
                slot.itemIconImage.sprite = defaultShopSlotSprite; // 기본 이미지로 설정
                slot.itemIconImage.color = Color.gray; // 비활성화 느낌
                continue;
            }

            string itemName = upgradeDef.targetItemData.itemName;
            int currentLevel = ShopManager.Instance.GetItemUpgradeLevel(itemName); // 현재 업그레이드 레벨
            int nextCost = upgradeDef.GetCostForLevel(currentLevel + 1); // 다음 업그레이드 비용

            Debug.Log($"[ShopUI] 슬롯 {i} ({itemName}) - 현재 골드: {ShopManager.Instance.currentGold}, 다음 비용: {nextCost}");
            Debug.Log($"[ShopUI] 슬롯 {i} ({itemName}) - 현재 레벨: {currentLevel}, 최대 레벨: {upgradeDef.maxLevel}");

            // ⬇️ [추가] 아이템 아이콘 이미지 설정
            if (slot.itemIconImage != null)
            {
                slot.itemIconImage.sprite = upgradeDef.targetItemData.icon;
                if (slot.itemIconImage.sprite == null) // 아이템 아이콘이 없을 경우 기본 상점 슬롯 스프라이트 사용
                {
                    Debug.LogWarning($"[ShopUI] 아이템 '{upgradeDef.targetItemData.itemName}'의 아이콘이 null입니다. defaultShopSlotSprite 사용 시도.");
                    slot.itemIconImage.sprite = defaultShopSlotSprite;
                }
                slot.itemIconImage.color = Color.white; // 기본적으로 흰색
            }

            // UI 텍스트 업데이트
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

            // 버튼 상호작용 가능 여부 설정
            slot.upgradeButton.onClick.RemoveAllListeners(); // 기존 리스너 제거
            slot.upgradeButton.onClick.AddListener(() => OnUpgradeButtonClicked(upgradeDef)); // 새 리스너 추가

            if (currentLevel >= upgradeDef.maxLevel)
            {
                slot.costText.text = "MAX";
                slot.upgradeButton.interactable = false;
                if (slot.itemIconImage != null) slot.itemIconImage.color = Color.gray; // ⬇️ [수정] 최대 레벨 시 아이콘도 회색으로
                Debug.Log($"[ShopUI] 슬롯 {i} ({itemName}): 최대 레벨 도달. 버튼 비활성화.");
            }
            else if (ShopManager.Instance.currentGold >= nextCost)
            {
                slot.costText.text = $"비용: {nextCost} 골드";
                slot.upgradeButton.interactable = true;
                if (slot.itemIconImage != null) slot.itemIconImage.color = Color.white; // ⬇️ [수정] 업그레이드 가능 시 아이콘 흰색으로
                Debug.Log($"[ShopUI] 슬롯 {i} ({itemName}): 골드 충분. 버튼 활성화!");
            }
            else
            {
                slot.costText.text = $"비용: {nextCost} 골드";
                slot.upgradeButton.interactable = false;
                if (slot.itemIconImage != null) slot.itemIconImage.color = Color.gray; // ⬇️ [수정] 골드 부족 시 아이콘 회색으로
                Debug.Log($"[ShopUI] 슬롯 {i} ({itemName}): 골드 부족. 버튼 비활성화.");
            }
            Debug.Log($"[ShopUI] --- 슬롯 {i} 처리 완료 ---\n");
        }
        Debug.Log("[ShopUI] RefreshShopUI 메서드 완료.");
    }

    /// <summary>
    /// 업그레이드 버튼이 클릭되었을 때 호출됩니다.
    /// </summary>
    /// <param name="upgradeDef">클릭된 버튼과 연결된 UpgradeDefinition 에셋입니다.</param>
    public void OnUpgradeButtonClicked(UpgradeDefinition upgradeDef)
    {
        Debug.Log($"[ShopUI] 업그레이드 버튼 클릭됨: '{upgradeDef.name}'. 대상 아이템: '{upgradeDef.targetItemData?.itemName ?? "없음"}'.");

        if (ShopManager.Instance == null)
        {
            Debug.LogError("ShopManager.Instance를 찾을 수 없습니다. 업그레이드 불가.");
            return;
        }

        // ShopManager를 통해 업그레이드를 시도합니다.
        if (ShopManager.Instance.TryUpgradeItem(upgradeDef))
        {
            RefreshShopUI(); // 업그레이드 성공 시 UI를 즉시 갱신합니다.
            Debug.Log($"[ShopUI] 업그레이드 성공! UI 갱신됨.");
        }
        else
        {
            Debug.LogWarning($"[ShopUI] 업그레이드 실패. 현재 골드: {ShopManager.Instance.currentGold}.");
        }
    }

    // GameObject의 전체 경로를 얻기 위한 헬퍼 메서드 (디버그용)
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
