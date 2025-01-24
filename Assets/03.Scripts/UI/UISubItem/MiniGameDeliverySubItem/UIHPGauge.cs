using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIHPGauge : UISubItem
{
    enum Images{
        HPGaugeImage,

    }

    private Image _hpImage;

    private float _maxHP = 100f;
    private float _curHP = 100f;

    public override bool Init()
    {
        if(base.Init() == false){
            return false;
        }

        BindImage(typeof(Images));

        _hpImage = GetImage((int)Images.HPGaugeImage);

        return true;
    }

    public void InitHP(float maxHP){
        _maxHP = maxHP;
        _hpImage.fillAmount = 1;
    }

    public void SetHP(float value){
        _curHP = value;
        _hpImage.fillAmount = _curHP/_maxHP;
    }

}
