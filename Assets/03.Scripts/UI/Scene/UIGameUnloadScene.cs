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
        UIGameMenu,
        UIMiniGameUnloadPlayerInput,
        UICombo,
        UIComboBoxView
    }
    
    private UITimer _uiTimer;
    private UIScoreBoard _uiScoreBoard;
    private UIBoxPreview _uiBoxPreview;
    private UIMiniGameHelpButton _uiGameHelpButton;
    private UIMiniGameUnloadPlayerInput _uiPlayerInput;
    private UIComboDisplay _uiComboDisplay;
    private UIComboBoxView _uiComboBoxView;
    public UITimer UITimer { get { return _uiTimer; } }
    public UIScoreBoard UIScoreBoard { get { return _uiScoreBoard; } }
    public UIBoxPreview UIBoxPreview { get { return _uiBoxPreview; } }
    public UIMiniGameUnloadPlayerInput UIPlayerInput { get { return _uiPlayerInput; } }
    public UIComboDisplay UIComboDisplay { get { return _uiComboDisplay; } }
    public UIComboBoxView UIComboBoxView {get { return _uiComboBoxView; }}
    
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
        _uiGameHelpButton = GetObject((int)Objects.UIGameMenu).GetOrAddComponent<UIMiniGameHelpButton>();
        _uiPlayerInput = GetObject((int)Objects.UIMiniGameUnloadPlayerInput).GetOrAddComponent<UIMiniGameUnloadPlayerInput>();
        _uiComboDisplay = GetObject((int)Objects.UICombo).GetOrAddComponent<UIComboDisplay>();
        _uiComboBoxView = GetObject((int)Objects.UIComboBoxView).GetOrAddComponent<UIComboBoxView>();

        return true;
    }
    
}
