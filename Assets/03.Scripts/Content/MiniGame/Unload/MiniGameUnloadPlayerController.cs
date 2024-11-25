using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnloadPlayerController : IPlayerController
{
    private List<MiniGameUnloadBox> _heldBoxes = new List<MiniGameUnloadBox>(); // 들고 있는 상자 스택
    private int _heldBoxesTop = 0;
    private float _boxHeight = 1f; // 상자 하나의 높이 (조정 가능)
    
    private float _detectionBoxRadius = 2f;

    public Player Player { get; set; }
    public bool IsDropBox { get; set; }
    
    public MiniGameUnloadPlayerController(){}
    public MiniGameUnloadPlayerController(Player player, float radius){
        Init(player);
        _detectionBoxRadius = radius;
    }

    public void Init(Player player)
    {
        Player = player;
        _heldBoxesTop = 0;
        _heldBoxes.Clear();
        IsDropBox = false;
    }
    
    public void InputJoyStick(Vector2 input)
    {
        // 플레이어 이동
        Player.PlayerMovement(input);
    }

    public void Interaction()
    {
        if (!IsDropBox)
        {
            // 상자 들기
            PickupBox();   
        }
        else
        {
            DropBox();
        }
    }

    private void PickupBox()
    {
        Debug.Log("Pickup box");

        // 플레이어 앞에 상자가 있는지 확인
        Vector3 rayOrigin = Player.CharacterTransform.position; // 레이 시작 위치

        float sphereDistance = 2f; // 감지 거리

        // 감지 실행
        Collider[] hits = Physics.OverlapSphere(rayOrigin, sphereDistance);

        Collider hit = null;
        float minDistance = int.MaxValue;
        foreach (var hitObj in hits)
        {
            float newDistance = 0;
            if(hitObj.CompareTag("Box") && minDistance > (newDistance = Vector3.Distance(Player.transform.position, hitObj.transform.position)))
            {
                minDistance = newDistance;
                hit = hitObj;
            }
        }

        if (hit != null)
        {
            GameObject box = hit.gameObject;

            // 상자의 Rigidbody 비활성화
            var boxRigidbody = box.GetComponent<Rigidbody>();
            if (boxRigidbody != null)
            {
                boxRigidbody.isKinematic = true; // 물리 효과 제거
            }

            var boxCollision  = box.GetComponent<BoxCollider>();
            if (boxCollision != null)
            {
                boxCollision.enabled = false;
            }

            
            // 상자를 스택에 추가하고 위치 설정
            _heldBoxes.Add(box.GetComponent<MiniGameUnloadBox>());
            
            box.transform.SetParent(Player.CharacterTransform);
            box.transform.localPosition  = Vector3.right + Vector3.up * (_boxHeight * (_heldBoxes.Count - 1));
            box.transform.localRotation = Quaternion.identity;

            Debug.Log($"Picked up box! Total held boxes: {_heldBoxes.Count}");

            _heldBoxesTop++;
        }
        else
        {
            Debug.Log("No box to pick up!");
        }

    }


    private void DropBox()
    {
        Debug.Log("Drop box");
        if (_heldBoxes.Count > 0 && _heldBoxes.Contains(_heldBoxes[_heldBoxesTop-1]))
        {
            // 스택에서 가장 위에 있는 상자 제거
            GameObject box = _heldBoxes[_heldBoxesTop-1].gameObject;
            _heldBoxes.RemoveAt(_heldBoxesTop-1);
            // 상자의 Rigidbody 활성화
            var boxRigidbody = box.GetComponent<Rigidbody>();
            if (boxRigidbody != null)
            {
                boxRigidbody.isKinematic = false; // 물리 효과 다시 활성화
            }

            var boxCollision  = box.GetComponent<BoxCollider>();
            if (boxCollision != null)
            {
                boxCollision.enabled = true;
            }

            
            // 상자를 플레이어의 발 아래로 놓기
            box.transform.SetParent(Managers.MiniGame.Root.transform);

            Vector3 playerDirection;
            if (Player.IsRight) playerDirection = Vector3.right;
            else playerDirection = Vector3.left;

            box.transform.position = Player.CharacterTransform.position + playerDirection;

            Debug.Log($"Dropped box! Remaining boxes: {_heldBoxes.Count}");
            _heldBoxesTop--;
        }
        else
        {
            Debug.Log("No boxes to drop!");
        }
    }

    public void ChangeInteraction()
    {
        Debug.Log("Change interaction");
        IsDropBox = !IsDropBox;
    }
}
