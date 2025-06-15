using UnityEngine;
using System.Collections; // �ڷ�ƾ ���
using TMPro; // TextMeshProUGUI ���
using UnityEngine.UI;

public class LogPanelController : MonoBehaviour
{
    // �� Inspector���� �Ҵ� ��
    [Header("UI References")]
    public RectTransform logPanelRectTransform; // �α� â Panel�� RectTransform (���� ������)
    public TMP_Text logText;                    // �α� �޽����� ǥ���� TextMeshProUGUI ������Ʈ (GlobalDebugLogger�� ������ ���� �Ҵ�)

    [Header("Panel Settings")]
    public float collapsedHeight = 50f;         // ���� ���� ���� ����
    public float expandedHeight = 300f;         // ������ ���� ����
    public float animationDuration = 0.2f;      // Ȯ��/��� �ִϸ��̼� �ð�

    private bool isExpanded = false; // ���� Ȯ�� ����
    private GlobalDebugLogger globalDebugLogger; // GlobalDebugLogger ����

    void Awake()
    {
        // GlobalDebugLogger �ν��Ͻ��� ã���ϴ�.
        globalDebugLogger = FindObjectOfType<GlobalDebugLogger>();
        if (globalDebugLogger == null)
        {
            Debug.LogError("GlobalDebugLogger�� ã�� �� �����ϴ�. LogPanelController�� ����� �۵����� �ʽ��ϴ�.");
            enabled = false;
            return;
        }

        // �ʱ� ���� ����
        if (logPanelRectTransform != null)
        {
            logPanelRectTransform.sizeDelta = new Vector2(logPanelRectTransform.sizeDelta.x, collapsedHeight);
        }

        isExpanded = false;

        // ó������ ���� �����̹Ƿ�, ������ �α׸� ���̵��� ������Ʈ�մϴ�.
        UpdateLogDisplay();
    }

    // �� �α� â�� ����ϴ� �޼��� (UI Button�� On Click() �̺�Ʈ�� ����) ��
    public void ToggleLogPanel()
    {
        isExpanded = !isExpanded;
        StopAllCoroutines(); // ���� �ִϸ��̼� �ߴ�
        StartCoroutine(AnimateLogPanelHeight(isExpanded ? expandedHeight : collapsedHeight));
        UpdateLogDisplay(); // ���� ���� �� �α� ������Ʈ
    }

    // �� �α� �ؽ�Ʈ�� ������Ʈ�ϴ� ���� �޼��� ��
    // GlobalDebugLogger�� fullLog�� ����Ͽ� ������ �α׸� ǥ���ϰų� ��ü �α׸� ǥ���մϴ�.
    private void UpdateLogDisplay()
    {
        if (logText == null || globalDebugLogger == null) return;

        if (isExpanded)
        {
            // Ȯ�� ����: GlobalDebugLogger�� ��ü �α׸� �״�� ǥ��
            logText.text = globalDebugLogger.fullLog;
            // ��ũ���� �� �Ʒ��� ������ �̵��Ͽ� �ֽ� �αװ� ���̰� �մϴ�.
            // ScrollRect ������Ʈ�� logPanelRectTransform�� �پ��ֽ��ϴ�.
            ScrollRect scrollRect = logPanelRectTransform.GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
                // ��� ��ũ�� �̵�
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }
        else
        {
            // ���� ����: GlobalDebugLogger�� ��ü �α׿��� ������ �ٸ� �����Ͽ� ǥ��
            string[] lines = globalDebugLogger.fullLog.Split('\n');
            if (lines.Length > 1) // ������ ���� �����Ѵٸ� (split�� �� ���ڿ� ����)
            {
                // ������ ���� ����ִ� ��찡 �����Ƿ�, ��ȿ�� ������ ���� ã���ϴ�.
                string lastValidLine = "";
                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                    {
                        lastValidLine = lines[i];
                        break;
                    }
                }
                logText.text = lastValidLine;
            }
            else // �αװ� �� ���̰ų� ���� ���
            {
                logText.text = globalDebugLogger.fullLog.Trim(); // Trim()���� �� �� ����
            }
        }
    }

    // �α� â ���� �ִϸ��̼� �ڷ�ƾ (������ ����)
    private IEnumerator AnimateLogPanelHeight(float targetHeight)
    {
        float startHeight = logPanelRectTransform.sizeDelta.y;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentHeight = Mathf.Lerp(startHeight, targetHeight, elapsedTime / animationDuration);
            logPanelRectTransform.sizeDelta = new Vector2(logPanelRectTransform.sizeDelta.x, currentHeight);
            yield return null;
        }
        logPanelRectTransform.sizeDelta = new Vector2(logPanelRectTransform.sizeDelta.x, targetHeight);
    }

    // �� �߿�: GlobalDebugLogger�� fullLog�� ������Ʈ�� ������ �� �α� �гε� ������Ʈ�ǵ��� ó�� ��
    // GlobalDebugLogger�� HandleLog �޼��忡�� �� LogPanelController�� UpdateLogDisplay()�� ȣ���ϵ��� �����ϰų�,
    // GlobalDebugLogger�� �α� ������Ʈ �̺�Ʈ�� �߰��ϴ� ���� �� ����մϴ�.
    // ������ ���� ������ ����� Update()���� ��� Ȯ���ϴ� ���Դϴ�.
    // ���⼭�� GlobalDebugLogger�� fullLog�� ���� �����Ͽ� ����ϹǷ�, Update()���� Ȯ���ϴ� ���� ���ʿ��� �� �ֽ��ϴ�.
    // ������ Update()���� UpdateLogDisplay()�� ȣ���ϸ� ���� ���ϰ� �� �� �����Ƿ�,
    // GlobalDebugLogger���� �αװ� �߰��� �� LogPanelController�� ��������� ������Ʈ�ϵ��� �ϴ� ���� �����ϴ�.
    // �Ʒ� OnGUI�� Debug.Log�� �׽�Ʈ�ϱ� ���� ��� ����� �� �ֽ��ϴ�.

    // �� �κ��� �׽�Ʈ �뵵�θ� ����ϼ���. ���� ���ӿ����� Debug.Log�� ����մϴ�.
    /*
    private void OnGUI() {
        if (GUI.Button(new Rect(10, 10, 150, 30), "Add Test Log")) {
            Debug.Log("This is a test log message from OnGUI.");
        }
    }
    */
}