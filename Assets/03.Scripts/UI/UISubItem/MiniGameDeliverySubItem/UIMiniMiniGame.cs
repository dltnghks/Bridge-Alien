using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class UIMiniMiniGame : UIPopup
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
    private float _maxAngle = 360f * 10f;
    private float _lastAngle = 0f;

    private float _maxTime = 10f;
    private float _elapsedTime = 0f;
    private bool _isDragging = false;
    private bool _isGameRunning = false;

    private Coroutine _startCoroutine;
    public UnityEvent<bool> onRepairEvent;

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

        // UI 월드 좌표로 센터 포지션 설정
        _centerPos = _leverRect.position;

        BindEvent(_leverRect.gameObject, OnPointerDown, Define.UIEvent.PointerDown);
        BindEvent(_leverRect.gameObject, OnDrag, Define.UIEvent.Drag);
        BindEvent(_leverRect.gameObject, OnPointerUp, Define.UIEvent.PointerUp);
        
        _board.SetActive(false);
        _infoText.gameObject.SetActive(false);

        StartMiniGame();
        
        return true;
    }

    public void StartMiniGame()
    {
        if (_startCoroutine != null)
        {
            StopCoroutine(_startCoroutine);
        }
        
        Managers.Sound.PlaySFX(SoundType.MiniGameDeliverySFX, MiniGameDeliverySoundSFX.Minigame_Start.ToString());

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
        if (!_isGameRunning)
            return;

        _isDragging = true;

        // Screen Space - Overlay에서는 카메라가 필요 없음
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _leverRect.parent as RectTransform, 
            Input.mousePosition, 
            null,  // Overlay 모드에서는 null 사용
            out mousePos
        );

        Vector2 leverLocalPos = _leverRect.localPosition;
        Vector2 direction = mousePos - leverLocalPos;
        _lastAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Debug.Log($"Initial angle: {_lastAngle}, Mouse: {mousePos}, Lever: {leverLocalPos}, Direction: {direction}");
        
        Managers.Sound.PlaySFX(SoundType.MiniGameDeliverySFX, MiniGameDeliverySoundSFX.Minigame_Lever.ToString());
    }

    private void OnPointerUp()
    {
        _isDragging = false;
    }

    private void OnDrag()
    {
        if (!_isGameRunning || !_isDragging)
            return;

        // Screen Space - Overlay에서는 카메라가 필요 없음
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _leverRect.parent as RectTransform, 
            Input.mousePosition, 
            null,  // Overlay 모드에서는 null 사용
            out mousePos
        );

        // 레버 중심점을 기준으로 한 방향 벡터 계산
        Vector2 leverLocalPos = _leverRect.localPosition;
        Vector2 direction = mousePos - leverLocalPos;
        
        // 현재 각도 계산
        float currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // UI 회전 적용 (기본적으로 오른쪽이 0도)
        _leverRect.rotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);

        // 누적 회전량 계산 (각도 차이를 절댓값으로)
        float deltaAngle = Mathf.DeltaAngle(_lastAngle, currentAngle);
        _accumulatedAngle += Mathf.Abs(deltaAngle);
        _lastAngle = currentAngle;

        UpdateGauge();

        if (_accumulatedAngle >= _maxAngle)
        {
            EndMiniGame(true, "수리에 성공했습니다!", Color.green);
        }
    }

    private void UpdateGauge()
    {
        float t = _accumulatedAngle / _maxAngle;
        _gaugeFill.fillAmount = t;
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
        
        onRepairEvent?.Invoke(isSuccess);
        
        // 게임 재시작
        Managers.MiniGame.CurrentGame.ResumeGame();
        (Managers.MiniGame.CurrentGame.PlayerCharacter as MiniGameDeliveryPlayer)?.OnMove(1f);
        
        onRepairEvent = null;
        Managers.UI.ClosePopupUI();
    }
}