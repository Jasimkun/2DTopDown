using UnityEngine;

[CreateAssetMenu(fileName = "NewLightItem", menuName = "Item/LightItem")]
public class LightItemData : BaseItemData
{
    [Header("스프라이트 스케일 효과 설정")]
    [Tooltip("레벨 0에서의 기본 스케일 배율입니다.")]
    public Vector3 baseTargetScaleMultiplier = new Vector3(1.5f, 1.5f, 1.5f);
    [Tooltip("레벨 1 증가당 추가되는 스케일 배율입니다.")]
    public float scaleMultiplierPerLevel = 0.1f;

    public float scaleHoldDuration = 5f;
    public int point;

    // ⬇️ [추가] 효과를 적용할 대상 오브젝트의 이름입니다.
    [Tooltip("스케일 효과를 적용할 GameObject의 이름입니다. (예: CameraOverlayImage)")]
    public string targetObjectNameForEffect = "CameraOverlayImage"; // ⬇️ [수정] 이 이름을 유니티에서 사용 중인 정확한 이름으로 변경해주세요!

    /// <summary>
    /// 주어진 레벨에 해당하는 실제 스케일 배율 값을 계산하여 반환합니다.
    /// </summary>
    /// <param name="level">계산할 업그레이드 레벨입니다.</param>
    /// <returns>해당 레벨에서의 최종 스케일 배율 값입니다.</returns>
    public Vector3 GetEffectiveTargetScaleMultiplier(int level)
    {
        return baseTargetScaleMultiplier + (Vector3.one * (level * scaleMultiplierPerLevel));
    }

    /// <summary>
    /// 아이템 사용 효과를 정의하는 메서드 (BaseItemData의 추상 메서드 오버라이드)
    /// 이 아이템은 'targetObjectNameForEffect'에 지정된 GameObject에 스케일 효과를 적용합니다.
    /// </summary>
    /// <param name="user">아이템을 사용하는 GameObject (여기서는 직접 사용되지 않고 대상 오브젝트를 찾습니다)</param>
    public override void UseItemEffect(GameObject user = null)
    {
        GameObject targetObject = GameObject.Find(targetObjectNameForEffect);

        if (targetObject != null)
        {
            ScaleEffectHandler handler = targetObject.GetComponent<ScaleEffectHandler>();
            if (handler == null)
            {
                handler = targetObject.AddComponent<ScaleEffectHandler>();
                Debug.Log($"[LightItemData] ScaleEffectHandler 컴포넌트가 '{targetObjectNameForEffect}'에 추가됨.");
            }

            handler.StartScaleEffect(this); // LightItemData 자체를 핸들러에 전달하여 레벨을 조회하도록 함
            Debug.Log($"[LightItemData] '{itemName}' 아이템 효과 사용! 스케일 효과를 '{targetObjectNameForEffect}'에 적용. (Level will be determined by ShopManager)");
        }
        else
        {
            Debug.LogWarning($"[LightItemData] '{itemName}' 아이템 효과를 적용할 대상 GameObject '{targetObjectNameForEffect}'를 씬에서 찾을 수 없습니다. 이름이 정확한지 확인하세요.");
        }
    }
}
