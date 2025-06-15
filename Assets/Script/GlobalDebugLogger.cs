using UnityEngine;
using TMPro;

public class GlobalDebugLogger : MonoBehaviour
{
    public TMP_Text debugText;
    public string fullLog = "";

    private string cachedPlayerName = "�� �� ���� �÷��̾�"; // �⺻�� ���� (�̸��� ã�� ���� ���)

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
        // PlayerPrefs���� "Playername" Ű�� ����� �̸��� �����ɴϴ�.
        // ���� �̸��� ����Ǿ� ���� �ʴٸ�, �⺻���� "�� �� ���� �÷��̾�"�� ���˴ϴ�.
        cachedPlayerName = PlayerPrefs.GetString("Playername", "�� �� ���� �÷��̾�");

        Debug.Log($"GlobalDebugLogger: �÷��̾� �̸� �ʱ�ȭ��: {cachedPlayerName}");
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry;

        logEntry = $"[{cachedPlayerName}] : {logString}";

        // �α� ����
        fullLog += logEntry + "\n";

        // TMP�� ���
        if (debugText != null)
        {
            debugText.text = fullLog;
        }
    }
}
