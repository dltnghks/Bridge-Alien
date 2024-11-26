using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameUnloadBoxSpawnPoint : MonoBehaviour
{
    private SphereCollider _boxCollider;
    private Vector3 _boxSpawnPosition;
    public MiniGameUnloadBoxList BoxList {get; set;}

    private UnityAction _triggerAction;

    public void SetBoxSpawnPoint(int maxSpawnBoxIndex, UnityAction triggerAction = null)
    {
        _boxCollider = Utils.GetOrAddComponent<SphereCollider>(gameObject);
        _boxSpawnPosition = transform.position;
        BoxList = new MiniGameUnloadBoxList();
        BoxList.SetBoxList(maxSpawnBoxIndex);
        _triggerAction = triggerAction;
    }

    public bool TrySpawnBox(MiniGameUnloadBox box){
        if(BoxList.TryAddInGameUnloadBoxList(box))
        {
            _boxSpawnPosition += Vector3.up * box.Info.Size;
            box.SetInGameActive(true, _boxSpawnPosition);
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
            _boxSpawnPosition -= Vector3.up * box.Info.Size;
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
            Managers.MiniGame.CurrentGame.PlayerController.InteractionActionNumber = (int)MiniGameUnloadInteractionAction.PickUpBox;
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
}
