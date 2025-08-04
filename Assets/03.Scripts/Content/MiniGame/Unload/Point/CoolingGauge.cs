using UnityEngine;

public class CoolingGauge : MonoBehaviour
{
    [Header("Gauge Settings")]
    // [Range] 어트리뷰트를 사용하면 Inspector에서 슬라이더로 값을 조절할 수 있습니다.
    [Range(0, 10)]
    public float currentValue = 0f; // 게이지의 현재 값
    public float maxValue = 10f;     // 게이지의 최대 값

    private SpriteRenderer gaugeRenderer;

    void Start()
    {
        // 이 스크립트가 적용된 오브젝트의 Renderer 컴포넌트를 가져옵니다.
        gaugeRenderer = GetComponent<SpriteRenderer>();
        if (gaugeRenderer == null)
        {
            Logger.LogError("GaugeController: Renderer component not found on this object!");
            return;
        }
        UpdateGaugeVisuals();
    }

    public void SetValue(float value, float maxTime)
    {
        // 값이 0과 maxValue 사이를 벗어나지 않도록 제한
        currentValue = Mathf.Clamp(value, 0, maxTime);
        currentValue /= maxTime;
        UpdateGaugeVisuals();
    }

    // material의 "_FillAmount" 속성을 업데이트하여 게이지의 채워진 비율을 조정
    private void UpdateGaugeVisuals()
    {
        if (gaugeRenderer == null) return;

        gaugeRenderer.material.SetFloat("_FillAmount", currentValue);
    }
}