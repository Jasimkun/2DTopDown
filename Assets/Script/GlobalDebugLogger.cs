using UnityEngine;
using TMPro;

public class GlobalDebugLogger : MonoBehaviour
{
    public TMP_Text debugText;
    public string fullLog = "";

    private string cachedPlayerName = "알 수 없는 플레이어"; // 기본값 설정 (이름을 찾지 못할 경우)

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    void Awake()
    {
        // PlayerPrefs에서 "Playername" 키로 저장된 이름을 가져옵니다.
        // 만약 이름이 저장되어 있지 않다면, 기본값인 "알 수 없는 플레이어"가 사용됩니다.
        cachedPlayerName = PlayerPrefs.GetString("Playername", "알 수 없는 플레이어");

        Debug.Log($"GlobalDebugLogger: 플레이어 이름 초기화됨: {cachedPlayerName}");
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry;

        logEntry = $"[{cachedPlayerName}] : {logString}";

        // 로그 누적
        fullLog += logEntry + "\n";

        // TMP에 출력
        if (debugText != null)
        {
            debugText.text = fullLog;
        }
    }
}
