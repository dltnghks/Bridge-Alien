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

    private BoxCollider _boxCollider;

    public void Start()
    {
        _boxCollider = Utils.GetOrAddComponent<BoxCollider>(gameObject);
    }

    public void SetAction(UnityAction<int> action)
    {
        _action = action;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // UI ë³?ê²?
            Managers.MiniGame.CurrentGame.PlayerController.ChangeInteraction();
        }

        if (collision.gameObject.CompareTag("Box"))
        {
            MiniGameUnloadBox box = collision.gameObject.GetComponent<MiniGameUnloadBox>();
            collision.gameObject.SetActive(false);
            if (CheckBoxInfo(box.Info))
            {
                Debug.Log("True Region");
                _action?.Invoke(box.Info.Weight);
            }
            else
            {
                Debug.Log("False Region");
            }
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // UI ë³?ê²?
            Managers.MiniGame.CurrentGame.PlayerController.ChangeInteraction();
        }
    }

    public bool CheckBoxInfo(MiniGameUnloadBoxInfo boxInfo)
    {
        return boxInfo.Region == _info.Region;
    }
}
