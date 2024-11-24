using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnloadPlayerController : IPlayerController
{
    public Player Player { get; set; }
    
    private Vector3 _holdPosition; // 기준이 되는 들고 있는 위치
    private List<MiniGameUnloadBox> _heldBoxes = new List<MiniGameUnloadBox>(); // 들고 있는 상자 스택
    private int _heldBoxesTop = 0;
    private float _boxHeight = 1f; // 상자 하나의 높이 (조정 가능)

    public void Init(Player player)
    {
        Player = player;
        _holdPosition = Player.transform.localPosition;
        _heldBoxesTop = 0;
        _heldBoxes.Clear();
    }
    
    public void InputJoyStick(Vector2 input)
    {
        if (input.x > 0)
        {
            _holdPosition = Player.transform.position + new Vector3(1f, 0f, 0f);
        }
        else if (input.x < 0)
        {
            _holdPosition = Player.transform.position + new Vector3(1f, 0f, 0f);
        }
        Debug.Log("_holdPosition : " + _holdPosition);
        // 플레이어 이동
        Player.PlayerMovement(input);
    }

    public void Interaction()
    {
        // 상자 들기
        PickupBox();
    }

    private void PickupBox()
    {
        // 플레이어 앞에 상자가 있는지 확인
        Vector3 rayOrigin = Player.transform.position; // 레이 시작 위치
        Vector3 rayDirection = Player.CharacterDirection; // 레이 방향
        float rayDistance = 2f; // 감지 거리

        // 레이를 시각적으로 표시
        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red, 1f); // Scene 뷰에 빨간색 레이 표시 (1초 지속)

        // Raycast를 실행하여 상자 감지
        Ray ray = new Ray(rayOrigin, rayDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.CompareTag("Box")) // 상자의 태그 확인
            {
                GameObject box = hit.collider.gameObject;

                // 상자의 Rigidbody 비활성화
                var boxRigidbody = box.GetComponent<Rigidbody>();
                if (boxRigidbody != null)
                {
                    boxRigidbody.isKinematic = true; // 물리 효과 제거
                }

                var boxCollision  = box.GetComponent<BoxCollider>();
                if (boxCollision != null)
                {
                    boxCollision.isTrigger = true; // 물리 효과 제거
                }

                
                // 상자를 스택에 추가하고 위치 설정
                _heldBoxes.Add(box.GetComponent<MiniGameUnloadBox>());

                
                box.transform.SetParent(Player.CharacterTransform);
                // 위치 계산
                box.transform.localPosition += Vector3.up * (_boxHeight * (_heldBoxes.Count - 1));
                box.transform.localRotation = Quaternion.identity;

                Debug.Log($"Picked up box! Total held boxes: {_heldBoxes.Count}");
            }
        }
        else
        {
            Debug.Log("No box to pick up!");
        }
    }


    private void DropBox()
    {
        /*if (_heldBoxes.Count > 0)
        {
            // 스택에서 가장 위에 있는 상자 제거
            GameObject box = _heldBoxes.Top().gameObject;

            // 상자의 Rigidbody 활성화
            var boxRigidbody = box.GetComponent<Rigidbody>();
            if (boxRigidbody != null)
            {
                boxRigidbody.isKinematic = false; // 물리 효과 다시 활성화
            }

            // 상자를 플레이어의 발 아래로 놓기
            box.transform.SetParent(Managers.MiniGame.Root.transform);

            Debug.Log($"Dropped box! Remaining boxes: {_heldBoxes.Count}");
        }
        else
        {
            Debug.Log("No boxes to drop!");
        }*/
    }
    
}
