using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeLimit = 60f;         // 제한 시간 (초)
    private float currentTime;

    public TextMeshProUGUI timerText;                // UI 텍스트로 타이머 표시 (선택)
    public GameObject gameOverPanel;      // 게임 오버 UI (선택)

    private bool isGameOver = false;

    void Start()
    {
        currentTime = timeLimit;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        currentTime -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTime <= 0)
        {
            GameOver();
        }

        // 스페이스바 치트키: +5초
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddTime(5f);
        }
    }

    //치트키
    public void AddTime(float seconds)
    {
        currentTime += seconds;
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    void GameOver()
    {
        isGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("Game Over!");
    }
}