using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro�� ����ϹǷ� �ʿ��մϴ�.

public class ShopUI : MonoBehaviour
{
    [Header("UI ����")]
    public GameObject shopPanel; // ���� ��ü �г� (Ȱ��ȭ/��Ȱ��ȭ)
    public TextMeshProUGUI goldText; // ShopManager�� goldDisplayText�� ������ TextMeshProUGUI

    [Header("���׷��̵� ���� UI")]
    [Tooltip("Inspector���� �� ���׷��̵� �ɼǿ� �ش��ϴ� UI ��Ҹ� �����ϼ���.")]
    public List<ShopUpgradeSlotUI> upgradeSlots; // �� ���׷��̵� �ɼ� UI ��� ���

    void Start()
    {
        shopPanel.SetActive(false); // ���� ���� �� ���� UI�� ����
        RefreshShopUI(); // �ʱ� UI ����
    }

    /// <summary>
    /// ���� UI�� ���� �����մϴ�.
    /// </summary>
    public void OpenShop()
    {
        shopPanel.SetActive(true); // ���� �г� Ȱ��ȭ
        RefreshShopUI(); // ���� UI ���� ����
        Debug.Log("���� ����.");
    }

    /// <summary>
    /// ���� UI�� �ݽ��ϴ�.
    /// </summary>
    public void CloseShop()
    {
        shopPanel.SetActive(false); // ���� �г� ��Ȱ��ȭ
        Debug.Log("���� ����.");
    }

    /// <summary>
    /// ���� UI�� ��� ��Ҹ� ���� ���� ������ ���׷��̵� ������ ���� �����մϴ�.
    /// </summary>
    public void RefreshShopUI()
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogWarning("ShopManager.Instance�� ã�� �� �����ϴ�. ���� UI�� ������ �� �����ϴ�.");
            return;
        }

        // 1. ��� UI ���� (ShopManager�� �̹� TextMeshProUGUI�� ���� �����ϹǷ� ���⼭�� ���� ����)
        // ShopManager.Instance.UpdateGoldUI()�� ȣ��� ������ �ڵ����� ���ŵ˴ϴ�.
        // ������ Ȥ�� �� ��츦 ����� ���� ������ ���� �ֽ��ϴ�.
        if (goldText != null)
        {
            goldText.text = $"���: {ShopManager.Instance.currentGold}";
        }


        // 2. �� ���׷��̵� ���� UI ����
        foreach (ShopUpgradeSlotUI slot in upgradeSlots)
        {
            if (slot.linkedUpgradeDefinition == null)
            {
                Debug.LogWarning("ShopUpgradeSlotUI�� ����� UpgradeDefinition�� �����ϴ�.");
                continue;
            }

            UpgradeDefinition upgradeDef = slot.linkedUpgradeDefinition;
            string itemName = upgradeDef.targetItemData?.itemName;

            if (itemName == null)
            {
                Debug.LogWarning($"UpgradeDefinition '{upgradeDef.name}'�� ��ȿ�� targetItemData�� ������� �ʾҽ��ϴ�.");
                slot.upgradeButton.interactable = false;
                slot.itemNameText.text = "����: ������ ����";
                slot.levelText.text = "";
                slot.costText.text = "";
                slot.effectDescriptionText.text = "";
                continue;
            }

            int currentLevel = ShopManager.Instance.GetItemUpgradeLevel(itemName);
            int nextCost = upgradeDef.GetCostForLevel(currentLevel + 1);

            // UI �ؽ�Ʈ ����
            slot.itemNameText.text = upgradeDef.upgradeTitle;
            slot.levelText.text = $"����: {currentLevel}/{upgradeDef.maxLevel}";

            // ȿ�� ���� �ؽ�Ʈ ���� (���� �� -> ���� ��)
            string currentEffect = upgradeDef.GetCurrentEffectValueString(currentLevel);
            string nextEffect = upgradeDef.GetNextEffectValueString(currentLevel);
            if (currentLevel < upgradeDef.maxLevel)
            {
                slot.effectDescriptionText.text = string.Format(upgradeDef.effectDescriptionFormat, currentEffect, nextEffect);
            }
            else
            {
                slot.effectDescriptionText.text = $"{upgradeDef.effectDescriptionFormat.Replace("{0} -> {1}", currentEffect)} (�ִ� ����)";
            }


            // ��ư ��ȣ�ۿ� ���� ���� ����
            if (currentLevel >= upgradeDef.maxLevel)
            {
                slot.costText.text = "MAX";
                slot.upgradeButton.interactable = false; // �ִ� �����̸� ��Ȱ��ȭ
            }
            else if (ShopManager.Instance.currentGold >= nextCost)
            {
                slot.costText.text = $"���: {nextCost} ���";
                slot.upgradeButton.interactable = true; // ��尡 ����ϸ� Ȱ��ȭ
            }
            else
            {
                slot.costText.text = $"���: {nextCost} ���"; // ��� ���� �ÿ��� ����� ǥ��
                slot.upgradeButton.interactable = false; // ��� ���� �� ��Ȱ��ȭ
            }

            // ��ư Ŭ�� ������ �߰� (��Ÿ�ӿ� �������� �߰�)
            // ���� ������ ���� �� �߰��Ͽ� �ߺ� ȣ�� ����
            slot.upgradeButton.onClick.RemoveAllListeners();
            slot.upgradeButton.onClick.AddListener(() => OnUpgradeButtonClicked(upgradeDef));
        }
    }

    /// <summary>
    /// ���׷��̵� ��ư�� Ŭ���Ǿ��� �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="upgradeDef">Ŭ���� ��ư�� ����� UpgradeDefinition �����Դϴ�.</param>
    public void OnUpgradeButtonClicked(UpgradeDefinition upgradeDef)
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogWarning("ShopManager.Instance�� ã�� �� �����ϴ�.");
            return;
        }

        // ShopManager�� ���� ���׷��̵带 �õ��մϴ�.
        if (ShopManager.Instance.TryUpgradeItem(upgradeDef))
        {
            // ���׷��̵� ���� �� UI�� �ٽ� �����Ͽ� ����� ����, ��� ���� �ݿ��մϴ�.
            RefreshShopUI();
        }
        // ���� �� (��� ����, �ִ� ���� ��) ShopManager���� �̹� �α׸� ����մϴ�.
    }

    // �� ���� UI�� �� ��ҿ� ���� ����ü (Inspector���� �����ϱ� �����ϵ���)
    // �� Ŭ������ ShopUI ��ũ��Ʈ�� ������ ���ǵ˴ϴ�.
    [System.Serializable]
    public class ShopUpgradeSlotUI
    {
        [Tooltip("�� UI ������ � ���׷��̵� ���ǿ� ������� �Ҵ��ϼ���.")]
        public UpgradeDefinition linkedUpgradeDefinition; // ����� UpgradeDefinition ����

        public TextMeshProUGUI itemNameText;     // ������ �̸�/���׷��̵� ���� ǥ��
        public TextMeshProUGUI levelText;        // ���� ���� ǥ�� (��: "Lv. 1/3")
        public TextMeshProUGUI costText;         // ���� ���� ���׷��̵� ��� ǥ��
        public TextMeshProUGUI effectDescriptionText; // ȿ�� ���� ���� (��: "�ð�: 5�� -> 6��")
        public Button upgradeButton;              // ���׷��̵� ��ư
    }
}
