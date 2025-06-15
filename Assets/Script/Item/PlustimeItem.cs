using UnityEngine;

public class PlusTimeItem : MonoBehaviour
{
    public string itemName;

    [SerializeField] PlusTimeItemData data;

    public float GetTimeToAdd()
    {
        return data.timeToAdd;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //GameTimer timer = FindObjectOfType<GameTimer>();
            //if (timer != null)
            //{
                //timer.AddTime(GetTimeToAdd());
            //}

            InventorySystem inventory = FindObjectOfType<InventorySystem>();
            if (inventory != null)
            {
                inventory.AddItem(data);  // 인벤토리에 아이템 추가
            }

            Destroy(gameObject);
        }
    }
}
