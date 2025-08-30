using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragileBox : MiniGameUnloadBox
{
    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
    }
    
    public override void SetRandomInfo()
    {
        base.SetRandomInfo();
        BoxType = Define.BoxType.Fragile;
    }
    
    public void CheckBrokenBox(int height)
    {
        if(height > 1)
        {
            _info.IsBroken = true;
        }
        else
        {
            _info.IsBroken = false;
        }
    }
}
