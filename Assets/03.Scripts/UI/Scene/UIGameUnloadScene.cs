using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIGameUnloadScene : UIScene
{
    enum Objects
    {
        UITimer,
        UIScoreBoard,
        UIBoxPreview,
        UIOption,
    }
    
    private UITimer _uiTimer;
    private UIScoreBoard _uiScoreBoard;
    private UIBoxPreview _uiBoxPreview;
    private UIOption _uiOption;
    
    public UITimer UITimer { get { return _uiTimer; } }
    public UIScoreBoard UIScoreBoard { get { return _uiScoreBoard; } }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindObject(typeof(Objects));
        
        _uiTimer = GetObject((int)Objects.UITimer).GetOrAddComponent<UITimer>();
        _uiScoreBoard = GetObject((int)Objects.UIScoreBoard).GetOrAddComponent<UIScoreBoard>();
        _uiBoxPreview = GetObject((int)Objects.UIBoxPreview).GetOrAddComponent<UIBoxPreview>();
        _uiOption = GetObject((int)Objects.UIOption).GetOrAddComponent<UIOption>();
        
        return true;
    }
    
}
