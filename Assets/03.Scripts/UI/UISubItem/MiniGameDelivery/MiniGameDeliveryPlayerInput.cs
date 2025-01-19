using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameDeliveryPlayerInput : UIPlayerInput
{
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        _init = true;
        
        return _init;
    }
}
