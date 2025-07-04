using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColdBox : MiniGameUnloadBox
{
    [Header("설정")]
    public float coolingTime = 10f; // 냉장상태 전환 시간

    [SerializeField]
    private Sprite[] _coolingSprites; // 냉장상태로 전환 시 사용할 스프라이트들

    private SpriteRenderer objectRenderer;
    private Material originalMaterial;
    private Color originalColor;
    private bool isCooling = false;
    private bool isColdbox = false;
    private Action<float> _onCoolingProgress;

    void Start()
    {
        objectRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = objectRenderer.material;
        originalColor = originalMaterial.color;
    }

    // 미니게임 구역에 배치 시 호출될 메서드
    public void EnterCoolingArea(Action<float> onCoolingProgress = null)
    {
        _onCoolingProgress = onCoolingProgress;
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
            if (Managers.MiniGame.CurrentGame.IsPause)
            {
                yield return null; // 일시정지 상태에서는 대기
                continue;
            }


            timer += Time.deltaTime;
            float lerpRatio = timer / coolingTime;
            _onCoolingProgress?.Invoke(timer); // 냉각 진행률 알림

            SetCoolingSprite(lerpRatio); // 냉각 상태에 따라 스프라이트 변경

            yield return null;
        }

        _onCoolingProgress?.Invoke(0); // 냉각 진행률 알림
        isColdbox = true;
        isCooling = false;
        BoxType = Define.BoxType.Normal;
        SetCoolingSprite(1); // 냉각 상태에 따라 스프라이트 변경
    }

    // 바로 냉장상태로 전환
    public void DirectCooling()
    {
        if (!isColdbox && !isCooling)
        {
            isColdbox = true;
            isCooling = true;
            BoxType = Define.BoxType.Normal;
            SetCoolingSprite(1); // 냉각 상태에 따라 스프라이트 변경
        }
    }

    private void SetCoolingSprite(float ratio)
    {
        if (_coolingSprites.Length > 0)
        {
            int index = Mathf.Clamp(Mathf.FloorToInt(ratio * _coolingSprites.Length), 0, _coolingSprites.Length - 1);
            objectRenderer.sprite = _coolingSprites[index];
        }
    }
    
    public override void SetRandomInfo()
    {
        base.SetRandomInfo();
        _info.BoxType = Define.BoxType.Cold;
    }
}
