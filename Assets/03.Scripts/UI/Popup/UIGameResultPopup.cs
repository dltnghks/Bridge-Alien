using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameResultPopup : UIPopup
{
    enum Buttons
    {
        ConfirmButton,
    }
    
    enum Texts
    {
        ScoreText,
        ConfirmButtonText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        
        GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(OnClickConfirmButton);
        
        return true;
    }

    public void SetResultScore(int score)
    {
        if (Init())
        {
            string scoreText = $"Score : {score.ToString()}";
            GetText((int)Texts.ScoreText).SetText(scoreText);
        }
    }

    protected void OnClickConfirmButton()
    {
        Managers.Scene.ChangeScene(Define.Scene.GameMap);
    } 
    
}
