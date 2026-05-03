using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIHouseScene : UIScene
{
    public static bool BlockWorldInputThisFrame { get; private set; }
    private const float FatigueRecoverEffectDuration = 0.65f;
    private const float FatigueRecoverEffectSpawnInterval = 0.18f;
    private const float FatigueRecoverEffectStartScale = 1.25f;
    private const float FatigueRecoverEffectEndScale = 0.55f;
    private const int FatigueRecoverEffectSortingOrder = 1000;

    enum Texts{
        GoldText,
    }

    enum Buttons
    {
        PlayerStatusButton,
        TaskButton,
        WorkModuleButton,
        UINextButton,
    }

    enum Objects
    {
        UIGold,
        UIFatigue,
        FatigueIconGroup
    }

    private UIPopup _currentPopup = null;
    private UIFatigueIconGroup _fatigueIconGroup;
    private RectTransform _rectTransform;
    private RectTransform _goldUI;
    private RectTransform _fatigueUI;
    private Canvas _canvas;
    private int _displayedFatigue = -1;
    [SerializeField] private Sprite[] _timeImages;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindObject(typeof(Objects));

        _rectTransform = transform as RectTransform;
        _canvas = GetComponentInParent<Canvas>();
        _goldUI = GetObject((int)Objects.UIGold).transform as RectTransform;
        _fatigueUI = GetObject((int)Objects.UIFatigue).transform as RectTransform;
        _fatigueIconGroup = GetObject((int)Objects.FatigueIconGroup).GetOrAddComponent<UIFatigueIconGroup>();
        MoveStatusUIToSceneRoot();
        
        GetButton((int)Buttons.PlayerStatusButton).gameObject.BindEvent(OnClickPlayerStatusButton);
        GetButton((int)Buttons.TaskButton).gameObject.BindEvent(OnClickTaskButton);
        GetButton((int)Buttons.WorkModuleButton).gameObject.BindEvent(OnClickWorkModuleButton);
        GetButton((int)Buttons.UINextButton).gameObject.BindEvent(OnClickNextButton);

        SetGoldText();
        SetFatigue();
        PlayPendingFatigueRecoverEffects();

        return true;
    }

    public override void UIUpdate()
    {
        base.UIUpdate();
    }

    private void OnClickPlayerStatusButton()
    {
        if (_currentPopup)
        {
            _currentPopup.ClosePopupUI();
            return;
        }
        
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        _currentPopup = Managers.UI.ShowPopUI<UIPlayerStatusPopup>();
        PlaceStatusUIBehindPopup(_currentPopup);
    }

    private void OnClickTaskButton()
    {
        // 일과 팝업 생성
        if (_currentPopup)
        {
            _currentPopup.ClosePopupUI();
            return;
        }

        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        UIPlayerTaskPopup taskPopup = Managers.UI.ShowPopUI<UIPlayerTaskPopup>("UIPlayerTaskPopup", transform);
        PlaceStatusUIBehindPopup(taskPopup);
    }


    private void OnClickWorkModuleButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        _currentPopup = Managers.UI.ShowPopUI<UIWorkModulePopup>();
        PlaceStatusUIBehindPopup(_currentPopup);
    }

    private void OnClickNextButton()
    {
        BlockWorldInputThisFrame = true;
        StartCoroutine(ResetWorldInputBlock());
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        var stagePopup = Managers.UI.ShowPopUI<UIStagePopup>();
        PlaceStatusUIBehindPopup(stagePopup);
        stagePopup.InitStageButtonGroup();
    }

    private void MoveStatusUIToSceneRoot()
    {
        MoveToSceneRoot(_goldUI);
        MoveToSceneRoot(_fatigueUI);
    }

    private void MoveToSceneRoot(RectTransform target)
    {
        if (target == null || target.parent == transform)
        {
            return;
        }

        target.SetParent(transform, false);
    }

    private void PlaceStatusUIBehindPopup(UIPopup popup)
    {
        if (popup == null || popup.transform.parent != transform)
        {
            return;
        }

        PlaceBeforePopup(_goldUI, popup.transform);
        PlaceBeforePopup(_fatigueUI, popup.transform);
    }

    private void PlaceBeforePopup(RectTransform target, Transform popupTransform)
    {
        if (target == null || target.parent != transform)
        {
            return;
        }

        int popupIndex = popupTransform.GetSiblingIndex();
        target.SetSiblingIndex(Mathf.Max(0, popupIndex - 1));
    }
    
    private void SetGoldText()
    {
        GetText((int)Texts.GoldText).text = $"{Managers.Player.GetGold()}N";
    }
    
    private void SetFatigue()
    {
        int curFatigue = Managers.Player.GetStats(Define.PlayerStatsType.Fatigue);
        int previousFatigue = _displayedFatigue;

        _fatigueIconGroup.SetFatigue(curFatigue);
        _displayedFatigue = curFatigue;

        if (previousFatigue >= 0 && curFatigue > previousFatigue)
        {
            int recoveredFatigue = curFatigue - previousFatigue;
            Managers.Player.ConsumePendingFatigueRecoverEffectCount(recoveredFatigue);
            StartCoroutine(PlayFatigueRecoverEffects(previousFatigue, curFatigue));
        }
    }

    private void PlayPendingFatigueRecoverEffects()
    {
        int currentFatigue = Managers.Player.GetStats(Define.PlayerStatsType.Fatigue);
        int pendingRecoverCount = Managers.Player.ConsumePendingFatigueRecoverEffectCount(currentFatigue);
        if (pendingRecoverCount <= 0)
        {
            return;
        }

        int previousFatigue = Mathf.Max(0, currentFatigue - pendingRecoverCount);
        StartCoroutine(PlayFatigueRecoverEffects(previousFatigue, currentFatigue));
    }

    private IEnumerator PlayFatigueRecoverEffects(int previousFatigue, int currentFatigue)
    {
        int startIndex = Mathf.Clamp(previousFatigue, 0, PlayerManager.FatigueMaxValue - 1);
        int endIndex = Mathf.Clamp(currentFatigue - 1, 0, PlayerManager.FatigueMaxValue - 1);

        for (int i = startIndex; i <= endIndex; i++)
        {
            PlayFatigueRecoverEffect(i);
            yield return new WaitForSeconds(FatigueRecoverEffectSpawnInterval);
        }
    }

    private void PlayFatigueRecoverEffect(int targetIconIndex)
    {
        Image targetIconImage = _fatigueIconGroup.GetFatigueIconImage(targetIconIndex);
        if (targetIconImage == null || _rectTransform == null)
        {
            return;
        }

        RectTransform targetRectTransform = targetIconImage.rectTransform;
        GameObject effectObject = new GameObject("UIFatigueRecoverEffect", typeof(RectTransform), typeof(Canvas), typeof(CanvasRenderer), typeof(Image));
        effectObject.transform.SetParent(transform, false);
        effectObject.transform.SetAsLastSibling();

        Canvas effectCanvas = effectObject.GetComponent<Canvas>();
        effectCanvas.overrideSorting = true;
        effectCanvas.sortingOrder = FatigueRecoverEffectSortingOrder;

        Image effectImage = effectObject.GetComponent<Image>();
        effectImage.sprite = targetIconImage.sprite;
        effectImage.preserveAspect = true;
        effectImage.raycastTarget = false;

        RectTransform effectRectTransform = effectObject.GetComponent<RectTransform>();
        effectRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        effectRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        effectRectTransform.pivot = new Vector2(0.5f, 0.5f);
        effectRectTransform.anchoredPosition = Vector2.zero;
        effectRectTransform.sizeDelta = targetRectTransform.rect.size;
        effectRectTransform.localScale = Vector3.one * FatigueRecoverEffectStartScale;

        Vector2 targetPosition = GetAnchoredPosition(targetRectTransform);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(effectRectTransform.DOAnchorPos(targetPosition, FatigueRecoverEffectDuration).SetEase(Ease.InOutCubic));
        sequence.Join(effectRectTransform.DOScale(FatigueRecoverEffectEndScale, FatigueRecoverEffectDuration).SetEase(Ease.InQuad));
        sequence.Join(effectImage.DOFade(0.0f, 0.18f).SetDelay(FatigueRecoverEffectDuration - 0.18f));
        sequence.OnComplete(() =>
        {
            targetRectTransform.DOKill();
            targetRectTransform.DOPunchScale(Vector3.one * 0.2f, 0.25f, 6, 0.7f);
            Destroy(effectObject);
        });
    }

    private Vector2 GetAnchoredPosition(RectTransform target)
    {
        Camera uiCamera = null;
        if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = _canvas.worldCamera;
        }

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, screenPoint, uiCamera, out Vector2 localPoint);
        return localPoint;
    }

    private void OnEnable()
    {
        Managers.Player.OnPlayerDataChanged += SetGoldText;
        Managers.Player.OnPlayerDataChanged += SetFatigue;
    }
    
    private void OnDisable()
    {
        Managers.Player.OnPlayerDataChanged -= SetGoldText;
        Managers.Player.OnPlayerDataChanged -= SetFatigue;
    }

    private IEnumerator ResetWorldInputBlock()
    {
        yield return null;
        BlockWorldInputThisFrame = false;
    }
}
