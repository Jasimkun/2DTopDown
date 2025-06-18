using UnityEngine;

// Unity 에디터의 Assets 메뉴에서 "Create/Item/LightItem"으로 이 ScriptableObject Asset을 생성할 수 있습니다.
[CreateAssetMenu(fileName = "NewLightItem", menuName = "Item/LightItem")]
public class LightItemData : ScriptableObject
{
    [Header("스프라이트 스케일 효과 설정")]
    [Tooltip("원본 스케일 대비 목표 스케일 배율입니다.")]
    public Vector3 targetScaleMultiplier = new Vector3(1.5f, 1.5f, 1.5f);


    [Tooltip("스케일이 커진 상태로 유지되는 시간 (초)입니다.")]
    public float scaleHoldDuration = 5f;

    public int point;      // 아이템 점수나 값 (필요하면 사용)
    public string itemName;
    public Sprite icon;

    /// <summary>
    /// 이 효과를 특정 GameObject에 적용하도록 지시합니다.
    /// 대상 GameObject에 ScaleEffectHandler 컴포넌트가 없으면 자동으로 추가합니다.
    /// </summary>
    /// <param name="targetObject">스케일 효과를 적용할 GameObject입니다.</param>
    public void ApplyEffect(GameObject targetObject)
    {
        // targetObject에 ScaleEffectHandler 컴포넌트가 있는지 확인하고 없으면 추가합니다.
        ScaleEffectHandler handler = targetObject.GetComponent<ScaleEffectHandler>();
        if (handler == null)
        {
            handler = targetObject.AddComponent<ScaleEffectHandler>();
        }

        // 핸들러에게 효과 설정을 전달하고 시작하도록 합니다.
        handler.StartScaleEffect(this);
    }
}