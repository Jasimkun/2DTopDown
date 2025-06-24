using UnityEngine;

[CreateAssetMenu(fileName = "NewLightItem", menuName = "Inventory/Items/Light Item")]
public class LightItemData : BaseItemData
{
    [Tooltip("이 아이템 사용 시 활성화/비활성화할 조명 오브젝트입니다. (씬에 존재해야 함)")]
    public GameObject targetLightObject;


    // BaseItemData의 UseItemEffect() 메서드를 오버라이드하여 Light Item의 고유 로직 구현
    public override void UseItemEffect()
    {
        Debug.Log($"[LightItemData] '{itemName}' 아이템 사용됨!");

        if (targetLightObject != null)
        {
            bool currentActiveState = targetLightObject.activeSelf;
            targetLightObject.SetActive(!currentActiveState);
            Debug.Log($"[LightItemData] '{targetLightObject.name}' 오브젝트의 활성 상태를 {!currentActiveState}로 변경했습니다.");
        }
        else
        {
            Debug.LogWarning($"[LightItemData] '{itemName}' 아이템 사용 실패: 'targetLightObject'가 설정되지 않았습니다.");
        }
    }
    public float GetEffectiveTargetScaleMultiplier()
    {
        // 스케일을 바꾸는 용도라면 기본값 리턴
        return 1f;
    }
    public Vector3 GetEffectiveTargetScaleMultiplier(int level)
    {
        // 레벨마다 스케일이 10% 증가한다고 가정
        float scale = 1f + 0.1f * level;
        return new Vector3(scale, scale, scale);
    }

}
