using UnityEngine;
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� �ʿ��մϴ�.

// DestinationTrigger ��ũ��Ʈ�� ������ ������ ��ġ�˴ϴ�.
// �� ��ũ��Ʈ�� �÷��̾ �� ������ �����ߴ��� �����ϰ�,
// ���� Ŭ���� ������ ������ �� ���� ������ ��ȯ�ϴ� ������ �մϴ�.
public class DestinationTrigger : MonoBehaviour
{
    [Tooltip("�÷��̾� GameObject�� �±��Դϴ�. 'Player' �±׷� �����Ǿ� �ִ��� Ȯ���ϼ���.")]
    public string playerTag = "Player"; // �÷��̾ �ĺ��ϱ� ���� �±�

    [Tooltip("�÷��̾ �������� �������� �� �ε��� ���� ���� �̸��Դϴ�.")]
    public string nextSceneName = "Test2"; // ��ȯ�� ���� ���� �̸�

    // 2D Collider�� ���� ������Ʈ�� �� Ʈ���� �ȿ� ������ �� ȣ��˴ϴ�.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ʈ���� �ȿ� ���� ������Ʈ�� 'playerTag' (�⺻��: "Player")�� ��ġ�ϴ��� Ȯ���մϴ�.
        if (other.CompareTag(playerTag))
        {
            Debug.Log($"[DestinationTrigger] �÷��̾� ({other.name})�� �������� �����߽��ϴ�! ���� Ŭ����.");

            // GameManager �ν��Ͻ��� �����ϸ� ���� Ŭ���� �� ��� ���� ������ ȣ���մϴ�.
            // GameManager�� ������ �����Ǿ� �־�� �մϴ�.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ClearGameAndAwardGold();
            }
            else
            {
                Debug.LogWarning("[DestinationTrigger] GameManager.Instance�� ã�� �� �����ϴ�. ��� ������ �ǳʶݴϴ�.");
            }

            // ������ ���� ������ ��ȯ�մϴ�.
            // SceneManager.LoadScene() �Լ��� Unity�� �� ��ȯ�� ����մϴ�.
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // ���� 3D �����̶�� OnTriggerEnter (2D ���)�� ����ؾ� �մϴ�.
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag(playerTag))
    //     {
    //         Debug.Log($"[DestinationTrigger] �÷��̾� ({other.name})�� �������� �����߽��ϴ�! ���� Ŭ����.");
    //         if (GameManager.Instance != null)
    //         {
    //             GameManager.Instance.ClearGameAndAwardGold();
    //         }
    //         else
    //         {
    //             Debug.LogWarning("[DestinationTrigger] GameManager.Instance�� ã�� �� �����ϴ�. ��� ������ �ǳʶݴϴ�.");
    //         }
    //         SceneManager.LoadScene(nextSceneName);
    //     }
    // }
}
