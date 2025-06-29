using System;
using System.Collections;
using UnityEngine;

public class BoxWarpSkill : ChargeSkill
{    private MiniGameUnloadBoxList _playerBoxList; // 플레이어 상자 더미 참조
    private MiniGameUnloadDeliveryPoint[] _deliveryPoints; // 배달 지점 목록

    private Action OnDropBox; // 상자 배달 완료 후 호출될 액션

    public void SetDropBoxAction(Action action)
    {
        OnDropBox = action;
    }

    public void SetPlayerBoxList(MiniGameUnloadBoxList playerBoxList)
    {
        Logger.Log("Setting player box list for BoxWarpSkill");
        _playerBoxList = playerBoxList;
    }

    public void SetDeliveryPointList(MiniGameUnloadDeliveryPoint[] deliveryPoints)
    {
        Logger.Log("Setting delivery points for BoxWarpSkill");
        _deliveryPoints = deliveryPoints;
    }

    protected override void OnActivate()
    {
        
        remainingCharges--; // 성공적으로 사용했으므로 횟수 감소
        OnCountChanged?.Invoke(remainingCharges); // 사용 횟수 감소 알림
        // 사용 횟수 감소는 성공적으로 사용했을 때만 처리
        StartCoroutine(BoxWarpProcess());
    }

    public override bool CanUseSkill()
    {
        // 횟수가 남았는지와 상자를 들고 있는지 함께 체크
        return base.CanUseSkill() && _playerBoxList != null && !_playerBoxList.IsEmpty;
    }

    private IEnumerator BoxWarpProcess()
    {
        MiniGameUnloadBox topBox = _playerBoxList.PeekBoxList();
        
        // 이펙트 표시 1
        yield return new WaitForSeconds(0.5f);
        
        // 상자 이동
        MiniGameUnloadBox box = _playerBoxList.PeekBoxList();

        foreach (var deliveryPoint in _deliveryPoints)
        {
            if (deliveryPoint.CheckBoxInfo(box.Info) && deliveryPoint.CanPlaceBox(box))
            {
                // 상자를 배달 지점으로 이동
                deliveryPoint.PlaceBox(box);
                OnDropBox?.Invoke(); // 상자 배달 완료 후 액션 호출
                Logger.Log($"Teleported box to {deliveryPoint.name}");
                break;
            }
        }

        // 이펙트 표시
    }

    public override void TryActivate()
    {
        Logger.Log("BoxWarpSkill TryActivate");
        if(CanUseSkill())
        {
            OnActivate();
        }
        else
        {
            Logger.Log("Cannot use BoxWarpSkill: either no charges left or no box to teleport.");
        }
    }
}
