using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �ʿ��մϴ�.
using UnityEngine.UI; // ��ư ������Ʈ�� ����ϱ� ���� �ʿ��մϴ�.

/// <summary>
/// ���� ȭ������ ���ư��� ��ư ����� �����ϴ� ��ũ��Ʈ�Դϴ�.
/// �� ��ũ��Ʈ�� UI ��ư�� �����Ͽ� ����մϴ�.
/// </summary>
public class GoToMain : MonoBehaviour
{
    [Header("���� �� ����")]
    [Tooltip("���� ȭ������ ����� ���� �̸��Դϴ�. Unity�� 'File > Build Settings'�� �߰��� ��Ȯ�� �� �̸��� �Է��ϼ���.")]
    public string mainSceneName = "Main"; // �⺻������ "MainScene" ����. ���� �� �̸����� �������ּ���.

    /// <summary>
    /// �� ��ư�� Text ������Ʈ�Դϴ�. (���� ����)
    /// </summary>
    [SerializeField]
    private Text buttonText;

    void Awake()
    {
        // Text ������Ʈ�� �Ҵ���� �ʾҴٸ�, �ڵ����� ã���ϴ�.
        //if (buttonText == null)
        //{
            //buttonText = GetComponentInChildren<TextMeshProUGUI>();
            //if (buttonText == null)
            //{
                // TextMeshProUGUI�� �ִٸ� TextMeshPro ���ӽ����̽��� ����Ͽ� ã�ƾ� �մϴ�.
                // �� ���ÿ����� �⺻ Unity UI.Text�� �����մϴ�.
                //Debug.LogWarning("[MainMenuButton] ��ư���� Text ������Ʈ�� ã�� �� �����ϴ�. TextMeshProUGUI�� ����Ѵٸ� ��ũ��Ʈ�� �����ؾ� �մϴ�.");
            //}
        //}

        // ��ư ������Ʈ�� ã�� OnClick �����ʸ� �߰��մϴ�.
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners(); // ���� ������ ���� (�ߺ� ����)
            button.onClick.AddListener(GoToMainMenu);
            //Debug.Log($"[MainMenuButton] '{gameObject.name}' ��ư�� GoToMainMenu �����ʰ� �߰��Ǿ����ϴ�.");
        }
        else
        {
            //Debug.LogError($"[MainMenuButton] '{gameObject.name}' GameObject�� Button ������Ʈ�� �����ϴ�. ��ư ����� ����� �� �����ϴ�.");
        }
    }

    /// <summary>
    /// �� �޼���� ��ư Ŭ�� �� ȣ��˴ϴ�.
    /// ������ ���� ������ ���� ���� ��ȯ�մϴ�.
    /// </summary>
    public void GoToMainMenu()
    {
        // �� �ε带 �õ��ϱ� ���� �� �̸��� ��ȿ���� ������ Ȯ���մϴ�.
        if (string.IsNullOrEmpty(mainSceneName))
        {
            //Debug.LogError("[MainMenuButton] ���� �� �̸��� �������� �ʾҽ��ϴ�. Inspector���� 'Main Scene Name'�� �Է��ϼ���.");
            return;
        }

        //Debug.Log($"[MainMenuButton] ���� ȭ������ �̵��մϴ�: '{mainSceneName}' �� �ε� ��...");

        // ���� �ε��մϴ�. (���� �� ����, ���� ���� ��ε��ϰ� �� ���� �ε�)
        // �񵿱� �ε带 ���Ѵٸ� SceneManager.LoadSceneAsync�� ����մϴ�.
        SceneManager.LoadScene(mainSceneName);
    }
}
