using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameMapPlayerController : MonoBehaviour
{
    public Camera MainCamera{get; private set;}
    public GameObject PlayerCharacter{get; private set;}
    public List<MiniGamePoint> MiniGamePointList;

    private int _currentMiniGamePoint = 0;
    // Pointer 위치 가져오기
    private Vector2 _touchPosition;

    private bool _initialized = false; 
    
    private void Start(){
        MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        
        PlayerCharacter = GameObject.FindGameObjectWithTag("Player");
        PlayerCharacter.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
        
        GameObject miniGamePointListObj = GameObject.Find("MiniGamePointList");
        
        if(miniGamePointListObj){
            foreach(var point in miniGamePointListObj.transform.GetComponentsInChildren<MiniGamePoint>()){
                MiniGamePointList.Add(point);

                if (point.MiniGameSceneType == Managers.Scene.SelectedSceneType)
                {
                    Logger.Log($"Selected MiniGamePoint : {point.transform.position}");
                    Vector3 pos = point.transform.position;
                    pos.y = PlayerCharacter.transform.position.y;
                    PlayerCharacter.transform.position = pos;
                    
                    _currentMiniGamePoint = MiniGamePointList.IndexOf(point);
                }
            }
        }else{
            Logger.LogWarning("Point List가 없습니다.");
        }
        
        _initialized = true;
    }

    private void OnPointer(InputValue value)
    {
        // Pointer 위치 가져오기
        _touchPosition = value.Get<Vector2>();
    }
    
    private bool IsPointerOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition // 터치 입력일 경우, Input.GetTouch(0).position 사용
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        return results.Count > 0;
    }


    private void OnTouch(){
        if(IsPointerOverUI() || !_initialized){
            Debug.Log("Clicked on UI");
            return; // UI 클릭 시 뒤 오브젝트 처리 중단
        }
        // Raycast로 3D 오브젝트 클릭 감지
        Ray ray = MainCamera.ScreenPointToRay(_touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"Touched on 3D Object: {hit.collider.gameObject.name}");
            
            MiniGamePoint miniGamePoint = hit.collider.GetComponent<MiniGamePoint>();
            if(miniGamePoint != null){ 
                Managers.Scene.SelectedSceneType = Define.Scene.Unknown;
                MoveToMiniGamePoint(miniGamePoint);
            }
            else{
                // IClickable 인터페이스 호출
               hit.collider.GetComponent<MiniGamePoint>()?.OnClick();
            }
        }
    }

    private void MoveToMiniGamePoint(MiniGamePoint miniGamePoint)
    {
        if (!_initialized)
        {
            return;
        }
     
        int destIndex = MiniGamePointList.IndexOf(miniGamePoint);

        if (_currentMiniGamePoint == destIndex)
        {
            // UI 변경
            miniGamePoint.OnClick();
            return;
        }
        
        if(_currentMiniGamePoint < destIndex) destIndex = _currentMiniGamePoint+1;
        else if(_currentMiniGamePoint > destIndex) destIndex = _currentMiniGamePoint-1;

        if (destIndex < 0 || destIndex >= MiniGamePointList.Count)
        {
            return;
        }
        
        Vector3 pos = MiniGamePointList[destIndex].transform.position;
        pos.y += PlayerCharacter.transform.position.y;
        
        PlayerCharacter.transform.DOMove(pos, 0.2f).OnComplete(() =>
            {
                _currentMiniGamePoint = destIndex;
                MoveToMiniGamePoint(miniGamePoint);
            }
        );
    }
}
