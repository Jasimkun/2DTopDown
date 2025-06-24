using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요합니다.
using UnityEngine.UI; // 버튼 컴포넌트를 사용하기 위해 필요합니다.

/// <summary>
/// 메인 화면으로 돌아가는 버튼 기능을 제공하는 스크립트입니다.
/// 이 스크립트를 UI 버튼에 연결하여 사용합니다.
/// </summary>
public class GoToMain : MonoBehaviour
{
    [Header("메인 씬 설정")]
    [Tooltip("메인 화면으로 사용할 씬의 이름입니다. Unity의 'File > Build Settings'에 추가된 정확한 씬 이름을 입력하세요.")]
    public string mainSceneName = "Main"; // 기본값으로 "MainScene" 설정. 실제 씬 이름으로 변경해주세요.

    /// <summary>
    /// 이 버튼의 Text 컴포넌트입니다. (선택 사항)
    /// </summary>
    [SerializeField]
    private Text buttonText;

    void Awake()
    {
        // Text 컴포넌트가 할당되지 않았다면, 자동으로 찾습니다.
        //if (buttonText == null)
        //{
            //buttonText = GetComponentInChildren<TextMeshProUGUI>();
            //if (buttonText == null)
            //{
                // TextMeshProUGUI가 있다면 TextMeshPro 네임스페이스를 사용하여 찾아야 합니다.
                // 이 예시에서는 기본 Unity UI.Text를 가정합니다.
                //Debug.LogWarning("[MainMenuButton] 버튼에서 Text 컴포넌트를 찾을 수 없습니다. TextMeshProUGUI를 사용한다면 스크립트를 수정해야 합니다.");
            //}
        //}

        // 버튼 컴포넌트를 찾아 OnClick 리스너를 추가합니다.
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners(); // 기존 리스너 제거 (중복 방지)
            button.onClick.AddListener(GoToMainMenu);
            //Debug.Log($"[MainMenuButton] '{gameObject.name}' 버튼에 GoToMainMenu 리스너가 추가되었습니다.");
        }
        else
        {
            //Debug.LogError($"[MainMenuButton] '{gameObject.name}' GameObject에 Button 컴포넌트가 없습니다. 버튼 기능을 사용할 수 없습니다.");
        }
    }

    /// <summary>
    /// 이 메서드는 버튼 클릭 시 호출됩니다.
    /// 지정된 메인 씬으로 게임 씬을 전환합니다.
    /// </summary>
    public void GoToMainMenu()
    {
        // 씬 로드를 시도하기 전에 씬 이름이 유효한지 간단히 확인합니다.
        if (string.IsNullOrEmpty(mainSceneName))
        {
            //Debug.LogError("[MainMenuButton] 메인 씬 이름이 설정되지 않았습니다. Inspector에서 'Main Scene Name'을 입력하세요.");
            return;
        }

        //Debug.Log($"[MainMenuButton] 메인 화면으로 이동합니다: '{mainSceneName}' 씬 로드 중...");

        // 씬을 로드합니다. (단일 씬 모드로, 현재 씬을 언로드하고 새 씬을 로드)
        // 비동기 로드를 원한다면 SceneManager.LoadSceneAsync를 사용합니다.
        SceneManager.LoadScene(mainSceneName);
    }
}
