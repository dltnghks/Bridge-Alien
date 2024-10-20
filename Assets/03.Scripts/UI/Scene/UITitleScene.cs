using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITitleScene : UIScene
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
