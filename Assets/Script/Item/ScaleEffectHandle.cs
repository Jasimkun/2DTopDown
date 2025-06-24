using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 필요합니다.

// ScaleEffectHandler는 LightItemData에 정의된 스케일 변화 효과를 처리합니다.
// 이 컴포넌트는 플레이어 또는 효과를 받을 대상 GameObject에 추가됩니다.
public class ScaleEffectHandler : MonoBehaviour
{
    private Vector3 originalScale; // 효과 적용 전 GameObject의 원래 스케일
    private Coroutine currentScaleCoroutine; // 현재 실행 중인 코루틴 참조

    [Header("기본 스케일 효과 설정")]
    [Tooltip("기본 목표 스케일 배율입니다. StartScaleEffect 호출 시 명시적으로 전달되지 않을 경우 사용됩니다.")]
    public Vector3 defaultTargetScaleMultiplier = new Vector3(1.5f, 1.5f, 1.5f); // 기본값 설정

    [Tooltip("기본 스케일 효과 지속 시간 (초)입니다. StartScaleEffect 호출 시 명시적으로 전달되지 않을 경우 사용됩니다.")]
    public float defaultScaleHoldDuration = 3.0f; // 기본값 설정

    void Awake()
    {
        // GameObject의 원래 스케일을 저장합니다.
        originalScale = transform.localScale;
        Debug.Log($"[ScaleEffectHandler] Awake: 원래 스케일 저장됨 - {originalScale}");
    }

    /// <summary>
    /// 스케일 효과를 시작합니다.
    /// 이 메서드는 특정 LightItemData에 의존하지 않습니다.
    /// </summary>
    /// <param name="targetMultiplier">원래 스케일에 곱할 목표 배율입니다.</param>
    /// <param name="duration">스케일이 유지될 시간입니다.</param>
    public void StartScaleEffect(Vector3 targetMultiplier, float duration)
    {
        // 이미 실행 중인 코루틴이 있다면 중지합니다.
        if (currentScaleCoroutine != null)
        {
            StopCoroutine(currentScaleCoroutine);
            Debug.Log("[ScaleEffectHandler] 기존 스케일 코루틴 중지됨.");
        }

        Debug.Log($"[ScaleEffectHandler] 스케일 효과 시작. 목표 배율: {targetMultiplier}, 지속 시간: {duration}초.");

        // 스케일 효과 코루틴을 시작합니다.
        currentScaleCoroutine = StartCoroutine(ScaleRoutine(targetMultiplier, duration));
    }

    /// <summary>
    /// 기본값으로 스케일 효과를 시작합니다.
    /// </summary>
    public void StartDefaultScaleEffect()
    {
        StartScaleEffect(defaultTargetScaleMultiplier, defaultScaleHoldDuration);
        Debug.Log("[ScaleEffectHandler] 기본 스케일 효과 시작됨.");
    }


    /// <summary>
    /// 스케일 변화 효과를 코루틴으로 구현합니다.
    /// 1. 목표 스케일로 즉시 변경
    /// 2. 일정 시간 유지
    /// 3. 원래 스케일로 되돌리기
    /// </summary>
    /// <param name="targetMultiplier">원래 스케일에 곱할 목표 배율입니다.</param>
    /// <param name="duration">스케일이 유지될 시간입니다.</param>
    private IEnumerator ScaleRoutine(Vector3 targetMultiplier, float duration)
    {
        // 1. 목표 스케일로 즉시 변경
        // 원래 스케일에 목표 배율을 곱하여 최종 목표 스케일을 계산합니다.
        Vector3 newScale = new Vector3(
            originalScale.x * targetMultiplier.x,
            originalScale.y * targetMultiplier.y,
            originalScale.z * targetMultiplier.z
        );
        transform.localScale = newScale;
        Debug.Log($"[ScaleEffectHandler] 스케일 변경됨: {transform.localScale}");

        // 2. 일정 시간 유지
        yield return new WaitForSeconds(duration);
        Debug.Log($"[ScaleEffectHandler] 스케일 {duration}초 유지 완료.");

        // 3. 원래 스케일로 되돌리기
        transform.localScale = originalScale;
        Debug.Log($"[ScaleEffectHandler] 스케일이 원래 크기 ({originalScale})로 돌아옴.");

        currentScaleCoroutine = null; // 코루틴 완료를 표시
    }

    void OnDestroy()
    {
        // GameObject가 파괴될 때 실행 중인 코루틴이 있다면 중지합니다.
        if (currentScaleCoroutine != null)
        {
            StopCoroutine(currentScaleCoroutine);
            Debug.Log("[ScaleEffectHandler] GameObject 파괴 시 스케일 코루틴 중지됨.");
        }
    }
}
