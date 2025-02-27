using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogPopup : UIPopup
{
    
    /*
     *
     * Popup으로 생성하고 SetDialog로 세팅해주기
     * 
     */
    
    enum Texts
    {
        NameText,
        DialogText,
    }

    enum Images
    {
        LeftCharacterImage,
        RightCharacterImage,
    }

    enum Buttons
    {
        SkipButton,
        NextButton,
    }
    
    public float TypingSpeed = 0.01f; // 글자 타이핑 속도
    
    private List<Image> _speakerCharacterImages = new List<Image>();
    private TextMeshProUGUI _dialogText;  
    private Button _nextButton;

    private List<DialogData> _currentDialogs;
    private int _currentDialogIndex;
    private Tween _typingTween; // 현재 실행 중인 트윈
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));

        _dialogText = GetText((int)Texts.DialogText);
        
        _speakerCharacterImages.Add(GetImage((int)Images.LeftCharacterImage));
        _speakerCharacterImages.Add(GetImage((int)Images.RightCharacterImage));
        
        GetButton((int)Buttons.SkipButton).gameObject.BindEvent(OnClickScreenButton);
        GetButton((int)Buttons.NextButton).gameObject.BindEvent(OnClickNextButton);
        
        _nextButton = GetButton((int)Buttons.NextButton);
        _nextButton.gameObject.SetActive(false);   
        
        UpdateDialog();
        return true;
    }

    private void OnClickScreenButton()
    {
        Logger.Log("OnClickScreenButton");
        SkipTyping();
    }
    
    private void OnClickNextButton()
    {
        Logger.Log("OnClickNextButton");
        UpdateDialog();
    }

    public void SetDialogs(Define.DialogType dialogueType)
    {
        _currentDialogs = Managers.Data.DialogDataManager.GetData(Define.DialogType.TUTORIAL_STORY_01);
        _currentDialogIndex = 0;

        if (_currentDialogs == null || _currentDialogs.Count == 0)
        {
            Logger.LogError("Dialogs not found");
            return;
        }
        UpdateDialog();
    }

    private void SetCharacterImages(int speakerIndex)
    {
        foreach (Image image in _speakerCharacterImages)
        {
            image.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
        
        Logger.Log(speakerIndex);
        // 이름으로 이미지를 변경?
        if (!(_currentDialogIndex >= _speakerCharacterImages.Count && speakerIndex < 0))
        {
            _speakerCharacterImages[speakerIndex].color = new Color(1, 1, 1, 1);
        }
    }
    
    private void UpdateDialog()
    {
        if (_currentDialogIndex >= _currentDialogs.Count)
        {
            // 대화 종료
            ClosePopupUI();
            Logger.Log("Dialog Index out of range.");
            return;
        }
        
        string speakerName = _currentDialogs[_currentDialogIndex].Character;
        string dialogText = _currentDialogs[_currentDialogIndex].Dialog;
        int speakerIndex = -1;
        switch (_currentDialogs[_currentDialogIndex].Speaker)
        {
            case "LEFT": speakerIndex = 0; break;
            case "RIGHT": speakerIndex = 1;break;
            case "NONE": speakerIndex = -1; break;
        } 
        // 이미지 변경
        SetCharacterImages(speakerIndex);
        // 이름 변경
        SetNameText(speakerName);
        // 대화 변경
        StartTyping(dialogText);
    }

    private void SetNameText(string characterName)
    {
        GetText((int)Texts.NameText).SetText(characterName);
    }
    
    public void StartTyping(string message)
    {
        if (_typingTween != null) _typingTween.Kill(); // 기존 애니메이션 정지

        // 스크립트 작성 사운드 추가
        
        _nextButton.gameObject.SetActive(false);    // 화살표 끄기
        
        _dialogText.text = ""; // 텍스트 초기화
        Sequence sequence = DOTween.Sequence(); // 시퀀스 애니메이션 생성

        for (int i = 0; i < message.Length; i++)
        {
            string currentText = message.Substring(0, i + 1); // i번째 글자까지 출력
            sequence.AppendCallback(() => _dialogText.text = currentText);
            sequence.AppendInterval(TypingSpeed); // 일정 간격 후 다음 글자 추가
            
            // 효과음 추가
            //if (typingSound != null) typingSound.Play(); // 효과음 재생
        }

        _typingTween = sequence; // 현재 실행 중인 트윈 저장
        _typingTween.OnComplete(EndTyping); // 현재 트윈이 끝나면 타이핑 종료 함수 호출
    }
    
    public void SkipTyping()
    {
        if (_typingTween != null && _typingTween.IsActive())
        {
            _typingTween.Complete(); // 즉시 전체 텍스트 출력

            _dialogText.text = _currentDialogs[_currentDialogIndex].Dialog;
        }
    }

    private void EndTyping()
    {
        _nextButton.gameObject.SetActive(true);
        _currentDialogIndex++;
    }
}
