using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro를 사용하기 위해 필요합니다.

public class GameTimer : MonoBehaviour
{
    public float timeLimit = 60f; // 제한 시간
    private float currentTime;    // 현재 남은 시간
    private float mazeClearTimeAtEnd = 0f; // ⬇️ [추가] 미로 클리어 시 남은 시간을 저장할 변수

    public TextMeshProUGUI timerText; // 시간을 표시할 TextMeshProUGUI 컴포넌트
    public GameObject gameOverPanel;  // 게임 오버 시 활성화할 패널

    private bool isGameOver = false; // 게임 오버 상태 여부

    void Start()
    {
        currentTime = timeLimit; // 초기 시간 설정
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // 시작 시 게임 오버 패널 숨김
    }

    void Update()
    {
        if (isGameOver) return; // 게임 오버 상태면 업데이트 중지

        currentTime -= Time.deltaTime; // 시간 감소
        UpdateTimerUI(); // UI 업데이트

        if (currentTime <= 0) // 시간이 다 되면 게임 오버
        {
            GameOver();
        }

        // 테스트용 치트키: 스페이스바 누르면 시간 증가 (테스트 후 삭제하거나 비활성화하세요)
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddTime(10f); // 10초 추가
            Debug.Log("10초 추가됨!");
        }
        */
    }

    /// <summary>
    /// 현재 시간에 지정된 초만큼 추가합니다.
    /// </summary>
    /// <param name="seconds">추가할 시간 (초).</param>
    public void AddTime(float seconds)
    {
        currentTime += seconds;
        Debug.Log($"시간 아이템 사용: {seconds}초 추가됨. 현재 시간: {currentTime:F2}초.");

        // 추가된 시간으로 인해 게임 오버 상태가 아닐 경우만 처리
        if (isGameOver && currentTime > 0)
        {
            isGameOver = false; // 시간을 추가해서 게임 오버 상태 해제
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
            Debug.Log("시간 아이템 사용으로 게임 오버가 해제되었습니다!");
        }
    }

    /// <summary>
    /// 타이머 UI를 갱신합니다.
    /// </summary>
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            float displayTime = Mathf.Max(0f, currentTime); // 시간이 0보다 작아지지 않도록
            timerText.text = displayTime.ToString("F2"); // 소수점 두 자리까지 표시
        }
    }

    /// <summary>
    /// 게임 오버 처리 로직입니다.
    /// </summary>
    void GameOver()
    {
        currentTime = 0; // 시간 0으로 고정
        UpdateTimerUI(); // 최종 UI 업데이트

        isGameOver = true; // 게임 오버 상태로 설정

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true); // 게임 오버 패널 활성화

        Debug.Log("Game Over!");

        // ⬇️ [추가] 게임 오버 시 상점 매니저에게 미로 클리어 시간 보고 (실패 시)
        // mazeClearTimeAtEnd는 클리어 성공 시 사용되므로, 여기서는 골드 보상 로직을 생략합니다.
        // 상점은 미로 클리어 성공 시에만 골드 보상을 줄 것이므로, GameOver 시에는 골드 보상 없음.
        // 예를 들어, 플레이어가 목적지에 도달했을 때 이벤트를 호출하여 골드를 획득하게 해야 합니다.
    }

    /// <summary>
    /// ⬇️ [추가] 미로 클리어 성공 시 호출되는 메서드 (PlayerController 등에서 호출)
    /// </summary>
    public void MazeClearedSuccessfully()
    {
        if (isGameOver) return; // 이미 게임 오버된 상태라면 처리하지 않음

        mazeClearTimeAtEnd = timeLimit - currentTime; // 사용한 시간 계산
        Debug.Log($"미로 클리어 성공! 소요 시간: {mazeClearTimeAtEnd:F2}초.");

        // 여기에서 ShopManager를 통해 골드를 보상합니다.
        // 예시: 남은 시간이 많을수록 (즉, 클리어 시간이 짧을수록) 더 많은 골드
        int goldReward = CalculateGoldReward(mazeClearTimeAtEnd);
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.AddGold(goldReward);
            Debug.Log($"미로 클리어 보상으로 골드 {goldReward}개 획득!");
        }
        else
        {
            Debug.LogWarning("ShopManager.Instance를 찾을 수 없습니다. 골드를 보상할 수 없습니다.");
        }

        // 게임 정지 또는 다음 씬 로드 등의 로직 추가
        // Time.timeScale = 0f; // 게임 일시 정지 (상점 오픈 등)
        // SceneManager.LoadScene("ShopScene"); // 상점 씬으로 이동 (예시)

        isGameOver = true; // 클리어 성공 시에도 타이머는 더 이상 작동하지 않음
    }

    /// <summary>
    /// ⬇️ [추가] 미로 클리어 시간에 따른 골드 보상을 계산합니다.
    /// </summary>
    /// <param name="clearTime">미로를 클리어하는 데 걸린 시간입니다.</param>
    /// <returns>계산된 골드 보상 양입니다.</returns>
    private int CalculateGoldReward(float clearTime)
    {
        // 간단한 예시: 시간이 짧을수록 보상 증가 (시간 제한이 60초라고 가정)
        // 1초당 1골드 (최대 60골드)
        int reward = Mathf.Max(0, Mathf.FloorToInt(timeLimit - clearTime));
        // 또는 복잡한 공식: 예) (MaxTime - ClearTime) * BonusFactor
        // 예시: 30초 이내 클리어 시 50골드, 60초 이내 시 20골드 등
        return reward;
    }
}
