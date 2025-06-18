using UnityEngine;
using System.Collections; // Coroutine을 사용하기 위해 필요합니다.

public class ScaleEffectHandler : MonoBehaviour
{
    private Vector3 originalLocalScale; // 이 GameObject의 시작 로컬 스케일을 저장합니다.
    private Coroutine currentScaleCoroutine; // 현재 실행 중인 스케일 코루틴의 참조입니다.

    void Awake()
    {
        // Awake에서 원본 스케일을 저장합니다. 이는 스크립트가 활성화될 때 한 번만 호출됩니다.
        originalLocalScale = transform.localScale;
    }

    /// <summary>
    /// LightItemData ScriptableObject에서 효과 설정을 받아 스케일 변경 코루틴을 시작합니다.
    /// </summary>
    /// <param name="effectData">적용할 LightItemData ScriptableObject 인스턴스입니다.</param>
    public void StartScaleEffect(LightItemData effectData)
    {
        // 이미 실행 중인 스케일 코루틴이 있다면 중지하여 중복 실행을 방지합니다.
        if (currentScaleCoroutine != null)
        {
            StopCoroutine(currentScaleCoroutine);
        }

        // 새로운 스케일 변경 코루틴을 시작하고 그 참조를 저장합니다.
        currentScaleCoroutine = StartCoroutine(ScaleRoutine(effectData));
    }

    private IEnumerator ScaleRoutine(LightItemData effectData)
    {
        Vector3 currentScale;
        Vector3 targetScale;
        //float timer;

        // 1. 스케일을 키우는 단계: 현재 스케일에서 목표 스케일까지 부드럽게 커집니다.
        currentScale = transform.localScale;
        targetScale = originalLocalScale;
        targetScale.x *= effectData.targetScaleMultiplier.x;
        targetScale.y *= effectData.targetScaleMultiplier.y;
        targetScale.z *= effectData.targetScaleMultiplier.z;
        //timer = 0f;

        //while (timer < effectData.scaleUpDuration)
        {
            //transform.localScale = Vector3.Lerp(currentScale, targetScale, timer / effectData.scaleUpDuration);
            //timer += Time.deltaTime; // 프레임 속도에 독립적으로 시간을 증가시킵니다.
            //yield return null; // 다음 프레임까지 대기합니다.
        }
        //transform.localScale = targetScale; // 정확히 목표 스케일로 설정합니다.

        // 2. 스케일이 커진 상태로 유지되는 단계: 설정된 시간 동안 커진 스케일을 유지합니다.
        yield return new WaitForSeconds(effectData.scaleHoldDuration);

        // 3. 스케일을 원본으로 되돌리는 단계: 커진 스케일에서 원본 스케일까지 부드럽게 작아집니다.
        currentScale = transform.localScale; // 현재 커져있는 스케일을 시작점으로 설정합니다.
        //timer = 0f;

        //while (timer < effectData.scaleDownDuration)
        {
            //transform.localScale = Vector3.Lerp(currentScale, originalLocalScale, timer / effectData.scaleDownDuration);
            //timer += Time.deltaTime; // 프레임 속도에 독립적으로 시간을 증가시킵니다.
            //yield return null; // 다음 프레임까지 대기합니다.
        }
        transform.localScale = originalLocalScale; // 정확히 원본 스케일로 설정합니다.

        currentScaleCoroutine = null; // 코루틴이 완료되었음을 표시합니다.
    }
}