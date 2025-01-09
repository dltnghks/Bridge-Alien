using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameUnloadBoxSpawnPoint : MonoBehaviour
{
    private SphereCollider _boxCollider;
    private Vector3 _boxSpawnPosition;
    public MiniGameUnloadBoxList BoxList {get; set;}
    public float _boxHeight = 0;

    private UnityAction _triggerAction;

    public void SetBoxSpawnPoint(int maxSpawnBoxIndex, UnityAction triggerAction = null)
    {
        _boxCollider = Utils.GetOrAddComponent<SphereCollider>(gameObject);
        _boxSpawnPosition = transform.position;
        _boxSpawnPosition.y = 0;
        BoxList = new MiniGameUnloadBoxList();
        BoxList.SetBoxList(maxSpawnBoxIndex);
        _triggerAction = triggerAction;
    }

    public bool TrySpawnBox(MiniGameUnloadBox box){
        if(!box.gameObject.activeSelf && BoxList.TryAddInGameUnloadBoxList(box))
        {
            Vector3 spawnPos = _boxSpawnPosition + Vector3.up * (_boxHeight + box.Info.Size/2);
            _boxHeight += box.Info.Size;
            box.SetInGameActive(true, spawnPos);
            return true;
        }
        else
        {
            return false;
        }
    }

    public MiniGameUnloadBox GetPickUpBox(){
        MiniGameUnloadBox box = BoxList.RemoveAndGetTopInGameUnloadBoxList();
        if(box != null)
        {
            _boxHeight -= box.Info.Size;
            return box;
        }
        else
        {
            return null;
        }
        
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            if(Managers.MiniGame.CurrentGame.PlayerController.ChangeInteraction((int)MiniGameUnloadInteractionAction.PickUpBox)){
                _triggerAction?.Invoke();
            }
        }
    }
    
    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            if(Managers.MiniGame.CurrentGame.PlayerController.ChangeInteraction((int)MiniGameUnloadInteractionAction.None)){
                _triggerAction?.Invoke();
            }
        }
       
    }
}
