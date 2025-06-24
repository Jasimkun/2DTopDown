using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요합니다.

// DestinationTrigger 스크립트는 목적지 지점에 배치됩니다.
// 이 스크립트는 플레이어가 이 지점에 도달했는지 감지하고,
// 게임 클리어 로직을 실행한 후 다음 씬으로 전환하는 역할을 합니다.
public class DestinationTrigger : MonoBehaviour
{
    [Tooltip("플레이어 GameObject의 태그입니다. 'Player' 태그로 설정되어 있는지 확인하세요.")]
    public string playerTag = "Player"; // 플레이어를 식별하기 위한 태그

    [Tooltip("플레이어가 목적지에 도달했을 때 로드할 다음 씬의 이름입니다.")]
    public string nextSceneName = "Test2"; // 전환할 다음 씬의 이름

    // 2D Collider를 가진 오브젝트가 이 트리거 안에 들어왔을 때 호출됩니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 트리거 안에 들어온 오브젝트가 'playerTag' (기본값: "Player")와 일치하는지 확인합니다.
        if (other.CompareTag(playerTag))
        {
            Debug.Log($"[DestinationTrigger] 플레이어 ({other.name})가 목적지에 도착했습니다! 게임 클리어.");

            // GameManager 인스턴스가 존재하면 게임 클리어 및 골드 지급 로직을 호출합니다.
            // GameManager는 별도로 구현되어 있어야 합니다.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ClearGameAndAwardGold();
            }
            else
            {
                Debug.LogWarning("[DestinationTrigger] GameManager.Instance를 찾을 수 없습니다. 골드 지급을 건너뜁니다.");
            }

            // 지정된 다음 씬으로 전환합니다.
            // SceneManager.LoadScene() 함수는 Unity의 씬 전환을 담당합니다.
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // 만약 3D 게임이라면 OnTriggerEnter (2D 대신)를 사용해야 합니다.
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag(playerTag))
    //     {
    //         Debug.Log($"[DestinationTrigger] 플레이어 ({other.name})가 목적지에 도착했습니다! 게임 클리어.");
    //         if (GameManager.Instance != null)
    //         {
    //             GameManager.Instance.ClearGameAndAwardGold();
    //         }
    //         else
    //         {
    //             Debug.LogWarning("[DestinationTrigger] GameManager.Instance를 찾을 수 없습니다. 골드 지급을 건너뜁니다.");
    //         }
    //         SceneManager.LoadScene(nextSceneName);
    //     }
    // }
}
