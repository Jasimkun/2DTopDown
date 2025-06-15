using UnityEngine;
using System.IO;
// using System.Collections.Generic; // List�� ���õ� ���� ���� (�ʿ��) - �� ��ũ��Ʈ������ ���� List�� �ٷ��� �����Ƿ� �ʿ� ���� �� �ֽ��ϴ�.

public class ResetManager : MonoBehaviour
{
    // InventorySystem ���� (���� Inspector���� ���� ������ �ʿ� ���� �ڵ�� ã���ϴ�)
    private InventorySystem inventorySystem; // [����] SerializeField ����, private���� ����

    void Awake()
    {
        // ���� InventorySystem�� �ִٸ� ������ ���ɴϴ�.
        // �� ��ũ��Ʈ�� InventorySystem���� ���� Awake�� ��� null�� �� �����Ƿ�
        // ResetAllGameData()���� �ٽ� null üũ�� �ϴ� ���� �����մϴ�.
        inventorySystem = FindObjectOfType<InventorySystem>();
        if (inventorySystem == null)
        {
            Debug.LogWarning("���� InventorySystem�� �����ϴ�. �κ��丮 ���� �ʱ�ȭ�� ������� �ʽ��ϴ�.");
        }
    }

    // ��� ���� �����͸� �����ϴ� ���� �޼���
    public void ResetAllGameData()
    {
        Debug.Log("��� ���� ������ �ʱ�ȭ�� �����մϴ�...");

        // 1. PlayerPrefs ������ ����
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs �����Ͱ� �����Ǿ����ϴ�.");

        // 2. Application.persistentDataPath ������ ��� ���� ���� (JSON ���̺� ���� ��)
        string persistentPath = Application.persistentDataPath;
        if (Directory.Exists(persistentPath))
        {
            DirectoryInfo directory = new DirectoryInfo(persistentPath);
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
                Debug.Log($"���� ������: {file.Name}");
            }
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                subDirectory.Delete(true);
                Debug.Log($"���� ������: {subDirectory.Name}");
            }
            Debug.Log($"'{persistentPath}' ����� ��� ���� ���� ������ �����Ǿ����ϴ�.");
        }
        else
        {
            Debug.Log($"'{persistentPath}' ��ΰ� �������� �ʽ��ϴ�. ������ ���� ����.");
        }

        // 3. InventorySystem �ʱ�ȭ
        // ResetAllGameData()�� ȣ��� �� inventorySystem�� null�� ���� �����Ƿ� �ٽ� �ѹ� ã���ϴ�.
        // (Ư�� Awake ���� ������)
        if (inventorySystem == null)
        {
            inventorySystem = FindObjectOfType<InventorySystem>();
        }

        if (inventorySystem != null)
        {
            // InventorySystem�� ResetInventoryData() �޼��带 ȣ��
            //inventorySystem.ResetInventoryData(); // �� �޼��尡 InventorySystem���� Clear() �� ���� ����, UI ������ ���
            Debug.Log("�κ��丮 �ý����� �ʱ�ȭ ��û�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogWarning("InventorySystem�� ã�� �� ���� �κ��丮�� �ʱ�ȭ�� �� �����ϴ�.");
        }

        Debug.Log("��� ���� ������ �ʱ�ȭ�� �Ϸ�Ǿ����ϴ�! ������ ������ؾ� �մϴ�.");

        // ��� �����Ͱ� �ʱ�ȭ�� ��, �Ϲ������δ� ������ ���� �޴��� �����ų� ������մϴ�.
        // SceneManager.LoadScene("MainMenuScene"); // ����: SceneManager�� ����Ϸ��� using UnityEngine.SceneManagement; �ʿ�
        // Application.Quit(); // ���� ���� (���� ��)
    }
}