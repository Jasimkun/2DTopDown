using UnityEngine;
using System.Collections; // Coroutine�� ����ϱ� ���� �߰�

// �� ��ũ��Ʈ�� ������ �������� ����(����, ����)�� �����ϰ�,
// ���� Ŭ���� �� ���� �ð��� ���� ��带 �����ϴ� ������ ó���մϴ�.
// �̱��� ������ ����Ͽ� ���� �� ��𿡼��� ���� ������ �� �ֽ��ϴ�.
public class GameManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GameManager Instance { get; private set; }

    // ���� ���� �ð� (��)
    private float _gameStartTime;
    // ������ ���� ���� ������ ����
    private bool _isGameRunning = false;

    [Header("��� ���� ���� (���� �ð� ����)")]
    [Tooltip("�� �ð� ���� ������ Ŭ�����ؾ� ��带 ���� �� �ֽ��ϴ�. ������ �ִ� ��� �ð��Դϴ�.")]
    [SerializeField] private float _maxAllowedGameTime = 60f; // ����: ���� Ŭ������� �ִ� 60�� ���

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� ����Ǿ �ı����� �ʵ��� ����
            Debug.Log("[GameManager] GameManager �ʱ�ȭ �Ϸ�.");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // ���� ���� �� �ٷ� ������ ������ ���� �ְ�,
        // Ư�� �̺�Ʈ(��: ���� ��ư Ŭ��)�� ���� ������ ���� �ֽ��ϴ�.
        // ���÷� Start���� �ٷ� �����ϵ��� �մϴ�.
        StartGame();
    }

    // ���� ���� �޼���
    public void StartGame()
    {
        if (_isGameRunning)
        {
            Debug.LogWarning("[GameManager] ������ �̹� ���� ���Դϴ�!");
            return;
        }

        _gameStartTime = Time.time; // ���� ���� �ð��� ���
        _isGameRunning = true;
        Debug.Log("[GameManager] ���� ����! ���� �ð�: " + _gameStartTime);

        // Debug.Log("[GameManager] �׽�Ʈ: 5�� �� ���� Ŭ���� �ùķ��̼� ����...");
        // StartCoroutine(SimulateGameClearAfterDelay(5f)); // 5�� �� Ŭ���� �ùķ��̼�
        // Debug.Log("[GameManager] �׽�Ʈ: 65�� �� ���� Ŭ���� �ùķ��̼� ���� (�ð� �ʰ�)...");
        // StartCoroutine(SimulateGameClearAfterDelay(65f)); // 65�� �� Ŭ���� �ùķ��̼�
    }

    // ������ Ŭ�����ϰ� ��带 �����ϴ� �޼��� (���� Ŭ���� ������ ������ �� ȣ��)
    public void ClearGameAndAwardGold()
    {
        if (!_isGameRunning)
        {
            Debug.LogWarning("[GameManager] ������ ���۵��� �ʾҴµ� Ŭ�����Ϸ� �մϴ�!");
            return;
        }

        _isGameRunning = false; // ���� ���� ���·� ����
        float clearTime = Time.time - _gameStartTime; // Ŭ���� �ð� ���

        Debug.Log($"[GameManager] ���� Ŭ����! �ҿ� �ð�: {clearTime:F2} ��");

        int awardedGold = CalculateGoldReward(clearTime);

        // CurrencyManager�� �ʱ�ȭ�Ǿ����� Ȯ�� �� ��� ����
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddGold(awardedGold);
            Debug.Log($"[GameManager] Ŭ���� �������� ��� {awardedGold}�� ����!");
        }
        else
        {
            Debug.LogError("[GameManager] CurrencyManager �ν��Ͻ��� ã�� �� �����ϴ�. ��� ���� ����!");
        }

        // ���� Ŭ���� �� �߰����� ���� (��: ��� ȭ�� ǥ��, ���� ���� �ε� ��)
        // ���⿡ �߰� ����
    }

    // Ŭ���� �ð��� ���� ��� ������ ����ϴ� ���� (���� �ð� ����)
    private int CalculateGoldReward(float clearTime)
    {
        // ���� �ð� ���: �ִ� ��� �ð� - ���� Ŭ���� �ð�
        float timeRemaining = _maxAllowedGameTime - clearTime;
        Debug.Log($"[GameManager] ���� �ð�: {timeRemaining:F2} ��");

        int awardedGold = 0;

        // ���� �ð��� ���� (�ִ� ��� �ð� �ʰ�)�� ��� ��� ����
        if (timeRemaining < 0)
        {
            awardedGold = 0;
            Debug.Log("[GameManager] �ִ� ��� �ð��� �ʰ��Ͽ� Ŭ�����߽��ϴ�. ��� 0�� ����.");
        }
        else
        {
            // 2�� ������ ��� ���� (0-2�� �������� 1���, 2-4�� �������� 2��� ��)
            // Mathf.FloorToInt�� �Ҽ��� ���ϸ� �����ϴ�.
            // ���� ��� timeRemaining�� 1.5�ʸ� (1.5 / 2) = 0.75, FloorToInt(0.75) = 0.
            // ���⿡ +1�� �Ͽ� 1��尡 �˴ϴ�.
            // timeRemaining�� 3.5�ʸ� (3.5 / 2) = 1.75, FloorToInt(1.75) = 1.
            // ���⿡ +1�� �Ͽ� 2��尡 �˴ϴ�.
            awardedGold = Mathf.FloorToInt(timeRemaining / 2f) + 1;
            Debug.Log($"[GameManager] ���� �ð��� ���� ���� ���: {awardedGold}��.");
        }

        return awardedGold;
    }

    // (���� ����) ���� Ŭ��� �ùķ��̼��ϱ� ���� �ڷ�ƾ
    // ���� ���ӿ����� Ŭ���� ������ ������ �� ClearGameAndAwardGold()�� ȣ���մϴ�.
    private IEnumerator SimulateGameClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearGameAndAwardGold();
    }
}
