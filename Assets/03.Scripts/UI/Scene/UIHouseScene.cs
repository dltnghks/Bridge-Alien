using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIHouseScene : UIScene
{
    public static bool BlockWorldInputThisFrame { get; private set; }
    private const string HousePlayerObjectName = "HousePlayer";
    private const float FatigueRecoverEffectDuration = 0.65f;
    private const float FatigueRecoverEffectSpawnInterval = 0.18f;
    private const float FatigueRecoverEffectStartScale = 1.25f;
    private const float FatigueRecoverEffectEndScale = 0.55f;
    private const int FatigueRecoverEffectSortingOrder = 1000;
    private const int GoldGainPerEffect = 500;
    private const float GoldGainEffectDuration = 0.65f;
    private const float GoldGainEffectSpawnInterval = 0.12f;
    private const float GoldGainEffectStartScale = 1.25f;
    private const float GoldGainEffectEndScale = 0.55f;
    private const int GoldGainEffectSortingOrder = 1000;

    enum Texts{
        GoldText,
    }

    enum Images
    {
        GoldIcon,
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
    private Transform _housePlayerTransform;
    private Image _goldIconImage;
    private Vector3 _goldIconDefaultScale = Vector3.one;
    private Canvas _canvas;
    private int _displayedGold = -1;
    private int _goldEffectTargetGold = -1;
    private int _displayedFatigue = -1;
    [SerializeField] private Sprite[] _timeImages;

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

        _rectTransform = transform as RectTransform;
        _canvas = GetComponentInParent<Canvas>();
        _goldUI = GetObject((int)Objects.UIGold).transform as RectTransform;
        _fatigueUI = GetObject((int)Objects.UIFatigue).transform as RectTransform;
        _housePlayerTransform = GameObject.Find(HousePlayerObjectName)?.transform;
        _goldIconImage = GetImage((int)Images.GoldIcon);
        _goldIconDefaultScale = _goldIconImage != null ? _goldIconImage.rectTransform.localScale : Vector3.one;
        _fatigueIconGroup = GetObject((int)Objects.FatigueIconGroup).GetOrAddComponent<UIFatigueIconGroup>();
        MoveStatusUIToSceneRoot();
        
        GetButton((int)Buttons.PlayerStatusButton).gameObject.BindEvent(OnClickPlayerStatusButton);
        GetButton((int)Buttons.TaskButton).gameObject.BindEvent(OnClickTaskButton);
        GetButton((int)Buttons.WorkModuleButton).gameObject.BindEvent(OnClickWorkModuleButton);
        GetButton((int)Buttons.UINextButton).gameObject.BindEvent(OnClickNextButton);

        SetGoldText();
        SetFatigue();
        PlayPendingFatigueRecoverEffects();
        StartCoroutine(OpenPendingStagePopupNextFrame());
        
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
        OpenStagePopup();
    }

    private void OpenPendingStagePopup()
    {
        if (Managers.Stage.ConsumeShouldOpenStagePopupOnHouse() == false)
        {
            return;
        }

        OpenStagePopup();
    }

    private IEnumerator OpenPendingStagePopupNextFrame()
    {
        yield return null;
        OpenPendingStagePopup();
    }

    private void OpenStagePopup()
    {
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
        int curGold = Managers.Player.GetGold();

        if (_displayedGold < 0)
        {
            _goldEffectTargetGold = curGold;
            SetDisplayedGold(curGold);
            return;
        }

        if (curGold <= _displayedGold)
        {
            _goldEffectTargetGold = curGold;
            SetDisplayedGold(curGold);
            return;
        }

        int animationBaseGold = Mathf.Max(_displayedGold, _goldEffectTargetGold);
        if (curGold <= animationBaseGold)
        {
            _goldEffectTargetGold = curGold;
            return;
        }

        _goldEffectTargetGold = curGold;
        StartCoroutine(PlayGoldGainEffects(curGold - animationBaseGold));
    }

    private void SetDisplayedGold(int gold)
    {
        _displayedGold = gold;
        GetText((int)Texts.GoldText).text = $"{_displayedGold}N";
    }

    private IEnumerator PlayGoldGainEffects(int gainedGold)
    {
        int remainingGold = gainedGold;
        while (remainingGold > 0)
        {
            int effectGold = Mathf.Min(GoldGainPerEffect, remainingGold);
            PlayGoldGainEffect(effectGold);
            remainingGold -= effectGold;
            yield return new WaitForSeconds(GoldGainEffectSpawnInterval);
        }
    }

    private void PlayGoldGainEffect(int effectGold)
    {
        if (_goldIconImage == null || _rectTransform == null)
        {
            return;
        }

        RectTransform targetRectTransform = _goldIconImage.rectTransform;
        GameObject effectObject = new GameObject("UIGoldGainEffect", typeof(RectTransform), typeof(Canvas), typeof(CanvasRenderer), typeof(Image));
        effectObject.transform.SetParent(transform, false);
        effectObject.transform.SetAsLastSibling();

        Canvas effectCanvas = effectObject.GetComponent<Canvas>();
        effectCanvas.overrideSorting = true;
        effectCanvas.sortingOrder = GoldGainEffectSortingOrder;

        Image effectImage = effectObject.GetComponent<Image>();
        effectImage.sprite = _goldIconImage.sprite;
        effectImage.preserveAspect = true;
        effectImage.raycastTarget = false;

        RectTransform effectRectTransform = effectObject.GetComponent<RectTransform>();
        effectRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        effectRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        effectRectTransform.pivot = new Vector2(0.5f, 0.5f);
        effectRectTransform.anchoredPosition = GetEffectStartPosition();
        effectRectTransform.sizeDelta = targetRectTransform.rect.size;
        effectRectTransform.localScale = Vector3.one * GoldGainEffectStartScale;

        Vector2 targetPosition = GetAnchoredPosition(targetRectTransform);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(effectRectTransform.DOAnchorPos(targetPosition, GoldGainEffectDuration).SetEase(Ease.InOutCubic));
        sequence.Join(effectRectTransform.DOScale(GoldGainEffectEndScale, GoldGainEffectDuration).SetEase(Ease.InQuad));
        sequence.Join(effectImage.DOFade(0.0f, 0.18f).SetDelay(GoldGainEffectDuration - 0.18f));
        sequence.OnComplete(() =>
        {
            if (_goldEffectTargetGold > _displayedGold)
            {
                SetDisplayedGold(Mathf.Min(_displayedGold + effectGold, _goldEffectTargetGold));
            }

            targetRectTransform.DOKill();
            targetRectTransform.localScale = _goldIconDefaultScale;
            targetRectTransform
                .DOPunchScale(Vector3.one * 0.2f, 0.25f, 6, 0.7f)
                .OnComplete(() => targetRectTransform.localScale = _goldIconDefaultScale);
            Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.PlusScore.ToString());
            Destroy(effectObject);
        });
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
        effectRectTransform.anchoredPosition = GetEffectStartPosition();
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
            Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.PlusScore.ToString());
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

    private Vector2 GetEffectStartPosition()
    {
        return _housePlayerTransform != null
            ? GetWorldAnchoredPosition(_housePlayerTransform.position)
            : Vector2.zero;
    }

    private Vector2 GetWorldAnchoredPosition(Vector3 worldPosition)
    {
        Camera worldCamera = Camera.main;
        if (worldCamera == null)
        {
            return Vector2.zero;
        }

        Vector2 screenPoint = worldCamera.WorldToScreenPoint(worldPosition);
        Camera uiCamera = null;
        if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = _canvas.worldCamera;
        }

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
