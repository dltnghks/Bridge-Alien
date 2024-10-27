using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoadingPopup : UIPopup
{
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        return true;
    }
}
