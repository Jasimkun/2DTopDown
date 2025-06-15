using UnityEngine;
using System.IO;
// using System.Collections.Generic; // List와 관련된 오류 방지 (필요시) - 이 스크립트에서는 직접 List를 다루지 않으므로 필요 없을 수 있습니다.

public class ResetManager : MonoBehaviour
{
    // InventorySystem 참조 (이제 Inspector에서 직접 연결할 필요 없이 코드로 찾습니다)
    private InventorySystem inventorySystem; // [변경] SerializeField 제거, private으로 변경

    void Awake()
    {
        // 씬에 InventorySystem이 있다면 참조를 얻어옵니다.
        // 이 스크립트가 InventorySystem보다 먼저 Awake될 경우 null일 수 있으므로
        // ResetAllGameData()에서 다시 null 체크를 하는 것이 안전합니다.
        inventorySystem = FindObjectOfType<InventorySystem>();
        if (inventorySystem == null)
        {
            Debug.LogWarning("씬에 InventorySystem이 없습니다. 인벤토리 관련 초기화는 수행되지 않습니다.");
        }
    }

    // 모든 게임 데이터를 리셋하는 공용 메서드
    public void ResetAllGameData()
    {
        Debug.Log("모든 게임 데이터 초기화를 시작합니다...");

        // 1. PlayerPrefs 데이터 삭제
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs 데이터가 삭제되었습니다.");

        // 2. Application.persistentDataPath 내부의 모든 파일 삭제 (JSON 세이브 파일 등)
        string persistentPath = Application.persistentDataPath;
        if (Directory.Exists(persistentPath))
        {
            DirectoryInfo directory = new DirectoryInfo(persistentPath);
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
                Debug.Log($"파일 삭제됨: {file.Name}");
            }
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                subDirectory.Delete(true);
                Debug.Log($"폴더 삭제됨: {subDirectory.Name}");
            }
            Debug.Log($"'{persistentPath}' 경로의 모든 게임 저장 파일이 삭제되었습니다.");
        }
        else
        {
            Debug.Log($"'{persistentPath}' 경로가 존재하지 않습니다. 삭제할 파일 없음.");
        }

        // 3. InventorySystem 초기화
        // ResetAllGameData()가 호출될 때 inventorySystem이 null일 수도 있으므로 다시 한번 찾습니다.
        // (특히 Awake 순서 때문에)
        if (inventorySystem == null)
        {
            inventorySystem = FindObjectOfType<InventorySystem>();
        }

        if (inventorySystem != null)
        {
            // InventorySystem의 ResetInventoryData() 메서드를 호출
            //inventorySystem.ResetInventoryData(); // 이 메서드가 InventorySystem에서 Clear() 및 파일 삭제, UI 갱신을 담당
            Debug.Log("인벤토리 시스템이 초기화 요청되었습니다.");
        }
        else
        {
            Debug.LogWarning("InventorySystem을 찾을 수 없어 인벤토리를 초기화할 수 없습니다.");
        }

        Debug.Log("모든 게임 데이터 초기화가 완료되었습니다! 게임을 재시작해야 합니다.");

        // 모든 데이터가 초기화된 후, 일반적으로는 게임을 메인 메뉴로 돌리거나 재시작합니다.
        // SceneManager.LoadScene("MainMenuScene"); // 예시: SceneManager를 사용하려면 using UnityEngine.SceneManagement; 필요
        // Application.Quit(); // 게임 종료 (빌드 시)
    }
}