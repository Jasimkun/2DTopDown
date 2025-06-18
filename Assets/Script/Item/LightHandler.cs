using UnityEngine;
using System.Collections; // Coroutine�� ����ϱ� ���� �ʿ��մϴ�.

public class ScaleEffectHandler : MonoBehaviour
{
    private Vector3 originalLocalScale; // �� GameObject�� ���� ���� �������� �����մϴ�.
    private Coroutine currentScaleCoroutine; // ���� ���� ���� ������ �ڷ�ƾ�� �����Դϴ�.

    void Awake()
    {
        // Awake���� ���� �������� �����մϴ�. �̴� ��ũ��Ʈ�� Ȱ��ȭ�� �� �� ���� ȣ��˴ϴ�.
        originalLocalScale = transform.localScale;
    }

    /// <summary>
    /// LightItemData ScriptableObject���� ȿ�� ������ �޾� ������ ���� �ڷ�ƾ�� �����մϴ�.
    /// </summary>
    /// <param name="effectData">������ LightItemData ScriptableObject �ν��Ͻ��Դϴ�.</param>
    public void StartScaleEffect(LightItemData effectData)
    {
        // �̹� ���� ���� ������ �ڷ�ƾ�� �ִٸ� �����Ͽ� �ߺ� ������ �����մϴ�.
        if (currentScaleCoroutine != null)
        {
            StopCoroutine(currentScaleCoroutine);
        }

        // ���ο� ������ ���� �ڷ�ƾ�� �����ϰ� �� ������ �����մϴ�.
        currentScaleCoroutine = StartCoroutine(ScaleRoutine(effectData));
    }

    private IEnumerator ScaleRoutine(LightItemData effectData)
    {
        Vector3 currentScale;
        Vector3 targetScale;
        //float timer;

        // 1. �������� Ű��� �ܰ�: ���� �����Ͽ��� ��ǥ �����ϱ��� �ε巴�� Ŀ���ϴ�.
        currentScale = transform.localScale;
        targetScale = originalLocalScale;
        targetScale.x *= effectData.targetScaleMultiplier.x;
        targetScale.y *= effectData.targetScaleMultiplier.y;
        targetScale.z *= effectData.targetScaleMultiplier.z;
        //timer = 0f;

        //while (timer < effectData.scaleUpDuration)
        {
            //transform.localScale = Vector3.Lerp(currentScale, targetScale, timer / effectData.scaleUpDuration);
            //timer += Time.deltaTime; // ������ �ӵ��� ���������� �ð��� ������ŵ�ϴ�.
            //yield return null; // ���� �����ӱ��� ����մϴ�.
        }
        //transform.localScale = targetScale; // ��Ȯ�� ��ǥ �����Ϸ� �����մϴ�.

        // 2. �������� Ŀ�� ���·� �����Ǵ� �ܰ�: ������ �ð� ���� Ŀ�� �������� �����մϴ�.
        yield return new WaitForSeconds(effectData.scaleHoldDuration);

        // 3. �������� �������� �ǵ����� �ܰ�: Ŀ�� �����Ͽ��� ���� �����ϱ��� �ε巴�� �۾����ϴ�.
        currentScale = transform.localScale; // ���� Ŀ���ִ� �������� ���������� �����մϴ�.
        //timer = 0f;

        //while (timer < effectData.scaleDownDuration)
        {
            //transform.localScale = Vector3.Lerp(currentScale, originalLocalScale, timer / effectData.scaleDownDuration);
            //timer += Time.deltaTime; // ������ �ӵ��� ���������� �ð��� ������ŵ�ϴ�.
            //yield return null; // ���� �����ӱ��� ����մϴ�.
        }
        transform.localScale = originalLocalScale; // ��Ȯ�� ���� �����Ϸ� �����մϴ�.

        currentScaleCoroutine = null; // �ڷ�ƾ�� �Ϸ�Ǿ����� ǥ���մϴ�.
    }
}