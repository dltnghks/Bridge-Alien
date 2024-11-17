using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIGameUnloadResultPopup : UIConfirmPopup
{
    enum Texts
    {
        ScoreText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindText(typeof(Texts));
        
        return true;
    }

    public void SetResultScore(int score)
    {
        gameObject.transform.DOScale(Vector3.one, 2);
        if (Init())
        {
            string scoreText = $"Score : {score.ToString()}";
            GetText((int)Texts.ScoreText).SetText(scoreText);
        }
    }

    protected override void OnClickConfirmButton()
    {
        Managers.Scene.ChangeScene(Define.Scene.GameMap);
    } 
    
}
