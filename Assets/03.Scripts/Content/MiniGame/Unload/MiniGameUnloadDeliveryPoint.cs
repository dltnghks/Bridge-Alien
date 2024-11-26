using System;
using System.Collections;
using System.Collections.Generic;
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
    
    private UnityAction<int> _action;
    private UnityAction _triggerAction;
    private BoxCollider _boxCollider;

    public void Start()
    {
        _boxCollider = Utils.GetOrAddComponent<BoxCollider>(gameObject);
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
            Managers.MiniGame.CurrentGame.PlayerController.InteractionActionNumber = (int)MiniGameUnloadInteractionAction.DropBox;
        }

        if (coll.gameObject.CompareTag("Box"))
        {
            MiniGameUnloadBox box = coll.gameObject.GetComponent<MiniGameUnloadBox>();
            coll.gameObject.SetActive(false);
            if (CheckBoxInfo(box.Info))
            {
                Debug.Log("True Region");
                _action?.Invoke(box.Info.Weight);
            }
            else
            {
                Debug.Log("False Region");
                _action?.Invoke(-box.Info.Weight);
            }
        }
        
        _triggerAction?.Invoke();
    }
    
    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            Managers.MiniGame.CurrentGame.PlayerController.InteractionActionNumber = (int)MiniGameUnloadInteractionAction.None;
        }
        _triggerAction?.Invoke();
    }

    public bool CheckBoxInfo(MiniGameUnloadBoxInfo boxInfo)
    {
        return boxInfo.Region == _info.Region;
    }
}
