using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameUnloadColdPoint : MiniGameUnloadBasePoint, IBoxPlacePoint
{
    [Header("Setting")]
    private int _maxIndex = 1;
    
    private MiniGameUnloadBoxList _boxList = new MiniGameUnloadBoxList();
    private UnityAction _triggerAction;
    
    public MiniGameUnloadBox CurrentBox { get { return _boxList.PeekBoxList(); } }

    private void Awake()
    {
        AllowedTypes = new Define.BoxType[] { Define.BoxType.Cold};
        _boxList.SetBoxList(_maxIndex);
    }

    public void SetColdArea(int maxIndex, UnityAction triggerAction)
    {
        _maxIndex = maxIndex;
        _triggerAction = triggerAction;
    }
    
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            var box = CurrentBox;
            
            if (box != null)
            {
                if(box.Info.BoxType == Define.BoxType.Normal && 
                   Managers.MiniGame.CurrentGame.PlayerController.ChangeInteraction((int)MiniGameUnloadInteractionAction.PickUpBox))
                {
                    _triggerAction?.Invoke();    
                } 
                else if (box.Info.BoxType == Define.BoxType.Cold &&
                         Managers.MiniGame.CurrentGame.PlayerController.ChangeInteraction(
                             (int)MiniGameUnloadInteractionAction.None))
                {
                    _triggerAction?.Invoke();
                }
                        
            }
            else if(Managers.MiniGame.CurrentGame.PlayerController.ChangeInteraction((int)MiniGameUnloadInteractionAction.DropBox))
            {
                _triggerAction?.Invoke();
            }
        }

        if (coll.gameObject.CompareTag("Box"))
        {
            ColdBox box = coll.gameObject.GetComponent<ColdBox>();
            if (!box.IsUnloaded)
            {
                box.IsUnloaded = true;
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

    public MiniGameUnloadBox GetPickUpBox(){
        MiniGameUnloadBox box = _boxList.RemoveAndGetTopInGameUnloadBoxList();
        if(box != null)
        {
            return box;
        }
        else
        {
            return null;
        }
        
    }

    public bool CanPlaceBox(MiniGameUnloadBox box)
    {
        return CanProcess(box.BoxType) && !_boxList.IsFull;
    }

    public void PlaceBox(MiniGameUnloadBox box)
    {
        ColdBox coldBox = box.GetComponent<ColdBox>();
        if (coldBox != null && _boxList.TryAddInGameUnloadBoxList(coldBox))
        {
            _boxList.TryAddInGameUnloadBoxList(coldBox);
            coldBox.EnterCoolingArea();
            coldBox.transform.DOMove(transform.position, 1f);
        }
    }
}
