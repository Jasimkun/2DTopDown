using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightItem_1 : MonoBehaviour
{
    public BaseItemData_1 itemDataToAcquire;

    // �ν����Ϳ��� ������ ��ư
    public Button useButton;

    // ������ ȿ���� �̹� ����Ǿ����� üũ�ϴ� �÷���
    private bool isUsed = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (InventorySystem_1.Instance != null)
            {
                InventorySystem_1.Instance.AddItem(itemDataToAcquire);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("[LightItem_1] InventorySystem_1 �ν��Ͻ��� ã�� �� �����ϴ�.");
            }
        }
    }

    public void UseItemEffect()
    {
        if (isUsed) return;  // �̹� ��������� �ƹ� ���� �� ��

        isUsed = true;       // ��� �÷��� ����

        Debug.Log($"[{itemDataToAcquire.itemName}] ������ ȿ�� ����.");

        GameObject cameraLight = Camera.main.transform.Find("CameraLight")?.gameObject;

        if (cameraLight != null)
        {
            cameraLight.transform.localScale *= 1.5f;
            Debug.Log("CameraLight ������ ����.");
        }
        else
        {
            Debug.LogWarning("CameraLight�� ã�� �� �����ϴ�.");
        }

        if (useButton != null)
        {
            useButton.interactable = false;
            Debug.Log("������ ��� ��ư ��Ȱ��ȭ��.");
        }
        else
        {
            Debug.LogWarning("useButton�� ����Ǿ� ���� �ʽ��ϴ�.");
        }
    }
}
