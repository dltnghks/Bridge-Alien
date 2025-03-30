using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    enum Objects
    {
        UIChoiceGroup,
    }
    
    public float TypingSpeed = 0.01f; // 글자 타이핑 속도
    
    private List<Image> _speakerCharacterImages = new List<Image>();
    private TextMeshProUGUI _dialogText;  
    private Button _nextButton;
    private VerticalLayoutGroup _choiceGroup;
    private List<UIChoiceButton> _choiceButtons = new List<UIChoiceButton>();

    private Dictionary<string, DialogData> _dialogsDict = new Dictionary<string, DialogData>();
    private Dictionary<string, List<DialogData>> _choiceDict = new Dictionary<string, List<DialogData>>();
    private DialogData _currentDialog;
    private List<DialogData> _currentChoices = new List<DialogData>();
    
    private Tween _typingTween; // 현재 실행 중인 트윈
    
    private Action _callback;
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindObject(typeof(Objects));

        _dialogText = GetText((int)Texts.DialogText);
        
        _speakerCharacterImages.Add(GetImage((int)Images.LeftCharacterImage));
        _speakerCharacterImages.Add(GetImage((int)Images.RightCharacterImage));
        
        GetButton((int)Buttons.SkipButton).gameObject.BindEvent(OnClickScreenButton);
        GetButton((int)Buttons.NextButton).gameObject.BindEvent(OnClickNextButton);
        
        _nextButton = GetButton((int)Buttons.NextButton);
        _nextButton.gameObject.SetActive(false);

        _choiceGroup = GetObject((int)Objects.UIChoiceGroup).GetOrAddComponent<VerticalLayoutGroup>();
        _choiceButtons = _choiceGroup.GetComponentsInChildren<UIChoiceButton>().ToList();
        
        foreach (var choiceButton in _choiceButtons)
        {
            choiceButton.gameObject.SetActive(false);
        }
        
        
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
        
        NextDialog();
    }
    
    private void OnClickChoiceButton(string nextDialogID)
    {
        ClearChoices();
        NextDialog(nextDialogID);
    }
    
    private void NextDialog(string nextDialogID = null)
    {
        Logger.Log("NextDialog");

        if (nextDialogID == null && _dialogsDict.ContainsKey(_currentDialog.NextDialogID))
        {
            nextDialogID = _currentDialog.NextDialogID;
        }
        
        if (nextDialogID != null)
        {
            Logger.Log($"{nextDialogID}");
            _currentDialog = _dialogsDict[nextDialogID];
            UpdateDialog();
        }
        else
        {
            Logger.Log("NextDialog Not Found");
            _currentChoices = _choiceDict[_currentDialog.NextDialogID];
            ShowChoices();
        }
    }
    
    public void SetDialogs(Define.Dialog dialogue, Action callback = null)
    {
        Init();
        
        Logger.Log($"SetDialogs : {dialogue}");   
        
        _callback = callback; 
        List<DialogData> currentDialogs = Managers.Data.DialogData.GetData(Define.Dialog.TUTORIAL_STORY_01);
        
        foreach (var dialog in currentDialogs)
        {
            if (dialog.Type == Define.DialogType.Choice)
            {
                if (!_choiceDict.ContainsKey(dialog.DialogID))
                {
                    _choiceDict[dialog.DialogID] = new List<DialogData>();
                }
                _choiceDict[dialog.DialogID].Add(dialog);
            }
            else
            {
                _dialogsDict[dialog.DialogID] = dialog;
            }
        }
        

        if (currentDialogs.Count == 0)
        {
            Logger.LogError("Dialogs not found");
            EndDialog();
            return;
        }
        
        _currentDialog = currentDialogs[0];
        
        UpdateDialog();
    }
    
    private void UpdateDialog()
    {   
        if (_currentDialog.Type == Define.DialogType.End)
        {
            // 대화 종료
            EndDialog();
            return;
        }

        switch (_currentDialog.Type)
        {
            case Define.DialogType.Dialog :
                string characterName = _currentDialog.CharacterName;
                string dialogText = _currentDialog.Script;
        
                // 이름 변경
                SetNameText(characterName);
                // 대화 변경
                StartTyping(dialogText);
                break;
            case Define.DialogType.Unknown:
                Logger.Log("Unknown");
                break;
            default:
                Logger.LogError("Unknown dialog type");
                break;
        }
    }
    
    private void UpdateDialog(string dialogID)
    {
        
    }
    
    private void ShowChoices()
    {
        _nextButton.gameObject.SetActive(false);    // 화살표 끄기
        
        if (_currentChoices.Count == 0)
        {
            Logger.LogError("No choices found");
            return;
        }

        int index = 0;
        foreach (var choiceData in _currentChoices)
        {
            if (index >= _choiceButtons.Count)
            {
                var newChoiceButton = Managers.Resource.Instantiate(_choiceButtons[0].gameObject, _choiceGroup.transform);
                _choiceButtons.Add(newChoiceButton.GetOrAddComponent<UIChoiceButton>());
            }
            
            
            var choiceButton = _choiceButtons[index];
            choiceButton.gameObject.SetActive(true);
            choiceButton.SetChoiceButton(choiceData, OnClickChoiceButton);
            index++;
        }
    }

    private void ClearChoices()
    {
        foreach (var choiceButton in _choiceButtons)
        {
            choiceButton.gameObject.SetActive(false);
        }
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

            _dialogText.text = _currentDialog.Script;
        }
    }

    private void EndTyping()
    {
        _nextButton.gameObject.SetActive(true);
    }

    private void EndDialog()
    {
        Logger.Log("EndDialog");
        _callback?.Invoke();
        ClosePopupUI();
    }
}
