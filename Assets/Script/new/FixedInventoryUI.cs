using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FixedInventoryUI : MonoBehaviour
{
    public Image timeItemImage;
    public TextMeshProUGUI timeItemText;
    public Button timeItemButton;

    public Image lightItemImage;
    public TextMeshProUGUI lightItemText;
    public Button lightItemButton;

    public Sprite emptySlotSprite;

    public void RefreshUI()
    {
        var timeItem = InventorySystem_1.Instance.FindItemByName("시간아이템");
        var lightItem = InventorySystem_1.Instance.FindItemByName("빛아이템");

        SetSlot(timeItem, timeItemImage, timeItemText, timeItemButton);
        SetSlot(lightItem, lightItemImage, lightItemText, lightItemButton);
    }

    private void SetSlot(InventorySystem_1.InventoryItem_1 item, Image image, TextMeshProUGUI text, Button button)
    {
        button.onClick.RemoveAllListeners();

        if (item != null && item.itemData != null && item.itemData.icon != null)
        {
            image.sprite = item.itemData.icon;

            if (item.usedInCurrentScene)
            {
                image.color = Color.gray;
                text.text = "USED";
                button.interactable = false;
            }
            else
            {
                image.color = Color.white;
                text.text = item.count.ToString();
                button.interactable = true;

                button.onClick.AddListener(() =>
                {
                    Debug.Log("버튼 클릭됨!"); // ← 이게 실제 클릭 시 로그!
                    InventorySystem_1.Instance.UseInventoryItem(item);
                });
            }
        }
        else
        {
            image.sprite = emptySlotSprite;
            image.color = new Color(1, 1, 1, 0.3f);
            text.text = "";
            button.interactable = false;
        }
    }

}
