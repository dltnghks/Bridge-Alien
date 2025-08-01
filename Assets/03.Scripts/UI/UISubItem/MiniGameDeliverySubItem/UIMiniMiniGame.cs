using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMiniMiniGame : UISubItem
{
    private enum Texts
    {
        TimeText,
        MiniGameInfoText,
    }

    private enum Objects
    {
        Board,
    }

    private enum Buttons
    {
        LeverButton
    }

    private enum Images
    {
        Gauge_Fill
    }

    private Image _gaugeFill;
    private RectTransform _leverRect;
    private Vector2 _centerPos;
    private TextMeshProUGUI _timeText;
    private TextMeshProUGUI _infoText;
    private GameObject _board;

    private float _accumulatedAngle = 0f;
    private float _maxAngle = 360f;
    private float _lastAngle = 0f;

    private float _maxTime = 10f;
    private float _elapsedTime = 0f;
    private bool _isDragging = false;
    private bool _isGameRunning = false;
    private Action<bool> _onGameEnd;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindObject(typeof(Objects));

        _leverRect = GetButton((int)Buttons.LeverButton).GetComponent<RectTransform>();
        _gaugeFill = GetImage((int)Images.Gauge_Fill);
        _timeText = GetText((int)Texts.TimeText);
        _infoText = GetText((int)Texts.MiniGameInfoText);
        _board = GetObject((int)Objects.Board);

        _centerPos = RectTransformUtility.WorldToScreenPoint(Camera.main, _leverRect.position);

        BindEvent(_leverRect.gameObject, OnPointerDown, Define.UIEvent.PointerDown);
        BindEvent(_leverRect.gameObject, OnDrag, Define.UIEvent.Drag);
        BindEvent(_leverRect.gameObject, OnPointerUp, Define.UIEvent.PointerUp);
        
        _board.SetActive(false);
        _infoText.gameObject.SetActive(false);

        // For Debug
        // StartMiniGame();

        return true;
    }

    public void Initialize(Action<bool> onGameEnd)
    {
        _onGameEnd = onGameEnd;
    }
    
    private Coroutine _startCoroutine;

    public void StartMiniGame()
    {
        if (_startCoroutine != null)
        {
            StopCoroutine(_startCoroutine);
        }
        
        Managers.Sound.PlaySFX(SoundType.MiniGameDeliverySFX, MiniGameDeliverySoundSFX.Minigame_Start.ToString());

        // 주변 Hurdle 날리기


        _startCoroutine = StartCoroutine(ShowStartSequence());
    }

    private IEnumerator ShowStartSequence()
    {
        _infoText.gameObject.SetActive(true);
        _infoText.text = "엔진 고장!";
        _infoText.color = Color.red;
        yield return new WaitForSeconds(2f);

        _infoText.text = "잠시 뒤 엔진 수리를 시도합니다.";
        yield return new WaitForSeconds(2f);

        _infoText.gameObject.SetActive(false);

        _elapsedTime = 0f;
        _accumulatedAngle = 0f;
        _isGameRunning = true;
        _board.SetActive(true);
    }

    private void Update()
    {
        if (!_isGameRunning)
            return;

        _elapsedTime += Time.deltaTime;
        float remainingTime = Mathf.Max(0f, _maxTime - _elapsedTime);
        _timeText.text = remainingTime.ToString("F1");

        if (remainingTime <= 0f)
        {
            EndMiniGame(false, "자가복구 시도중...", Color.red);
        }
    }

    private void OnPointerDown()
    {
        _isDragging = true;
        _lastAngle = GetCurrentAngle();
        Managers.Sound.PlaySFX(SoundType.MiniGameDeliverySFX, MiniGameDeliverySoundSFX.Minigame_Lever.ToString());
    }

    private void OnPointerUp()
    {
        _isDragging = false;
    }

    private void OnDrag()
    {
        if (!_isDragging || !_isGameRunning)
            return;

        float currentAngle = GetCurrentAngle();

        // 레버를 해당 각도로 바로 회전
        _leverRect.rotation = Quaternion.Euler(0f, 0f, currentAngle);

        // 시계 방향 회전일 때만 누적 (last - current가 양수면 시계 방향)
        float delta = Mathf.DeltaAngle(currentAngle, _lastAngle);
        if (delta > 0f)
        {
            _accumulatedAngle += delta;
            _accumulatedAngle = Mathf.Clamp(_accumulatedAngle, 0f, _maxAngle);
            UpdateGauge();
        }

        _lastAngle = currentAngle;

        if (_accumulatedAngle >= _maxAngle)
        {
            EndMiniGame(true, "엔진 수리 성공!", Color.blue);
        }
    }


    private float GetCurrentAngle()
    {
        Vector2 screenPos = Input.mousePosition;
        Vector2 dir = screenPos - _centerPos;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private void UpdateGauge()
    {
        float t = _accumulatedAngle / _maxAngle;
        _gaugeFill.fillAmount = t;
    }

    private void RotateLever(float angle)
    {
        _leverRect.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void EndMiniGame(bool isSuccess, string message, Color textColor)
    {
        _isGameRunning = false;
        _infoText.gameObject.SetActive(true);
        _infoText.text = message;
        _infoText.color = textColor;
        _timeText.text = "";

        Managers.Sound.PlaySFX(SoundType.MiniGameDeliverySFX, MiniGameDeliverySoundSFX.Minigame_Success.ToString());

        StartCoroutine(FinishSequence(isSuccess));
    }

    private IEnumerator FinishSequence(bool isSuccess)
    {
        _board.SetActive(false);
        yield return new WaitForSeconds(2f);
        _infoText.gameObject.SetActive(false);
        _onGameEnd?.Invoke(isSuccess);
        
        // 게임 재시작
        Managers.MiniGame.CurrentGame.ResumeGame();
        (Managers.MiniGame.CurrentGame.PlayerCharacter as MiniGameDeliveryPlayer)?.OnMove(1f);
    }
}
