using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class UIHPGauge : UISubItem
{
    enum Images
    {
        HPGaugeImage,
    }
    
    private Image _hpImage;

    public override bool Init()
    {
        if(base.Init() == false){
            return false;
        }

        BindImage(typeof(Images));

        _hpImage = GetImage((int)Images.HPGaugeImage);
        _hpImage.fillAmount = 1;
        
        return true;
    }

    public void SetHP(float value, float maxHP){
        float targetFillAmount = value / maxHP;
        
        // DOTween으로 fillAmount를 부드럽게 변경
        _hpImage.DOFillAmount(targetFillAmount, 0.5f).SetEase(Ease.OutCubic); // 0.5초 동안 부드럽게 감소
    }
}
