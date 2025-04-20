using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIChoiceButton : UISubItem
{
    enum Buttons
    {
        ChoiceButton,
    }

    enum Texts
    {
        ChoiceButtonText,
    }
    
    private Action<string> _callback;
    private DialogData _data;
    private int _choiceNumber;
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        
        GetButton((int)Buttons.ChoiceButton).gameObject.BindEvent(OnClickButton);
        
        return true;
    }

    private void OnClickButton()
    {
        Managers.Player.PlayerData.ChoiceNumber = _choiceNumber;
        _callback?.Invoke(_data.NextDialogID);
    }
    
    public void SetChoiceButton(DialogData data, Action<string> callback, int choice)
    {
        Init();

        _data = data;
        GetText((int)Texts.ChoiceButtonText).text = _data.Script;
        _callback = callback;
        _choiceNumber = choice;
    }
    
}
