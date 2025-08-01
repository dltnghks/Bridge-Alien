using System.Collections;
using System.Collections.Generic;
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
        // _handler.onDamageUpdateAction += UpdateUI;
    }

    private void UpdateUI(float percentage)
    {
        for (int i = 1; i <= 4; ++i)
        {
            if (i * 0.25f < percentage)
            {
                Debug.Log(percentage + " : " + i * 0.25f + " : " + i);
                _images[4 - i].enabled = false;
            }
            else
                _images[4 - i].enabled = true;
        }
    }
}
