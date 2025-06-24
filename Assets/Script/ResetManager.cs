using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement; // SceneManager�� ����ϱ� ���� �ʿ��մϴ�.

public class ResetManager : MonoBehaviour
{
    // InventorySystem�� ShopManager �ν��Ͻ��� �̱����̹Ƿ� Awake���� ã���ϴ�.
    private InventorySystem inventorySystem;
    private ShopManager shopManager;

    void Awake()
    {
        // Awake ������ ���� null�� �� �����Ƿ�, ResetAllGameData()���� �ٽ� FindObjectOfType�� ȣ���ϴ� ���� �� �����մϴ�.
        // ���⼭�� ����� �뵵�θ� ����մϴ�.
        inventorySystem = InventorySystem.Instance;
        shopManager = ShopManager.Instance;

        if (inventorySystem == null) Debug.LogWarning("���� InventorySystem �ν��Ͻ��� �����ϴ�. �κ��丮 �ʱ�ȭ�� ���� �߻� ����.");
        if (shopManager == null) Debug.LogWarning("���� ShopManager �ν��Ͻ��� �����ϴ�. ���� ������ �ʱ�ȭ�� ���� �߻� ����.");
    }

    /// <summary>
    /// ��� ���� ������(PlayerPrefs, ���� ����, �κ��丮, ����)�� ������ �ʱ�ȭ�մϴ�.
    /// �� �޼��尡 ȣ��� �Ŀ��� ������ ������ϴ� ���� �����ϴ�.
    /// </summary>
    public void ResetAllGameData()
    {
        Debug.Log("��� ���� ������ �ʱ�ȭ�� �����մϴ�...");

        // 1. PlayerPrefs ������ ����
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[ResetManager] PlayerPrefs �����Ͱ� �����Ǿ����ϴ�.");

        // 2. Application.persistentDataPath ������ ��� ���� ���� (JSON ���̺� ���� ��)
        string persistentPath = Application.persistentDataPath;
        if (Directory.Exists(persistentPath))
        {
            DirectoryInfo directory = new DirectoryInfo(persistentPath);
            foreach (FileInfo file in directory.GetFiles())
            {
                // .json ���ϸ� �����ϰų�, .meta ���� �� �߿����� ���� ������ ������ �� �ֽ��ϴ�.
                // ���⼭�� ��� ������ �����մϴ�.
                file.Delete();
                Debug.Log($"[ResetManager] ���� ������: {file.Name}");
            }
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                // �� ���� ���丮�� �����ϰų�, Ư�� ���丮�� ������ �� �ֽ��ϴ�.
                // ���⼭�� ��� ���� ���丮�� ��������� �����մϴ�.
                subDirectory.Delete(true);
                Debug.Log($"[ResetManager] ���� ������: {subDirectory.Name}");
            }
            Debug.Log($"[ResetManager] '{persistentPath}' ����� ��� ���� ���� ������ �����Ǿ����ϴ�.");
        }
        else
        {
            Debug.Log($"[ResetManager] '{persistentPath}' ��ΰ� �������� �ʽ��ϴ�. ������ ���� ����.");
        }

        // 3. InventorySystem ������ �ʱ�ȭ
        if (inventorySystem == null)
        {
            inventorySystem = InventorySystem.Instance;
        }
        if (inventorySystem == null)
        {
            // FindObjectOfType(true)�� ����Ͽ� ��Ȱ��ȭ�� ������Ʈ�� ���� ������Ʈ�� ã���� �մϴ�.
            inventorySystem = FindObjectOfType<InventorySystem>(true);
        }

        if (inventorySystem != null)
        {
            inventorySystem.ResetInventoryData();
            Debug.Log("[ResetManager] �κ��丮 �ý��� �ʱ�ȭ ��û �Ϸ�.");
        }
        else
        {
            Debug.LogWarning("[ResetManager] InventorySystem�� ã�� �� ���� �κ��丮�� �ʱ�ȭ�� �� �����ϴ�.");
        }

        // 4. ShopManager ������ �ʱ�ȭ
        if (shopManager == null)
        {
            shopManager = ShopManager.Instance;
        }
        if (shopManager == null)
        {
            // FindObjectOfType(true)�� ����Ͽ� ��Ȱ��ȭ�� ������Ʈ�� ���� ������Ʈ�� ã���� �մϴ�.
            shopManager = FindObjectOfType<ShopManager>(true);
        }

        if (shopManager != null)
        {
            shopManager.ResetShopData();
            Debug.Log("[ResetManager] ���� �ý��� �ʱ�ȭ ��û �Ϸ�.");
        }
        else
        {
            Debug.LogWarning("[ResetManager] ShopManager�� ã�� �� ���� ���� �����͸� �ʱ�ȭ�� �� �����ϴ�.");
        }

        Debug.Log("��� ���� ������ �ʱ�ȭ�� �Ϸ�Ǿ����ϴ�! ������ ������ؾ� ��������� ������ �ݿ��˴ϴ�.");

        // ��� �����Ͱ� �ʱ�ȭ�� ��, ������ ���� �޴��� �����ų� ���� ���� ��ε��Ͽ� ������ �ʱ�ȭ�� ���·� ����ϴ�.
        // ���� ���� �ٽ� �ε��ϴ� ���� ���� �����մϴ�.
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        Debug.Log($"[ResetManager] ���� �� '{currentSceneName}'��(��) ��ε��մϴ�.");
    }
}
