using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameDeliveryPathProgress
{
    private UIPathProgressBar _uiPathProgressBar;
    private UnityAction _endAction;
    
    // 현재 ProgressBar가 활성화 되어 있는지 확인하는 변수
    private bool _isActive;
    
    // 시간에 관련된 변수
    private float _currentRatio = .0f;
    private const float EndRatio = 1f;

    private Transform _target;
    private Vector3 _endPosition;

    public void Initialize(UIPathProgressBar uIPathProgressBar, Transform target, Vector3 endPosition, UnityAction endAction = null)
    {
        if (_isActive) return;
        _isActive = true;

        _uiPathProgressBar = uIPathProgressBar;
        _target = target;
        _endAction = endAction;
        _endPosition = endPosition;
        
        _currentRatio = target.position.x / endPosition.x;
        
        _uiPathProgressBar.Init();
    }

    public void ProgressUpdate()
    {
        _currentRatio = _target.position.x / _endPosition.x;
        _uiPathProgressBar.UpdateProgress(_currentRatio);
        
        if ((_currentRatio >= EndRatio) && _isActive)
            OnProgressComplete();
    }

    public void OnProgressComplete()
    {
        if (_isActive == false) return;
        
        _endAction?.Invoke();
        _isActive = false;
    }
}
