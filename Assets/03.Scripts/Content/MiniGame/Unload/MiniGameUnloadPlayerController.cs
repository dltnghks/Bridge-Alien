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
    private float _maxBoxWeight = 10f;
    private float _curBoxWeight = 0;
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
        _maxBoxWeight = 1000f;
        _curBoxWeight = 0;

        _boxList.SetBoxList(999);
        InteractionActionNumber = (int)MiniGameUnloadInteractionAction.None;
    }
    
    public void InputJoyStick(Vector2 input)
    {
        input = input - (input * (_boxList.CurrentUnloadBoxIndex * (_moveSpeedReductionRatio/100.0f)));
        // 플레이어 이동
        Player.PlayerMovement(input);
    }

    public void Interaction()
    {
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
                Logger.LogWarning($"{InteractionActionNumber} : Undefined Interaction");
                break;
        }
    }

    public bool ChangeInteraction(int actionNum)
    {
        if(actionNum == (int)MiniGameUnloadInteractionAction.DropBox && 
            _boxList.IsEmpty){
            return false;
        }

        InteractionActionNumber = actionNum;

        return true;
    }

    private void PickupBox()
    {
        Logger.Log("PickupBox");
        if(_boxList.IsFull)
        {
            return;
        }

        MiniGameUnloadBox box = _miniGameUnloadBoxSpawnPoint.BoxList.PeekBoxList();
        if (box != null && _curBoxWeight + box.Info.Weight <= _maxBoxWeight)
        {
            
            box.SetIsGrab(true);

            _miniGameUnloadBoxSpawnPoint.GetPickUpBox();
        
            _curBoxWeight += box.Info.Weight;
            // 상자를 스택에 추가하고 위치 설정
            _boxList.TryAddInGameUnloadBoxList(box);

            box.transform.SetParent(Player.CharacterTransform);
            box.transform.localPosition  = Vector3.right + Vector3.up * (_boxHeight + (box.Info.Size/2));
            box.transform.localRotation = Quaternion.identity;
            _boxHeight += box.Info.Size;

            Logger.Log("Player Current Box Weight : " + _curBoxWeight);
        }
        else
        {
            if (box == null)
                Logger.Log("No box to pick up!");
            else if(_curBoxWeight + box.Info.Weight >= _maxBoxWeight){
                Logger.Log("The weight of the current box exceeds the player's weight limit.");
            }
        }

    }


    private void DropBox()
    {
        Logger.Log("DropBox");
        if(_boxList.IsEmpty)
        {
            return;
        }

        MiniGameUnloadBox box = _boxList.RemoveAndGetTopInGameUnloadBoxList();
        if (box != null)
        {
            _curBoxWeight -= box.Info.Weight;

            box.CheckBrokenBox(_boxList.CurrentUnloadBoxIndex);

            // 상자를 플레이어의 발 아래로 놓기
            box.transform.SetParent(Managers.MiniGame.Root.transform);
            box.transform.position = Player.CharacterTransform.position + Vector3.up;
            box.SetIsGrab(false);
            
            _boxHeight -= box.Info.Size;

            Logger.Log("Player Current Box Weight : " + _curBoxWeight);
        }
        else
        {
            Logger.Log("No boxes to drop!");
        }
    }
}
