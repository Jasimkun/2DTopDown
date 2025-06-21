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
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
            //AddTime(5f);
        //}
    }

    //ġƮŰ
    public void AddTime(float seconds)
    {
        currentTime += seconds;
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            // Mathf.Max�� ����Ͽ� �ð��� ������ �Ǵ� ���� �����ϰ� �ּ� 0���� ǥ��
            float displayTime = Mathf.Max(0f, currentTime);
            // "F2" ������: �Ҽ��� ���� �� �ڸ����� ǥ�� (��: 9.99)
            timerText.text = displayTime.ToString("F2");
        }
    }

    void GameOver()
    {
        isGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        //Debug.Log("Game Over!");
    }
}