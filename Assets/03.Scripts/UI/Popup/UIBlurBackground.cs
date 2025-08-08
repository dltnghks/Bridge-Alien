using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBlurBackground : UIPopup
{
    public bool IsInputEnabled { get; set; }
    public bool IsBlurActive { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        IsInputEnabled = false;

        gameObject.BindEvent(OnClickBackground);

        return true;
    }

    private void OnClickBackground()
    {
        if (IsInputEnabled)
        {
            Managers.UI.ClosePopupUI();
        }
    } 
    
    public void SetActive(bool isActive)
    {
        if (isActive)
        {
            transform.SetParent(Managers.UI.SceneUI.transform);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.SetParent(Managers.UI.transform);
        }
        IsBlurActive = isActive;
        gameObject.SetActive(isActive);
    }
}
