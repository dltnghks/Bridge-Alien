using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITaskResultPopup : UIPopup
{
    enum Texts
    {
        ExperienceValueText,
        GravityAdaptationValueText,
        IntelligenceValueText,
        LuckValueText,
        TaskCompletedText,
    }

    enum Images
    {
        ExperienceValueTextIncreaseImage,
        GravityAdaptationValueTextIncreaseImage,
        IntelligenceValueTextIncreaseImage,
        LuckValueTextIncreaseImage,
        ExperienceValueTextDecreaseImage,
        GravityAdaptationValueTextDecreaseImage,
        IntelligenceValueTextDecreaseImage,
        LuckValueTextDecreaseImage,
    }

    private PlayerTaskData _selectedTaskData;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        gameObject.BindEvent(OnClickUI);

        return true;
    }

    public override void Init(object data)
    {
        base.Init(data);
        if (data is PlayerTaskData taskData)
        {
            _selectedTaskData = taskData;

            // 텍스트 세팅
            SetText();
            SetTaskStatTextImage();
        }
        else
        {
            Logger.LogError("data is not PlayerTaskData");
        }
    }

    private void OnClickUI()
    {
        ClosePopupUI();
    }

    public void SetText()
    {
        PlayerData playerData = Managers.Player.PlayerData;
        GetText((int)Texts.ExperienceValueText).text = $"{playerData.Stats[Define.PlayerStatsType.Experience]} / 100";
        GetText((int)Texts.GravityAdaptationValueText).text = $"{playerData.Stats[Define.PlayerStatsType.GravityAdaptation]} / 100";
        GetText((int)Texts.IntelligenceValueText).text = $"{playerData.Stats[Define.PlayerStatsType.Intelligence]} / 100";
        GetText((int)Texts.LuckValueText).text = $"{playerData.Stats[Define.PlayerStatsType.Luck]} / 100";

        if (_selectedTaskData != null)
        {
            GetText((int)Texts.TaskCompletedText).text = _selectedTaskData.TaskCompletedText;
        }
    }

    private void SetTaskStatTextImage()
    {
        SetTaskStatImages(false);

        // 능력치 상승치 표기
        if (_selectedTaskData.ExperienceValue > 0)
        {
            GetImage((int)Images.ExperienceValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
            GetText((int)Texts.ExperienceValueText).color = Color.green;
        }
        else if (_selectedTaskData.ExperienceValue < 0)
        {
            GetImage((int)Images.ExperienceValueTextDecreaseImage).color = new Color(1f, 1f, 1f, 1f);
            GetText((int)Texts.ExperienceValueText).color = Color.red;
        }


        if (_selectedTaskData.IntelligenceValue > 0)
        {
            GetImage((int)Images.IntelligenceValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
            GetText((int)Texts.IntelligenceValueText).color = Color.green;
        }
        else if (_selectedTaskData.IntelligenceValue < 0)
        {
            GetImage((int)Images.IntelligenceValueTextDecreaseImage).color = new Color(1f, 1f, 1f, 1f);
            GetText((int)Texts.IntelligenceValueText).color = Color.red;
        }


        if (_selectedTaskData.GravityAdaptationValue > 0)
        {
            GetImage((int)Images.GravityAdaptationValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
            GetText((int)Texts.GravityAdaptationValueText).color = Color.green;
        }
        else if (_selectedTaskData.GravityAdaptationValue < 0)
        {
            GetImage((int)Images.GravityAdaptationValueTextDecreaseImage).color = new Color(1f, 1f, 1f, 1f);
            GetText((int)Texts.GravityAdaptationValueText).color = Color.red;
        }

        if (_selectedTaskData.LuckMinValue != 0)
        {
            // 운 랜덤 상승
            GetImage((int)Images.LuckValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
    }

    private void SetTaskStatImages(bool active)
    {
        float value = active == true ? 1.0f : 0.0f;
        GetImage((int)Images.ExperienceValueTextIncreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.IntelligenceValueTextIncreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.GravityAdaptationValueTextIncreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.LuckValueTextIncreaseImage).color = new Color(1f, 1f, 1f, value);

        GetImage((int)Images.ExperienceValueTextDecreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.IntelligenceValueTextDecreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.GravityAdaptationValueTextDecreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.LuckValueTextDecreaseImage).color = new Color(1f, 1f, 1f, value);
    }


    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
    }

    public void OnDestroy()
    {
        // 일과를 수행하면 다음 이벤트로 넘어가기
        Managers.Daily.StartEvent();
    } 
}
