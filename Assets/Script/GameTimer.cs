using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeLimit = 60f;         // ���� �ð� (��)
    private float currentTime;

    public TextMeshProUGUI timerText;                // UI �ؽ�Ʈ�� Ÿ�̸� ǥ�� (����)
    public GameObject gameOverPanel;      // ���� ���� UI (����)

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

        // �����̽��� ġƮŰ: +5��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddTime(5f);
        }
    }

    //ġƮŰ
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