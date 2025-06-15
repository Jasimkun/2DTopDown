using UnityEngine;

public class PlusTimeItem : MonoBehaviour
{
    [SerializeField] PlusTimeItemData data;

    public float GetTimeToAdd()
    {
        return data.timeToAdd;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameTimer timer = FindObjectOfType<GameTimer>();
            if (timer != null)
            {
                timer.AddTime(GetTimeToAdd());
            }
            Destroy(gameObject);
        }
    }
}
