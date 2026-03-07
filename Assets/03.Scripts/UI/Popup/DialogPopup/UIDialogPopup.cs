using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogPopup : UIPopup
{
    enum Texts
    {
        NameText,
        DialogText,
    }

    enum Images
    {
        Background,
        NameBackground,
        LeftCharacterImage,
        RightCharacterImage,
        CenterCharacterImage,
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

    public float TypingSpeed = 0.01f;

    [Header("Dialog Background")]
    [SerializeField] private SerializedDictionary<Define.DialogSceneType, Sprite> _backgroundImage = new SerializedDictionary<Define.DialogSceneType, Sprite>();

    [Header("Speaker Character Images")]
    [SerializeField] private SerializedDictionary<Define.DialogSpeakerType, Sprite> _speakerCharacterImages = new SerializedDictionary<Define.DialogSpeakerType, Sprite>();

    private TextMeshProUGUI _dialogText;
    private Button _nextButton;
    private VerticalLayoutGroup _choiceGroup;
    private List<UIChoiceButton> _choiceButtons = new List<UIChoiceButton>();

    private readonly Dictionary<string, DialogData> _dialogsDict = new Dictionary<string, DialogData>();
    private readonly Dictionary<string, List<DialogData>> _choiceDict = new Dictionary<string, List<DialogData>>();
    private readonly List<DialogData> _currentChoices = new List<DialogData>();

    private DialogData _currentDialog;
    private Define.DialogSpeakerType _leftSpeakerType;
    private Define.DialogSpeakerType _rightSpeakerType;
    private Define.DialogSpeakerType _centerSpeakerType;

    private Tween _typingTween;
    private Color _nameBackgroundDefaultColor = Color.white;

    private Action _callback;
    private SceneBGM? _previousSceneBGM;
    private bool _shouldRestoreSceneBGM;
    private bool _isFinish;

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
        _nameBackgroundDefaultColor = GetImage((int)Images.NameBackground).color;

        GetButton((int)Buttons.NextButton).gameObject.BindEvent(OnClickNextButton);
        GetButton((int)Buttons.SkipButton).gameObject.BindEvent(OnClickSkipButton);

        _nextButton = GetButton((int)Buttons.NextButton);
        _nextButton.gameObject.SetActive(false);

        _choiceGroup = GetObject((int)Objects.UIChoiceGroup).GetOrAddComponent<VerticalLayoutGroup>();
        _choiceButtons = _choiceGroup.GetComponentsInChildren<UIChoiceButton>().ToList();

        return true;
    }

    private void OnClickSkipButton()
    {
        Logger.Log("OnClickSkipButton");
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        EndDialog();
    }

    private void OnClickNextButton()
    {
        if (_isFinish || SkipTyping())
        {
            return;
        }

        Logger.Log("OnClickNextButton");
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
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
            _currentChoices.Clear();
            _currentChoices.AddRange(_choiceDict[_currentDialog.NextDialogID]);
            ShowChoices();
        }
    }

    public void InitDialog(Define.Dialog dialogue, Define.DialogSceneType sceneType, Action callback = null)
    {
        Init();

        Logger.Log($"SetDialogs : {dialogue}");

        _isFinish = false;
        _callback = callback;
        _previousSceneBGM = Managers.Sound.CurrentBGMType == SoundType.SceneBGM
            ? Managers.Sound.CurrentSceneBGM
            : null;
        _shouldRestoreSceneBGM = false;

        SetDialogData(dialogue);
        if (_currentDialog == null)
        {
            return;
        }

        ApplyDialogBGM(dialogue);
        SetBackground(sceneType);

        SetSpeakerImage(Define.DialogSpeakerType.UNKNOWN, Define.DialogSpeakerPosType.Left);
        SetSpeakerImage(Define.DialogSpeakerType.UNKNOWN, Define.DialogSpeakerPosType.Right);
        SetSpeakerImage(Define.DialogSpeakerType.UNKNOWN, Define.DialogSpeakerPosType.Center);

        GetImage((int)Images.LeftCharacterImage).color = new Color(1, 1, 1, 0);
        GetImage((int)Images.RightCharacterImage).color = new Color(1, 1, 1, 0);
        GetImage((int)Images.CenterCharacterImage).color = new Color(1, 1, 1, 0);

        foreach (var choiceButton in _choiceButtons)
        {
            choiceButton.gameObject.SetActive(false);
        }

        UpdateDialog();
    }

    private void SetDialogData(Define.Dialog dialogue)
    {
        List<DialogData> currentDialogs = Managers.Data.DialogData.GetData(dialogue);

        _dialogsDict.Clear();
        _choiceDict.Clear();
        _currentChoices.Clear();

        if (currentDialogs == null || currentDialogs.Count == 0)
        {
            _currentDialog = null;
            Logger.LogError("Dialogs not found");
            EndDialog();
            return;
        }

        foreach (var dialog in currentDialogs)
        {
            if (dialog.Type == Define.DialogType.Choice)
            {
                if (_choiceDict.ContainsKey(dialog.DialogID) == false)
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

        _currentDialog = currentDialogs[0];
    }

    private void ApplyDialogBGM(Define.Dialog dialogue)
    {
        DialogBGM dialogBGM = Managers.Data.DialogData.GetBGM(dialogue);

        Logger.Log($"CurrentDialogBGM : {Managers.Sound.CurrentDialogBGM}, dialogBGM : {dialogBGM}");
        
        if (Managers.Sound.CurrentBGMType == SoundType.DialogBGM &&
            Managers.Sound.CurrentDialogBGM == dialogBGM)
        {
            return;
        }

        _shouldRestoreSceneBGM = _previousSceneBGM.HasValue;
        Managers.Sound.PlayDialogBGM(dialogBGM);
    }

    private void SetBackground(Define.DialogSceneType sceneType)
    {
        Image backgroundImage = GetImage((int)Images.Background);
        if (sceneType == Define.DialogSceneType.Unknown)
        {
            backgroundImage.sprite = null;
            backgroundImage.color = Color.clear;
        }
        else if (_backgroundImage.ContainsKey(sceneType))
        {
            backgroundImage.sprite = _backgroundImage[sceneType];
            backgroundImage.color = Color.white;
        }
        else
        {
            Logger.LogError($"Background image for {sceneType} not found");
        }
    }

    private void UpdateDialog()
    {
        if (_currentDialog.Type == Define.DialogType.End)
        {
            EndDialog();
            return;
        }

        string characterName = _currentDialog.CharacterName;
        string dialogText = _currentDialog.Script;

        switch (_currentDialog.Type)
        {
            case Define.DialogType.Dialog:
            case Define.DialogType.Monolog:
                SetSpeakerImage(_currentDialog.SpeakerType, _currentDialog.SpeakerPosType);
                SetNameText(characterName, _currentDialog.Type);
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

    private void ShowChoices()
    {
        _nextButton.gameObject.SetActive(false);

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
                GameObject newChoiceButton = Managers.Resource.Instantiate(_choiceButtons[0].gameObject, _choiceGroup.transform);
                _choiceButtons.Add(newChoiceButton.GetOrAddComponent<UIChoiceButton>());
            }

            UIChoiceButton choiceButton = _choiceButtons[index];
            choiceButton.gameObject.SetActive(true);
            choiceButton.SetChoiceButton(choiceData, OnClickChoiceButton, index);
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

    private void SetSpeakerImage(Define.DialogSpeakerType speakerType, Define.DialogSpeakerPosType speakerPosType)
    {
        if (speakerPosType == Define.DialogSpeakerPosType.Left)
        {
            _leftSpeakerType = speakerType;
        }
        else if (speakerPosType == Define.DialogSpeakerPosType.Right)
        {
            _rightSpeakerType = speakerType;
        }
        else if (speakerPosType == Define.DialogSpeakerPosType.Center)
        {
            _centerSpeakerType = speakerType;
        }
        else
        {
            _leftSpeakerType = Define.DialogSpeakerType.UNKNOWN;
            _rightSpeakerType = Define.DialogSpeakerType.UNKNOWN;
            _centerSpeakerType = Define.DialogSpeakerType.UNKNOWN;
        }

        Image leftImage = GetImage((int)Images.LeftCharacterImage);
        Image rightImage = GetImage((int)Images.RightCharacterImage);
        Image centerImage = GetImage((int)Images.CenterCharacterImage);

        leftImage.color = Color.HSVToRGB(0, 0, 0.3f);
        rightImage.color = Color.HSVToRGB(0, 0, 0.3f);
        centerImage.color = Color.HSVToRGB(0, 0, 0.3f);

        if (speakerPosType == Define.DialogSpeakerPosType.Left)
        {
            leftImage.sprite = _speakerCharacterImages[speakerType];
            leftImage.color = Color.white;
        }
        else if (speakerPosType == Define.DialogSpeakerPosType.Right)
        {
            rightImage.sprite = _speakerCharacterImages[speakerType];
            rightImage.color = Color.white;
        }
        else if (speakerPosType == Define.DialogSpeakerPosType.Center)
        {
            centerImage.sprite = _speakerCharacterImages[speakerType];
            centerImage.color = Color.white;
        }

        if (_leftSpeakerType == Define.DialogSpeakerType.UNKNOWN)
            leftImage.color = Color.clear;
        if (_rightSpeakerType == Define.DialogSpeakerType.UNKNOWN)
            rightImage.color = Color.clear;
        if (_centerSpeakerType == Define.DialogSpeakerType.UNKNOWN)
            centerImage.color = Color.clear;
    }

    private void SetNameText(string characterName, Define.DialogType dialogType)
    {
        GetText((int)Texts.NameText).SetText(characterName);
        bool hideNameBackground = string.IsNullOrWhiteSpace(characterName);

        Image nameBackgroundImage = GetImage((int)Images.NameBackground);
        nameBackgroundImage.color = hideNameBackground
            ? new Color(_nameBackgroundDefaultColor.r, _nameBackgroundDefaultColor.g, _nameBackgroundDefaultColor.b, 0f)
            : _nameBackgroundDefaultColor;
    }

    public void StartTyping(string message)
    {
        if (_typingTween != null)
        {
            _typingTween.Kill();
        }

        _nextButton.gameObject.SetActive(false);
        _dialogText.text = string.Empty;

        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < message.Length; i++)
        {
            string currentText = message.Substring(0, i + 1);
            sequence.AppendCallback(() => _dialogText.text = currentText);
            sequence.AppendInterval(TypingSpeed);
        }

        _typingTween = sequence;
        _typingTween.OnComplete(EndTyping);
    }

    public bool SkipTyping()
    {
        Logger.Log("SkipTyping");

        if (_typingTween != null && _typingTween.IsActive())
        {
            _typingTween.Complete();
            _dialogText.text = _currentDialog.Script;
            return true;
        }

        return false;
    }

    private void EndTyping()
    {
        _nextButton.gameObject.SetActive(true);
    }

    private void EndDialog()
    {
        _isFinish = true;
        Logger.Log("EndDialog");

        if (_shouldRestoreSceneBGM && _previousSceneBGM.HasValue)
        {
            Managers.Sound.PlaySceneBGM(_previousSceneBGM.Value);
        }

        _shouldRestoreSceneBGM = false;
        _previousSceneBGM = null;
        _callback?.Invoke();
    }
}
