// QuickSlotUIController.cs

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro�� ����Ѵٸ� �ʿ�

public class QuickSlotUIController : MonoBehaviour
{
    [Header("UI References")]
    public Button[] quickSlotButtons; // �� �� ������ Button ������Ʈ �߰�
    public Image[] quickSlotImages;
    public TextMeshProUGUI[] quickSlotCountTexts;

    [Header("Item Sprites")]
    public Sprite defaultSlotSprite;

    private InventorySystem inventorySystem;

    void Awake()
    {
        inventorySystem = FindObjectOfType<InventorySystem>();
        if (inventorySystem == null)
        {
            Debug.LogError("���� InventorySystem�� �����ϴ�. �� ���� UI�� ������ �� �����ϴ�.");
        }

        // --- �߰�: �� ��ư�� Ŭ�� �̺�Ʈ ������ �Ҵ� ---
        for (int i = 0; i < quickSlotButtons.Length; i++)
        {
            int slotIndex = i; // Ŭ���� ���� ������ ���� ���� ���� ���
            quickSlotButtons[i].onClick.AddListener(() => OnQuickSlotButtonClick(slotIndex));
            // �ʱ⿡�� ��ư ��Ȱ��ȭ (�������� ������ Ŭ�� �Ұ�)
            quickSlotButtons[i].interactable = false;
        }
        // --- �߰� �� ---

        RefreshQuickSlotsUI(); // Awake���� �ʱ� UI ����
    }

    // �� ���� ��ư Ŭ�� �� ȣ��� �޼���
    private void OnQuickSlotButtonClick(int slotIndex)
    {
        if (inventorySystem != null)
        {
            inventorySystem.UseInventoryItem(slotIndex);
        }
    }


    public void RefreshQuickSlotsUI()
    {
        if (inventorySystem == null)
        {
            Debug.LogWarning("InventorySystem�� �ʱ�ȭ���� �ʾ� �� ���� UI�� ������ �� �����ϴ�.");
            return;
        }

        // �� ���� ������ŭ �ݺ��ϸ� UI ������Ʈ
        for (int i = 0; i < quickSlotImages.Length; i++)
        {
            // --- �߰�: ��ư ���� Ȯ�� ---
            Button currentButton = (i < quickSlotButtons.Length) ? quickSlotButtons[i] : null;
            Image currentImage = quickSlotImages[i];
            TextMeshProUGUI currentText = quickSlotCountTexts[i];

            if (currentButton == null || currentImage == null || currentText == null)
            {
                Debug.LogWarning($"�� ���� UI ���� ����: {i}�� ������ ��ư/�̹���/�ؽ�Ʈ ������Ʈ�� �����Ǿ����ϴ�.");
                continue; // ���� �������� �Ѿ
            }
            // --- �߰� �� ---


            if (i < inventorySystem.items.Count) // �κ��丮 �������� ���� �������� ������
            {
                InventoryItem invItem = inventorySystem.items[i];
                PlusTimeItemData itemData = inventorySystem.GetItemDataByName(invItem.itemName);

                if (itemData != null)
                {
                    currentImage.sprite = itemData.icon; // PlusTimeItemData�� icon �ʵ尡 �ִٰ� ����
                    currentImage.color = Color.white;
                    //currentText.text = invItem.count.ToString();
                    currentButton.interactable = true; // �������� ������ ��ư Ȱ��ȭ
                }
                else
                {
                    currentImage.sprite = defaultSlotSprite;
                    currentImage.color = new Color(1, 1, 1, 0.5f);
                    currentText.text = "";
                    currentButton.interactable = false; // ������ ������ ������ ��Ȱ��ȭ
                    Debug.LogWarning($"������ '{invItem.itemName}'�� ���� PlusTimeItemData�� ã�� �� �����ϴ�.");
                }
            }
            else // �ش� ���Կ� ǥ���� �κ��丮 �������� ���� ���
            {
                currentImage.sprite = defaultSlotSprite;
                currentImage.color = new Color(1, 1, 1, 0.5f);
                currentText.text = "";
                currentButton.interactable = false; // ������ ������ ��ư ��Ȱ��ȭ
            }
        }
        //Debug.Log("�� ���� UI�� ���ŵǾ����ϴ�.");
    }
}