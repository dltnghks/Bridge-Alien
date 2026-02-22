using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageStarGroup : UISubItem
{
    enum Images
    {
        Star1,
        Star2,
        Star3,
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
        SetStarCount(starCount, 3);
    }

    public void SetStarCount(int starCount, int maxStarCount)
    {
        Init();

        for (int i = 0; i < 3; i++)
        {
            var image = GetImage(i);
            if (image == null)
            {
                continue;
            }

            bool isVisible = i < maxStarCount;
            var imageColor = image.color;
            imageColor.a = isVisible ? 1f : 0f;
            image.color = imageColor;

            var activeButton = image.gameObject.GetComponent<UIActiveButton>();
            if (activeButton == null)
            {
                continue;
            }

            if (!isVisible)
            {
                activeButton.Deactivate();
                continue;
            }

            if (i + 1 <= starCount)
                activeButton.Activate();
            else
                activeButton.Deactivate();
        }
    }
}
