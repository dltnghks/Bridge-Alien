using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MiniGameUnloadDisposalPoint : MiniGameUnloadBasePoint, IBoxPlacePoint
{
    // 폐기하는 구역
    [Header("Setting")]
    private int _maxIndex = 3;
    
    private MiniGameUnloadBoxList _boxList = new MiniGameUnloadBoxList();
    private Action _triggerAction;
    public void Start()
    {
        AllowedTypes = new Define.BoxType[] { Define.BoxType.Disposal};
        _boxList.SetBoxList(_maxIndex);
    }

    public void SetDisposalPoint(Action triggerAction)
    {
        _triggerAction = triggerAction;
    }
    
    public override bool CanProcess(Define.BoxType boxType)
    {
        return base.CanProcess(boxType);
    }
    
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            if(Managers.MiniGame.CurrentGame.PlayerController.ChangeInteraction((int)MiniGameUnloadInteractionAction.DropBox))
            {
                _triggerAction?.Invoke();
            }
        }
    }
    
    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            if(Managers.MiniGame.CurrentGame.PlayerController.ChangeInteraction((int)MiniGameUnloadInteractionAction.None))
            {
                _triggerAction?.Invoke();
            }
        }
    }
    
    public bool CanPlaceBox(MiniGameUnloadBox box)
    {
        return CanProcess(box.BoxType) && !_boxList.IsFull;
    }

    public void PlaceBox(MiniGameUnloadBox box)
    {
        if (box != null && _boxList.TryAddInGameUnloadBoxList(box))
        {
            box.transform.DOMove(transform.position, 1f);
            // 박스 상태 업데이트
            box.transform.SetParent(transform);
            box.SetIsGrab(false);
        }
    }
}
