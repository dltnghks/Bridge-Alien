using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFatigueIconGroup : UISubItem
{
    enum Images
    {
        FatigueIcon1,
        FatigueIcon2,
        FatigueIcon3,
    }

    private UIActiveButton[] _fatigueIcons = new UIActiveButton[3];

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindImage(typeof(Images));
        
        for (int i = 0; i < 3; i++)
        {
            _fatigueIcons[i] = GetImage(i).gameObject.GetOrAddComponent<UIActiveButton>();
        }

        return true;
    }

    public void SetFatigue(int fatigue)
    {
        Init();
        
        int iconCount = fatigue;
        for (int i = 0; i < 3; i++)
        {
            _fatigueIcons[i].SetActive(i < iconCount);
        }
    }

    public Image GetFatigueIconImage(int index)
    {
        Init();

        if (index < 0 || index >= _fatigueIcons.Length)
        {
            return null;
        }

        return GetImage(index);
    }
}
