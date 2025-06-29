using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActiveButton : UISubItem
{
    
    [SerializeField] private Sprite _activateSprite;
    [SerializeField] private Sprite _deactivateSprite;

    protected Image _buttonImage;
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        _buttonImage = Utils.GetOrAddComponent<Image>(gameObject);

        return true;
    }

    public void Activate()
    {
        _buttonImage.sprite = _activateSprite;
    }

    public void Deactivate()
    {
        _buttonImage.sprite = _deactivateSprite;
    }
}
