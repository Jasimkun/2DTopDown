using UnityEngine;
using System.Collections; // Coroutine을 사용하기 위해 추가

// 이 스크립트는 게임의 전반적인 상태(시작, 종료)를 관리하고,
// 게임 클리어 시 남은 시간에 따라 골드를 지급하는 로직을 처리합니다.
// 싱글턴 패턴을 사용하여 게임 내 어디에서든 쉽게 접근할 수 있습니다.
public class GameManager : MonoBehaviour
{
    // 싱글턴 인스턴스
    public static GameManager Instance { get; private set; }

    // 게임 시작 시간 (초)
    private float _gameStartTime;
    // 게임이 현재 진행 중인지 여부
    private bool _isGameRunning = false;

    [Header("골드 보상 설정 (남은 시간 기준)")]
    [Tooltip("이 시간 내에 게임을 클리어해야 골드를 받을 수 있습니다. 게임의 최대 허용 시간입니다.")]
    [SerializeField] private float _maxAllowedGameTime = 60f; // 예시: 게임 클리어까지 최대 60초 허용

    private void Awake()
    {
        // 싱글턴 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 파괴되지 않도록 설정
            Debug.Log("[GameManager] GameManager 초기화 완료.");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // 게임 시작 시 바로 게임을 시작할 수도 있고,
        // 특정 이벤트(예: 시작 버튼 클릭)에 의해 시작할 수도 있습니다.
        // 예시로 Start에서 바로 시작하도록 합니다.
        StartGame();
    }

    // 게임 시작 메서드
    public void StartGame()
    {
        if (_isGameRunning)
        {
            Debug.LogWarning("[GameManager] 게임이 이미 진행 중입니다!");
            return;
        }

        _gameStartTime = Time.time; // 현재 게임 시간을 기록
        _isGameRunning = true;
        Debug.Log("[GameManager] 게임 시작! 시작 시간: " + _gameStartTime);

        // Debug.Log("[GameManager] 테스트: 5초 후 게임 클리어 시뮬레이션 시작...");
        // StartCoroutine(SimulateGameClearAfterDelay(5f)); // 5초 후 클리어 시뮬레이션
        // Debug.Log("[GameManager] 테스트: 65초 후 게임 클리어 시뮬레이션 시작 (시간 초과)...");
        // StartCoroutine(SimulateGameClearAfterDelay(65f)); // 65초 후 클리어 시뮬레이션
    }

    // 게임을 클리어하고 골드를 지급하는 메서드 (게임 클리어 조건이 충족될 때 호출)
    public void ClearGameAndAwardGold()
    {
        if (!_isGameRunning)
        {
            Debug.LogWarning("[GameManager] 게임이 시작되지 않았는데 클리어하려 합니다!");
            return;
        }

        _isGameRunning = false; // 게임 종료 상태로 변경
        float clearTime = Time.time - _gameStartTime; // 클리어 시간 계산

        Debug.Log($"[GameManager] 게임 클리어! 소요 시간: {clearTime:F2} 초");

        int awardedGold = CalculateGoldReward(clearTime);

        // CurrencyManager가 초기화되었는지 확인 후 골드 지급
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddGold(awardedGold);
            Debug.Log($"[GameManager] 클리어 보상으로 골드 {awardedGold}개 지급!");
        }
        else
        {
            Debug.LogError("[GameManager] CurrencyManager 인스턴스를 찾을 수 없습니다. 골드 지급 실패!");
        }

        // 게임 클리어 후 추가적인 로직 (예: 결과 화면 표시, 다음 레벨 로드 등)
        // 여기에 추가 구현
    }

    // 클리어 시간에 따라 골드 보상을 계산하는 로직 (남은 시간 기준)
    private int CalculateGoldReward(float clearTime)
    {
        // 남은 시간 계산: 최대 허용 시간 - 실제 클리어 시간
        float timeRemaining = _maxAllowedGameTime - clearTime;
        Debug.Log($"[GameManager] 남은 시간: {timeRemaining:F2} 초");

        int awardedGold = 0;

        // 남은 시간이 음수 (최대 허용 시간 초과)인 경우 골드 없음
        if (timeRemaining < 0)
        {
            awardedGold = 0;
            Debug.Log("[GameManager] 최대 허용 시간을 초과하여 클리어했습니다. 골드 0개 지급.");
        }
        else
        {
            // 2초 단위로 골드 증가 (0-2초 남았으면 1골드, 2-4초 남았으면 2골드 등)
            // Mathf.FloorToInt는 소수점 이하를 버립니다.
            // 예를 들어 timeRemaining이 1.5초면 (1.5 / 2) = 0.75, FloorToInt(0.75) = 0.
            // 여기에 +1을 하여 1골드가 됩니다.
            // timeRemaining이 3.5초면 (3.5 / 2) = 1.75, FloorToInt(1.75) = 1.
            // 여기에 +1을 하여 2골드가 됩니다.
            awardedGold = Mathf.FloorToInt(timeRemaining / 2f) + 1;
            Debug.Log($"[GameManager] 남은 시간에 따라 계산된 골드: {awardedGold}개.");
        }

        return awardedGold;
    }

    // (선택 사항) 게임 클리어를 시뮬레이션하기 위한 코루틴
    // 실제 게임에서는 클리어 조건이 충족될 때 ClearGameAndAwardGold()를 호출합니다.
    private IEnumerator SimulateGameClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearGameAndAwardGold();
    }
}
