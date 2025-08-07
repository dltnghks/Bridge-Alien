using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComboBoxView : UISubItem
{
    enum Objects
    {
        UISmallBoxPreview1,
        UISmallBoxPreview2,
        UISmallBoxPreview3
    }

    private List<UISmallBoxPreview> _uiSmallBoxPreviewList = new List<UISmallBoxPreview>();

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindObject(typeof(Objects));

        _uiSmallBoxPreviewList.Add(GetObject((int)Objects.UISmallBoxPreview1).GetOrAddComponent<UISmallBoxPreview>());
        _uiSmallBoxPreviewList.Add(GetObject((int)Objects.UISmallBoxPreview2).GetOrAddComponent<UISmallBoxPreview>());
        _uiSmallBoxPreviewList.Add(GetObject((int)Objects.UISmallBoxPreview3).GetOrAddComponent<UISmallBoxPreview>());

        return true;
    }
    

    public void UpdateUI(MiniGameUnloadBoxList updateBoxList)
    {
        List<MiniGameUnloadBox> boxList = updateBoxList.BoxList;
        int index = boxList.Count-1;
        for (int i = 0; i < _uiSmallBoxPreviewList.Count; i++)
        {
            MiniGameUnloadBox box = null;
            if (0 <= index && index < boxList.Count)
            {
                box = boxList[i];
            }
            _uiSmallBoxPreviewList[i].SetBoxPreview(box);   
        }   

    }
}
