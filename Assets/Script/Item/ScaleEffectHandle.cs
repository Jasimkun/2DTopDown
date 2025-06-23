using UnityEngine;
using System.Collections;

public class ScaleEffectHandler : MonoBehaviour
{
    private Vector3 originalLocalScale;
    private Coroutine currentScaleCoroutine;

    void Awake()
    {
        originalLocalScale = transform.localScale;
    }

    /// <summary>
    /// LightItemData ScriptableObject에서 효과 설정을 받아 스케일 변경 코루틴을 시작합니다.
    /// </summary>
    /// <param name="effectData">적용할 LightItemData ScriptableObject 인스턴스입니다.</param>
    public void StartScaleEffect(LightItemData effectData) // ⬇️ [수정] LightItemData 타입으로 받음
    {
        // 이미 실행 중인 스케일 코루틴이 있다면 중지하여 중복 실행을 방지합니다.
        if (currentScaleCoroutine != null)
        {
            StopCoroutine(currentScaleCoroutine);
        }

        // 새로운 스케일 변경 코루틴을 시작하고 그 참조를 저장합니다.
        currentScaleCoroutine = StartCoroutine(ScaleRoutine(effectData));
    }

    private IEnumerator ScaleRoutine(LightItemData effectData) // ⬇️ [수정] LightItemData 타입으로 받음
    {
        // ShopManager로부터 LightItem의 현재 업그레이드 레벨을 가져옵니다.
        int currentLevel = 0;
        if (ShopManager.Instance != null && effectData != null)
        {
            currentLevel = ShopManager.Instance.GetItemUpgradeLevel(effectData.itemName);
        }

        Vector3 currentScale = transform.localScale;
        // LightItemData의 GetEffectiveTargetScaleMultiplier 메서드를 사용하여 레벨에 따른 최종 스케일 배율을 계산합니다.
        Vector3 effectiveMultiplier = effectData.GetEffectiveTargetScaleMultiplier(currentLevel);

        Vector3 targetScale = originalLocalScale;
        targetScale.x *= effectiveMultiplier.x;
        targetScale.y *= effectiveMultiplier.y;
        targetScale.z *= effectiveMultiplier.z;

        // 여기서는 즉시 스케일을 변경하고 유지 시간 후에 원본으로 돌아가도록 구현했습니다.
        // 부드러운 스케일 업/다운 애니메이션을 원한다면 Lerp 부분을 다시 활성화할 수 있습니다.

        // 1. 스케일을 키우는 단계 (즉시 변경)
        transform.localScale = targetScale;

        // 2. 스케일이 커진 상태로 유지되는 단계
        yield return new WaitForSeconds(effectData.scaleHoldDuration);

        // 3. 스케일을 원본으로 되돌리는 단계 (즉시 변경)
        transform.localScale = originalLocalScale;

        currentScaleCoroutine = null; // 코루틴이 완료되었음을 표시합니다.
    }
}
