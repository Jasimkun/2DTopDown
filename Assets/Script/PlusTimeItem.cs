using UnityEngine;

public class TimeBoostItem : MonoBehaviour
{
    public float timeIncrease = 5f; // 증가할 시간

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어와 충돌했을 때만 작동
        {
            GameTimer gameTimer = FindObjectOfType<GameTimer>();
            if (gameTimer != null)
            {
                gameTimer.AddTime(timeIncrease); // 시간 증가
                Destroy(gameObject); // 아이템 제거
            }
        }
    }
}