using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightItem_1 : MonoBehaviour
{
    public BaseItemData_1 itemDataToAcquire;

    // 인스펙터에서 연결할 버튼
    public Button useButton;

    // 아이템 효과가 이미 실행되었는지 체크하는 플래그
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
                Debug.LogWarning("[LightItem_1] InventorySystem_1 인스턴스를 찾을 수 없습니다.");
            }
        }
    }

    public void UseItemEffect()
    {
        if (isUsed) return;  // 이미 사용했으면 아무 동작 안 함

        isUsed = true;       // 사용 플래그 세움

        Debug.Log($"[{itemDataToAcquire.itemName}] 아이템 효과 실행.");

        GameObject cameraLight = Camera.main.transform.Find("CameraLight")?.gameObject;

        if (cameraLight != null)
        {
            cameraLight.transform.localScale *= 1.5f;
            Debug.Log("CameraLight 스케일 증가.");
        }
        else
        {
            Debug.LogWarning("CameraLight를 찾을 수 없습니다.");
        }

        if (useButton != null)
        {
            useButton.interactable = false;
            Debug.Log("아이템 사용 버튼 비활성화됨.");
        }
        else
        {
            Debug.LogWarning("useButton이 연결되어 있지 않습니다.");
        }
    }
}
