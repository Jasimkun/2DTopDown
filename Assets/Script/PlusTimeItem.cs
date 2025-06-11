using UnityEngine;

public class TimeBoostItem : MonoBehaviour
{
    public float timeIncrease = 5f; // ������ �ð�

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾�� �浹���� ���� �۵�
        {
            GameTimer gameTimer = FindObjectOfType<GameTimer>();
            if (gameTimer != null)
            {
                gameTimer.AddTime(timeIncrease); // �ð� ����
                Destroy(gameObject); // ������ ����
            }
        }
    }
}