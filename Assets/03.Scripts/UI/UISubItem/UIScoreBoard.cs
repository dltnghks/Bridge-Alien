using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScoreBoard : UISubItem
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
