using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragileBox : MiniGameUnloadBox
{
    public override void SetRandomInfo()
    {
        base.SetRandomInfo();
    }
    
    public void CheckBrokenBox(int height)
    {
        if(height > 0)
        {
            _info.IsBroken = true;
        }
        else
        {
            _info.IsBroken = false;
        }
    }
}
