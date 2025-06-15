using UnityEngine;
using System.Collections; // 코루틴 사용
using TMPro; // TextMeshProUGUI 사용
using UnityEngine.UI;

public class LogPanelController : MonoBehaviour
{
    // ★ Inspector에서 할당 ★
    [Header("UI References")]
    public RectTransform logPanelRectTransform; // 로그 창 Panel의 RectTransform (높이 조절용)
    public TMP_Text logText;                    // 로그 메시지를 표시할 TextMeshProUGUI 컴포넌트 (GlobalDebugLogger와 동일한 것을 할당)

    [Header("Panel Settings")]
    public float collapsedHeight = 50f;         // 접혀 있을 때의 높이
    public float expandedHeight = 300f;         // 펼쳐질 때의 높이
    public float animationDuration = 0.2f;      // 확장/축소 애니메이션 시간

    private bool isExpanded = false; // 현재 확장 상태
    private GlobalDebugLogger globalDebugLogger; // GlobalDebugLogger 참조

    void Awake()
    {
        // GlobalDebugLogger 인스턴스를 찾습니다.
        globalDebugLogger = FindObjectOfType<GlobalDebugLogger>();
        if (globalDebugLogger == null)
        {
            Debug.LogError("GlobalDebugLogger를 찾을 수 없습니다. LogPanelController가 제대로 작동하지 않습니다.");
            enabled = false;
            return;
        }

        // 초기 상태 설정
        if (logPanelRectTransform != null)
        {
            logPanelRectTransform.sizeDelta = new Vector2(logPanelRectTransform.sizeDelta.x, collapsedHeight);
        }

        isExpanded = false;

        // 처음에는 접힌 상태이므로, 마지막 로그만 보이도록 업데이트합니다.
        UpdateLogDisplay();
    }

    // ★ 로그 창을 토글하는 메서드 (UI Button의 On Click() 이벤트에 연결) ★
    public void ToggleLogPanel()
    {
        isExpanded = !isExpanded;
        StopAllCoroutines(); // 기존 애니메이션 중단
        StartCoroutine(AnimateLogPanelHeight(isExpanded ? expandedHeight : collapsedHeight));
        UpdateLogDisplay(); // 상태 변경 후 로그 업데이트
    }

    // ★ 로그 텍스트를 업데이트하는 내부 메서드 ★
    // GlobalDebugLogger의 fullLog를 사용하여 마지막 로그만 표시하거나 전체 로그를 표시합니다.
    private void UpdateLogDisplay()
    {
        if (logText == null || globalDebugLogger == null) return;

        if (isExpanded)
        {
            // 확장 상태: GlobalDebugLogger의 전체 로그를 그대로 표시
            logText.text = globalDebugLogger.fullLog;
            // 스크롤을 맨 아래로 강제로 이동하여 최신 로그가 보이게 합니다.
            // ScrollRect 컴포넌트는 logPanelRectTransform에 붙어있습니다.
            ScrollRect scrollRect = logPanelRectTransform.GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
                // 즉시 스크롤 이동
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }
        else
        {
            // 접힌 상태: GlobalDebugLogger의 전체 로그에서 마지막 줄만 추출하여 표시
            string[] lines = globalDebugLogger.fullLog.Split('\n');
            if (lines.Length > 1) // 마지막 줄이 존재한다면 (split은 빈 문자열 포함)
            {
                // 마지막 줄이 비어있는 경우가 있으므로, 유효한 마지막 줄을 찾습니다.
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
            else // 로그가 한 줄이거나 없는 경우
            {
                logText.text = globalDebugLogger.fullLog.Trim(); // Trim()으로 빈 줄 제거
            }
        }
    }

    // 로그 창 높이 애니메이션 코루틴 (이전과 동일)
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

    // ★ 중요: GlobalDebugLogger의 fullLog가 업데이트될 때마다 이 로그 패널도 업데이트되도록 처리 ★
    // GlobalDebugLogger의 HandleLog 메서드에서 이 LogPanelController의 UpdateLogDisplay()를 호출하도록 수정하거나,
    // GlobalDebugLogger에 로그 업데이트 이벤트를 추가하는 것이 더 깔끔합니다.
    // 하지만 가장 간단한 방법은 Update()에서 계속 확인하는 것입니다.
    // 여기서는 GlobalDebugLogger의 fullLog를 직접 접근하여 사용하므로, Update()에서 확인하는 것이 불필요할 수 있습니다.
    // 오히려 Update()에서 UpdateLogDisplay()를 호출하면 성능 저하가 올 수 있으므로,
    // GlobalDebugLogger에서 로그가 추가될 때 LogPanelController를 명시적으로 업데이트하도록 하는 것이 좋습니다.
    // 아래 OnGUI는 Debug.Log를 테스트하기 위해 잠시 사용할 수 있습니다.

    // 이 부분은 테스트 용도로만 사용하세요. 실제 게임에서는 Debug.Log를 사용합니다.
    /*
    private void OnGUI() {
        if (GUI.Button(new Rect(10, 10, 150, 30), "Add Test Log")) {
            Debug.Log("This is a test log message from OnGUI.");
        }
    }
    */
}