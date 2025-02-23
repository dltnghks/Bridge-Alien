using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIDialog : UIPopup
{
    enum Texts
    {
        NameText,
        DialogText,
    }

    enum Images
    {
        LeftCharacterImage,
        RightCharacterImage,
        ArrowImage,
    }
    
    public float TypingSpeed = 0.01f; // 글자 타이핑 속도
    
    
    private TextMeshProUGUI _dialogueText;  
    private Image _arrowImage;
    
    private string _currentText;
    private string _dialogType;
    private Tween _typingTween; // 현재 실행 중인 트윈
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        _dialogueText = GetText((int)Texts.DialogText);
        
        _arrowImage = GetImage((int)Images.ArrowImage);
        _arrowImage.gameObject.SetActive(false);    
        
        UpdateDialog();
        return true;
    }

    // 지울 함수
    private void Update()
    {
        // 테스트 코드
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (_typingTween == null || !_typingTween.IsActive())
            {
                UpdateDialog();
            }
            else
            {
                SkipTyping();
            }
        }
    }


    private void UpdateDialog()
    {
        // 이름 변경
        SetNameText("test");
        // 이미지 변경
        StartTyping("Google Cloud 설정 없이 간단하게 하고 싶다면, Google Apps Script를 사용하여 JSON을 반환하는 URL을 만들 수도 있어.");
    }

    private void SetNameText(string characterName)
    {
        GetText((int)Texts.NameText).SetText(characterName);
    }
    
    public void StartTyping(string message)
    {
        if (_typingTween != null) _typingTween.Kill(); // 기존 애니메이션 정지

        // 스크립트 작 사운드 추가
        
        _arrowImage.gameObject.SetActive(false);    // 화살표 끄기
        
        _currentText = message;
        _dialogueText.text = ""; // 텍스트 초기화
        Sequence sequence = DOTween.Sequence(); // 시퀀스 애니메이션 생성

        for (int i = 0; i < message.Length; i++)
        {
            string currentText = message.Substring(0, i + 1); // i번째 글자까지 출력
            sequence.AppendCallback(() => _dialogueText.text = currentText);
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

            _dialogueText.text = _currentText;
        }
    }

    private void EndTyping()
    {
        _arrowImage.gameObject.SetActive(true);
    }
}
