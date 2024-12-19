using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameMapPlayerController : MonoBehaviour
{
    public Camera MainCamera{get; private set;}
    public Player PlayerCharacter{get; private set;}

    // Pointer 위치 가져오기
    private Vector2 _touchPosition;
    private Define.Scene _selectedScene;

    private void Start(){
        MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        PlayerCharacter = GameObject.Find("Player").GetComponent<Player>();
    }

    private void OnPointer(InputValue value)
    {
        // Pointer 위치 가져오기
        _touchPosition = value.Get<Vector2>();
    }

    private void OnTouch(){
        if(EventSystem.current.IsPointerOverGameObject()){
            Debug.Log("Clicked on UI");
            return; // UI 클릭 시 뒤 오브젝트 처리 중단
        }
        // Raycast로 3D 오브젝트 클릭 감지
        Ray ray = MainCamera.ScreenPointToRay(_touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"Touched on 3D Object: {hit.collider.gameObject.name}");

            // IClickable 인터페이스 호출
            hit.collider.GetComponent<MiniGamePoint>()?.OnClick();
            
            MiniGamePoint miniGamePoint = hit.collider.GetComponent<MiniGamePoint>();
            if(miniGamePoint != null){
                _selectedScene = miniGamePoint.MiniGameType;        
                MoveToPoint(hit.collider.transform.position);
            }
        }
    }

    private void MoveToPoint(Vector3 pos){
        pos.y += PlayerCharacter.transform.position.y;
        PlayerCharacter.transform.DOMove(pos, 0.5f);
    }
}
