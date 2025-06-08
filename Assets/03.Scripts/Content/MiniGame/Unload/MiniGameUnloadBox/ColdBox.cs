using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColdBox : MiniGameUnloadBox
{
    [Header("설정")]
    public float coolingTime = 1f; // 냉장상태 전환 시간
    private Material runtimeMaterial;

    private SpriteRenderer objectRenderer;
    private Material originalMaterial;
    private Color originalColor;
    private bool isCooling = false;
    private bool isColdbox = false;

    void Start()
    {
        objectRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = objectRenderer.material;
        originalColor = originalMaterial.color;
        
        // 인스턴스 머티리얼 생성 (원본 머티리얼 보존)
        runtimeMaterial = new Material(originalMaterial);
        objectRenderer.material = runtimeMaterial;
    }

    // 미니게임 구역에 배치 시 호출될 메서드
    public void EnterCoolingArea()
    {
        if (!isColdbox && !isCooling)
        {
            StartCoroutine(CoolingProcess());
        }
    }

    IEnumerator CoolingProcess()
    {
        isCooling = true;
        float timer = 0f;

        
        while (timer < coolingTime)
        {
            timer += Time.deltaTime;
            float lerpRatio = timer / coolingTime;
            // r값을 1(원래값)에서 0(혹은 0.1 등)으로 낮춤
            float newR = Mathf.Lerp(originalColor.r, 0.1f, lerpRatio); // 0f 대신 0.1f 등 원하는 값 사용 가능
            Color newColor = new Color(newR, originalColor.g, originalColor.b, originalColor.a);
            runtimeMaterial.color = newColor;
            yield return null;
        }

        isColdbox = true;
        isCooling = false;
        BoxType = Define.BoxType.Normal;
    }
    
    public override void SetRandomInfo()
    {
        base.SetRandomInfo();
        _info.BoxType = Define.BoxType.Cold;
    }
}
