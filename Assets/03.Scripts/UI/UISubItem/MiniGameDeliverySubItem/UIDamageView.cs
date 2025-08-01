using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIDamageView : UISubItem
{
    private DamageHandler _handler;

    private enum Images
    {
        First,
        Second,
        Third,
        Fourth,
    }

    private Image[] _images;
    private Coroutine _fillCoroutine;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        _images = new Image[4];
        for (int i = 0; i < 4; ++i)
            _images[i] = GetImage(i);

        return true;
    }

    public void Initialize(DamageHandler damageHandler, UnityAction endAction = null)
    {
        _handler = damageHandler;
        _handler.OnDamageRateChanged += UpdateUI;
    }

    private void UpdateUI(float percentage)
    {
        if (_fillCoroutine != null)
            StopCoroutine(_fillCoroutine);
        if (percentage > 1f)
            return;
        
        _fillCoroutine = StartCoroutine(FillRoutine(percentage));
    }

    private IEnumerator FillRoutine(float percentage)
    {
        const float fillSpeed = 1.2f;

        if (percentage < 0.01f)
        {
            OnResetFill();
        }
        else
        {
            for (int i = 0; i < 4; ++i)
            {
                float segmentStart = i * 0.25f;
                float segmentEnd = (i + 1) * 0.25f;
                float targetFill = Mathf.InverseLerp(segmentStart, segmentEnd, percentage);
                targetFill = Mathf.Clamp01(targetFill);

                float current = _images[i].fillAmount;
                while (Mathf.Abs(current - targetFill) > 0.01f)
                {
                    current = Mathf.MoveTowards(current, targetFill, Time.deltaTime * fillSpeed);
                    _images[i].fillAmount = current;
                    yield return null;
                }

                _images[i].fillAmount = targetFill;
            }
        }
    }

    public void OnResetFill()
    {
        for (int i = 0; i < 4; ++i)
        {
            _images[i].fillAmount = 0f;
        }
    }
}