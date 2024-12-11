using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


[System.Serializable]
public struct MiniGameUnloadDeliveryPointInfo
{
    public Define.BoxRegion Region;
    
    public MiniGameUnloadDeliveryPointInfo(Define.BoxRegion region)
    {
        Region = region;
    }
}

public class MiniGameUnloadDeliveryPoint : MonoBehaviour
{
    [SerializeField] private MiniGameUnloadDeliveryPointInfo _info;
    
    private Transform _endPointTransform;
    
    private UnityAction<int> _action;
    private UnityAction _triggerAction;
    private BoxCollider _boxCollider;

    public void Start()
    {
        _boxCollider = Utils.GetOrAddComponent<BoxCollider>(gameObject);
        _endPointTransform = Utils.FindChild<Transform>(gameObject, "EndPoint", true);
    }

    public void SetAction(UnityAction<int> action, UnityAction triggerAction = null)
    {
        _action = action;
        _triggerAction = triggerAction;
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

        if (coll.gameObject.CompareTag("Box"))
        {
            MiniGameUnloadBox box = coll.gameObject.GetComponent<MiniGameUnloadBox>();
            MoveBoxToEndPoint(box);
        }
    }

    private void MoveBoxToEndPoint(MiniGameUnloadBox box)
    {
        if(!box.Info.IsGrab){
            if(box.Info.IsBroken)
            { 
                Logger.Log("broken box");
                _action?.Invoke(-10);
                box.SetInGameActive(false);
                return;
            }
            else if (CheckBoxInfo(box.Info))
            {
                Logger.Log("True Region");
                _action?.Invoke(box.Info.Weight);
            }
            else
            {
                Logger.Log("False Region");
                _action?.Invoke(-box.Info.Weight);
            }
        }

        box.transform.DOMove(_endPointTransform.position, 1).OnComplete(() =>
            box.SetInGameActive(false));
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

    public bool CheckBoxInfo(MiniGameUnloadBoxInfo boxInfo)
    {
        return boxInfo.Region == _info.Region;
    }
}
