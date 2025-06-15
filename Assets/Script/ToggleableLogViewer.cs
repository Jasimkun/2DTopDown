using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ToggleableLogViewer : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text debugText;

    private string fullLog = "";
    private string lastLine = "";
    private bool showFull = false;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"{logString}";
        fullLog += logEntry + "\n";
        lastLine = logEntry;

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (debugText != null)
        {
            debugText.text = showFull ? fullLog : lastLine;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        showFull = !showFull;
        UpdateDisplay();
    }
}
