using UnityEngine;
using UnityEngine.UI;

// 이 스크립트는 상점 UI를 열기 위한 버튼에 부착됩니다.
public class ShopButtonHandler : MonoBehaviour
{
    // ⬇️ [수정] ShopUI 스크립트를 직접 연결할 수 있는 필드입니다.
    // Inspector에서 ShopPanel GameObject에 붙어있는 ShopUI 컴포넌트를 여기에 드래그 앤 드롭하세요.
    public ShopUI shopUIReference;

    /// <summary>
    /// UI Button의 OnClick 이벤트에 연결됩니다.
    /// 연결된 ShopUI의 OpenShop 메서드를 호출하여 상점을 엽니다.
    /// </summary>
    public void OpenShopUI()
    {
        Debug.Log("[ShopButtonHandler] OpenShopUI 메서드가 호출되었습니다!");

        // ⬇️ [수정] FindObjectOfType 대신 직접 연결된 shopUIReference를 사용합니다.
        if (shopUIReference != null)
        {
            shopUIReference.OpenShop(); // ShopUI의 OpenShop 메서드 호출
            Debug.Log("[ShopButtonHandler] 상점 열기 버튼 클릭됨. ShopUI.OpenShop() 호출됨.");
        }
        else
        {
            Debug.LogError("[ShopButtonHandler] 'Shop UI Reference' 필드가 Inspector에 할당되지 않았습니다. 상점을 열 수 없습니다. ShopUI 스크립트가 붙은 GameObject의 ShopUI 컴포넌트를 여기에 연결해주세요.");
        }
    }
}
