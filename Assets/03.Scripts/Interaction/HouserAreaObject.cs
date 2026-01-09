using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class HouserAreaObject : MonoBehaviour, IInteractable
{
    public int Priority => -1;

    private Vector2 _targetPosition;
    private Collider2D _areaCollider;

    private void Awake()
    {
        _areaCollider = GetComponent<Collider2D>();
    }
    
    private void Update()
    {
        if (Managers.UI != null && Managers.UI.IsBlurActive)
        {
            return;
        }

        // 화면에 터치가 하나 이상 감지되면
        if (Input.touchCount > 0)
        {
            // 첫 번째 터치 정보를 가져옴
            Touch touch = Input.GetTouch(0);

            // 터치가 시작되는 순간을 감지
            if (touch.phase == TouchPhase.Began)
            {
                HandleInput(touch.position);
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput(Input.mousePosition);
        }
#endif        
    }

    /**
    * 화면 좌표(screenPosition)를 받아 Raycast를 수행하는 공통 함수
    */
    private void HandleInput(Vector2 screenPosition)
    {
        if (_areaCollider == null)
        {
            return;
        }

        // 1. 화면 좌표를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        if (_areaCollider.OverlapPoint(worldPosition) == false)
        {
            return;
        }
        
        // 2. Raycast 발사
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        // 이동을 위한 타겟 위치 설정
        _targetPosition = worldPosition;
    }


    public bool CanInteract()
    {
        return true;
    }

    public string GetInteractionPrompt()
    {
        return "이동하기";
    }

    public void Interact(Interactor interactor)
    {
        if (!CanInteract())
        {
            return;
        }

        interactor.InteractionComplete();
        HousePlayer player = interactor.GetComponent<HousePlayer>();
        player.MoveToTarget(_targetPosition);
    }

    private void EndInteract()
    {
    }
}
