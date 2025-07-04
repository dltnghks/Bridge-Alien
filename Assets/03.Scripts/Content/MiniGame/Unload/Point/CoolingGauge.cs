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

    public void SetValue(float value)
    {
        // 값이 0과 maxValue 사이를 벗어나지 않도록 제한
        currentValue = Mathf.Clamp(value, 0, maxValue);
        UpdateGaugeVisuals();
    }

    public void SetRatio(float ratio)
    {
        // ratio가 0.0 ~ 1.0 사이인지 확인하고, maxValue에 곱하여 현재 값을 설정
        if (ratio < 0f || ratio > 1f)
        {
            Logger.LogError("CoolingGauge: Ratio must be between 0.0 and 1.0");
            return;
        }
        currentValue = ratio * maxValue;
        UpdateGaugeVisuals();
    }

    // material의 "_FillAmount" 속성을 업데이트하여 게이지의 채워진 비율을 조정
    private void UpdateGaugeVisuals()
    {
        if (gaugeRenderer == null) return;

        // 현재 값의 비율(0.0 ~ 1.0)을 계산
        float fillRatio = currentValue / maxValue;

        // 셰이더의 "_FillAmount" 속성에 계산된 비율을 전달
        gaugeRenderer.material.SetFloat("_FillAmount", fillRatio);
    }
}