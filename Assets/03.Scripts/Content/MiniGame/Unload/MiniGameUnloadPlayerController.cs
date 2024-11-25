using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public enum MiniGameUnloadInteractionAction
{
    None,
    PickUpBox,
    DropBox,   
}

public class MiniGameUnloadPlayerController : IPlayerController
{
    private MiniGameUnloadBoxList _boxList = new MiniGameUnloadBoxList();
    private float _boxHeight = 0f;
    private float _detectionBoxRadius = 2f;
    private float _moveSpeedReductionRatio = 2.0f;
    private MiniGameUnloadBoxSpawnPoint _miniGameUnloadBoxSpawnPoint;

    public Player Player { get; set; }
    public int InteractionActionNumber { get; set; }
    public bool IsDropBox { get; set; }

    public MiniGameUnloadPlayerController(){}
    public MiniGameUnloadPlayerController(Player player, float radius, float moveSpeedReductionRatio, MiniGameUnloadBoxSpawnPoint miniGameUnloadBoxSpawnPoint){
        Init(player);
        _detectionBoxRadius = radius;
        _moveSpeedReductionRatio = moveSpeedReductionRatio;
        _miniGameUnloadBoxSpawnPoint = miniGameUnloadBoxSpawnPoint;
    }

    public void Init(Player player)
    {
        Player = player;
        // 플레이어 능력치를 기반으로 수정
        _boxList.SetBoxList(3);
        InteractionActionNumber = (int)MiniGameUnloadInteractionAction.None;
    }
    
    public void InputJoyStick(Vector2 input)
    {
        // 플레이어 이동
        Player.PlayerMovement(input);
    }

    public void Interaction()
    {
        Debug.Log(InteractionActionNumber);
        switch((MiniGameUnloadInteractionAction)InteractionActionNumber)
        {
            case MiniGameUnloadInteractionAction.None:
                break;
            case MiniGameUnloadInteractionAction.PickUpBox:
                PickupBox();
                break;
            case MiniGameUnloadInteractionAction.DropBox:
                DropBox();
                break;
            default:
                Debug.LogWarning($"{InteractionActionNumber} : Undefined Interaction");
                break;
        }
    }

    private void PickupBox()
    {
        Debug.Log("PickupBox");
        if(_boxList.IsFull)
        {
            return;
        }

        MiniGameUnloadBox box = _miniGameUnloadBoxSpawnPoint.GetPickUpBox();
        if (box != null)
        {
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
            _boxList.TryAddInGameUnloadBoxList(box);

            box.transform.SetParent(Player.CharacterTransform);
            _boxHeight += box.Info.Size;
            box.transform.localPosition  = Vector3.right + Vector3.up * _boxHeight;
            box.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.Log("No box to pick up!");
        }

    }


    private void DropBox()
    {
        Debug.Log("DropBox");
        if(_boxList.IsEmpty)
        {
            return;
        }

        MiniGameUnloadBox box = _boxList.RemoveAndGetTopInGameUnloadBoxList();
        if (box != null)
        {
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
            _boxHeight -= box.Info.Size;

            Vector3 playerDirection;
            if (Player.IsRight) playerDirection = Vector3.right;
            else playerDirection = Vector3.left;

            box.transform.position = Player.CharacterTransform.position + playerDirection;
        }
        else
        {
            Debug.Log("No boxes to drop!");
        }
    }
}
