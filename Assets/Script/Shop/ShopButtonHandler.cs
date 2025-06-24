using UnityEngine;
using UnityEngine.UI;

// 이 스크립트는 상점 UI를 열기 위한 버튼에 부착됩니다.
public class ShopButtonHandler : MonoBehaviour
{
    // ⬇️ 추가: Inspector에서 직접 연결할 상점 패널 GameObject
    // 이 패널을 활성화/비활성화하여 상점 UI를 열고 닫습니다.
    [SerializeField] private GameObject shopPanelGameObject;

    /// <summary>
    /// UI Button의 OnClick 이벤트에 연결됩니다.
    /// 상점 패널을 활성화하고, ShopUI 스크립트를 찾아 UI 내용을 갱신합니다.
    /// </summary>
    public void OpenShopUI()
    {
        Debug.Log("[ShopButtonHandler] OpenShopUI 메서드가 호출되었습니다!");

        // Inspector에서 상점 패널 GameObject가 할당되었는지 확인합니다.
        if (shopPanelGameObject == null)
        {
            Debug.LogError("[ShopButtonHandler] 'Shop Panel GameObject' 필드가 Inspector에 할당되지 않았습니다. 상점을 열 수 없습니다.");
            return;
        }

        // 1. 먼저 상점 패널 GameObject 자체를 활성화합니다.
        // 이렇게 하면 Panel이 씬에서 비활성화되어 있었더라도 열리게 됩니다.
        shopPanelGameObject.SetActive(true);
        Debug.Log("[ShopButtonHandler] 상점 패널 GameObject가 직접 활성화되었습니다.");

        // 2. 이제 상점 패널에 붙어있는 ShopUI 스크립트를 찾습니다.
        // FindObjectOfType(true)를 사용하면 씬에서 비활성화된 GameObject에 붙은 컴포넌트도 찾을 수 있습니다.
        ShopUI shopUI = FindObjectOfType<ShopUI>(true);

        if (shopUI != null)
        {
            // ShopUI 스크립트를 찾았다면, 그 스크립트의 OpenShop() 메서드를 호출합니다.
            // OpenShop() 내부에서 RefreshShopUI()가 호출되어 상점 내용이 갱신됩니다.
            shopUI.OpenShop();
            Debug.Log("[ShopButtonHandler] ShopUI 스크립트를 찾았고, OpenShop() 메서드를 호출하여 UI 내용을 갱신했습니다.");
        }
        else
        {
            Debug.LogWarning("[ShopButtonHandler] 씬에서 ShopUI 스크립트를 찾을 수 없습니다. Panel은 열렸지만 UI 내용은 갱신되지 않습니다. ShopUI 스크립트가 'shopPanelGameObject'에 붙어 있는지 확인하세요.");
        }
    }
}
