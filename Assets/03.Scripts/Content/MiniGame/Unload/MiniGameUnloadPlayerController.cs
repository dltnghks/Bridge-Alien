using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

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
    private float _moveSpeedReductionRatio = 2.0f;
    private MiniGameUnloadBoxSpawnPoint _miniGameUnloadBoxSpawnPoint;
    private MiniGameUnloadColdPoint _miniGameUnloadColdPoint;
    private List<MiniGameUnloadBasePoint> _cachedPoints = new List<MiniGameUnloadBasePoint>();
    private bool _isPointsCached;
    
    public Player Player { get; set; }
    public int InteractionActionNumber { get; set; }
    public bool IsDropBox { get; set; }

    public MiniGameUnloadPlayerController(){}
    public MiniGameUnloadPlayerController(Player player, float radius, float moveSpeedReductionRatio, MiniGameUnloadBoxSpawnPoint miniGameUnloadBoxSpawnPoint, MiniGameUnloadColdPoint miniGameUnloadColdPoint){
        Init(player);
        _moveSpeedReductionRatio = moveSpeedReductionRatio;
        _miniGameUnloadBoxSpawnPoint = miniGameUnloadBoxSpawnPoint;
        _miniGameUnloadColdPoint = miniGameUnloadColdPoint;
    }

    public void Init(Player player)
    {
        Player = player;

        _boxList.SetBoxList(3);
        InteractionActionNumber = (int)MiniGameUnloadInteractionAction.None;
        CacheAllPoints();
    }
    
    private void CacheAllPoints()
    {
        if (_isPointsCached) return;
        _cachedPoints.Clear();
        _cachedPoints.AddRange(Object.FindObjectsOfType<MiniGameUnloadBasePoint>());
        _isPointsCached = true;
    }
    
    public void InputJoyStick(Vector2 input)
    {
        if(Managers.MiniGame.CurrentGame.IsPause){
           return; 
        }

        input = input - (input * (_boxList.CurrentUnloadBoxIndex * (_moveSpeedReductionRatio/100.0f)));
        // 플레이어 이동
        Player.PlayerMovement(input);
    }

    public void Interaction()
    {
        if(Managers.MiniGame.CurrentGame.IsPause){
           return; 
        }

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
        if (_boxList.IsFull)
        {
            Logger.Log("Player Box is full");
            return;
        }
        // 1. 전체 포인트(스폰포인트, 냉동포인트 등)에서 가장 가까운 박스 찾기
        MiniGameUnloadBox nearestBox = FindNearestPickupBox();
        if (nearestBox == null) return;
            
        // 3. 포인트별 처리 (스폰포인트/냉동포인트 등)
        if (nearestBox.transform.parent != null)
        {
            MiniGameUnloadColdPoint coldPoint = nearestBox.GetComponentInParent<MiniGameUnloadColdPoint>();
            if (coldPoint != null)
            {
                nearestBox = coldPoint.GetPickUpBox();
            }
            else
            {
                // 스폰포인트 또는 다른 포인트에서 박스 제거
                nearestBox = _miniGameUnloadBoxSpawnPoint.GetPickUpBox();
            }
        }
        else
        {
            // 아무 포인트에도 속하지 않은 박스(예: 컨베이어 등)일 경우 별도 처리 없음
        }

        if (nearestBox == null)
        {
            return;
        }

        nearestBox.SetIsGrab(true);
        
        // 상자를 스택에 추가하고 위치 설정
        _boxList.TryAddInGameUnloadBoxList(nearestBox);

        nearestBox.transform.SetParent(Player.CharacterTransform);
        nearestBox.transform.localPosition = Vector3.right + Vector3.up * (_boxHeight);
        nearestBox.transform.localRotation = Quaternion.identity;
        _boxHeight += 1f;

    }


    private void DropBox()
    {
        if (_boxList.IsEmpty) return;

        // 1. 가장 가까운 포인트 찾기
        MiniGameUnloadBasePoint nearestPoint = FindNearestValidPoint();
        if (nearestPoint == null)
        {
            Debug.Log("포인트가 감지되지 않음");
            return;
        }

        // 2. 현재 들고 있는 박스 확인
        MiniGameUnloadBox carriedBox = _boxList.PeekBoxList();
        if (carriedBox == null) return;

        // 3. 포인트-박스 타입 호환성 검사
        if (!nearestPoint.CanProcess(carriedBox.Info.BoxType))
        {
            Debug.Log($"이 포인트에는 {carriedBox.Info.BoxType} 상자를 놓을 수 없음");
            return;
        }

        // 4. 포인트별 처리 로직 실행
        bool processSuccess = nearestPoint.ProcessBox(carriedBox.gameObject);
        if (!processSuccess)
        {
            Debug.Log("처리 실패: 포인트가 가득 찼거나 조건 불일치");
            return;
        }

        // 5. 플레이어 박스 리스트에서 제거
        _boxList.RemoveAndGetTopInGameUnloadBoxList();
        _boxHeight -= 1f;

        // 6. 박스 상태 업데이트
        carriedBox.transform.SetParent(nearestPoint.transform);
        carriedBox.SetIsGrab(false);
    }

    
    private MiniGameUnloadBasePoint FindNearestValidPoint()
    {
        MiniGameUnloadBasePoint nearest = null;
        float minDist = float.MaxValue;
        foreach (var point in _cachedPoints)
        {
            float dist = Vector3.Distance(Player.transform.position, point.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = point;
            }
        }
        return nearest; // 일정 거리 내에 있을 때만
    }
    
    private MiniGameUnloadBox FindNearestPickupBox()
    {
        Vector3 playerPos = Player.transform.position;
        MiniGameUnloadBox nearestBox = null;
        float minDist = float.MaxValue;

        // 1. 스폰포인트의 박스 체크
        MiniGameUnloadBox spawnBox = _miniGameUnloadBoxSpawnPoint.BoxList.PeekBoxList();
        if (spawnBox != null)
        {
            float dist = Vector3.Distance(playerPos, spawnBox.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestBox = spawnBox;
            }
        }

        // 2. 냉동포인트 등 다른 포인트의 박스 체크
        MiniGameUnloadBox coldBox = _miniGameUnloadColdPoint.CurrentBox;
        if(coldBox != null){
            float dist = Vector3.Distance(playerPos, coldBox.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestBox = coldBox;
            }
        }

        return nearestBox;
    }
}
