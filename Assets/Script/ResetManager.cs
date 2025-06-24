using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement; // SceneManager를 사용하기 위해 필요합니다.

public class ResetManager : MonoBehaviour
{
    // InventorySystem과 ShopManager 인스턴스는 싱글톤이므로 Awake에서 찾습니다.
    private InventorySystem inventorySystem;
    private ShopManager shopManager;

    void Awake()
    {
        // Awake 순서에 따라 null일 수 있으므로, ResetAllGameData()에서 다시 FindObjectOfType을 호출하는 것이 더 안전합니다.
        // 여기서는 디버그 용도로만 사용합니다.
        inventorySystem = InventorySystem.Instance;
        shopManager = ShopManager.Instance;

        if (inventorySystem == null) Debug.LogWarning("씬에 InventorySystem 인스턴스가 없습니다. 인벤토리 초기화에 문제 발생 가능.");
        if (shopManager == null) Debug.LogWarning("씬에 ShopManager 인스턴스가 없습니다. 상점 데이터 초기화에 문제 발생 가능.");
    }

    /// <summary>
    /// 모든 게임 데이터(PlayerPrefs, 저장 파일, 인벤토리, 상점)를 완전히 초기화합니다.
    /// 이 메서드가 호출된 후에는 게임을 재시작하는 것이 좋습니다.
    /// </summary>
    public void ResetAllGameData()
    {
        Debug.Log("모든 게임 데이터 초기화를 시작합니다...");

        // 1. PlayerPrefs 데이터 삭제
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[ResetManager] PlayerPrefs 데이터가 삭제되었습니다.");

        // 2. Application.persistentDataPath 내부의 모든 파일 삭제 (JSON 세이브 파일 등)
        string persistentPath = Application.persistentDataPath;
        if (Directory.Exists(persistentPath))
        {
            DirectoryInfo directory = new DirectoryInfo(persistentPath);
            foreach (FileInfo file in directory.GetFiles())
            {
                // .json 파일만 삭제하거나, .meta 파일 등 중요하지 않은 파일은 제외할 수 있습니다.
                // 여기서는 모든 파일을 삭제합니다.
                file.Delete();
                Debug.Log($"[ResetManager] 파일 삭제됨: {file.Name}");
            }
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                // 빈 서브 디렉토리만 삭제하거나, 특정 디렉토리만 삭제할 수 있습니다.
                // 여기서는 모든 서브 디렉토리를 재귀적으로 삭제합니다.
                subDirectory.Delete(true);
                Debug.Log($"[ResetManager] 폴더 삭제됨: {subDirectory.Name}");
            }
            Debug.Log($"[ResetManager] '{persistentPath}' 경로의 모든 게임 저장 파일이 삭제되었습니다.");
        }
        else
        {
            Debug.Log($"[ResetManager] '{persistentPath}' 경로가 존재하지 않습니다. 삭제할 파일 없음.");
        }

        // 3. InventorySystem 데이터 초기화
        // ResetAllGameData()가 호출될 때 싱글톤 인스턴스가 아직 설정되지 않았을 수 있으므로 다시 Find
        if (inventorySystem == null)
        {
            inventorySystem = InventorySystem.Instance; // 씬 로드 후에는 Instance가 설정되어 있을 가능성이 높음
        }
        if (inventorySystem == null)
        {
            inventorySystem = FindObjectOfType<InventorySystem>(); // 그래도 없으면 직접 찾기 (비활성 오브젝트도 찾으려면 FindObjectOfType<InventorySystem>(true) 사용)
        }

        if (inventorySystem != null)
        {
            inventorySystem.ResetInventoryData(); // InventorySystem의 ResetInventoryData() 호출
            Debug.Log("[ResetManager] 인벤토리 시스템 초기화 요청 완료.");
        }
        else
        {
            Debug.LogWarning("[ResetManager] InventorySystem을 찾을 수 없어 인벤토리를 초기화할 수 없습니다.");
        }

        // 4. ShopManager 데이터 초기화
        // ResetAllGameData()가 호출될 때 싱글톤 인스턴스가 아직 설정되지 않았을 수 있으므로 다시 Find
        if (shopManager == null)
        {
            shopManager = ShopManager.Instance; // 씬 로드 후에는 Instance가 설정되어 있을 가능성이 높음
        }
        if (shopManager == null)
        {
            shopManager = FindObjectOfType<ShopManager>(); // 그래도 없으면 직접 찾기 (비활성 오브젝트도 찾으려면 FindObjectOfType<ShopManager>(true) 사용)
        }

        if (shopManager != null)
        {
            shopManager.ResetShopData(); // ShopManager의 ResetShopData() 호출
            Debug.Log("[ResetManager] 상점 시스템 초기화 요청 완료.");
        }
        else
        {
            Debug.LogWarning("[ResetManager] ShopManager를 찾을 수 없어 상점 데이터를 초기화할 수 없습니다.");
        }

        Debug.Log("모든 게임 데이터 초기화가 완료되었습니다! 게임을 재시작해야 변경사항이 완전히 반영됩니다.");

        // 모든 데이터가 초기화된 후, 게임을 메인 메뉴로 돌리거나 현재 씬을 재로드하여 완전히 초기화된 상태로 만듭니다.
        // 현재 씬을 다시 로드하는 것이 가장 간단합니다.
        // 주의: 현재 씬 이름이 무엇인지 확인하고 사용해야 합니다.
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        Debug.Log($"[ResetManager] 현재 씬 '{currentSceneName}'을(를) 재로드합니다.");
    }
}
