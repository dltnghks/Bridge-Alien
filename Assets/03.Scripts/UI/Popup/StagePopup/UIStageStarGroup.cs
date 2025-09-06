using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStageStarGroup : UISubItem
{
    enum Images
    {
        Star1,
        Star2,
        Star3,
        StageClearIcon,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindImage(typeof(Images));

        return true;
    }

    public void SetStarCount(int starCount)
    {
        Init();

        for (int i = 0; i < 3; i++)
        {
            if (i + 1 <= starCount)
                GetImage(i).gameObject.GetComponent<UIActiveButton>().Activate();
            else
                GetImage(i).gameObject.GetComponent<UIActiveButton>().Deactivate();
        }

        GetImage((int)Images.StageClearIcon).color = Color.clear;
        if (starCount > 0)
        {
            GetImage((int)Images.StageClearIcon).color = Color.white;
        }
    }
}
