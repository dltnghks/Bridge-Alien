using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnloadBoxSpawnPoint : MonoBehaviour
{
    private SphereCollider _boxCollider;
    private Vector3 _boxSpawnPosition;
    public MiniGameUnloadBoxList BoxList {get; set;}

    public void SetBoxSpawnPoint(int maxSpawnBoxIndex)
    {
        _boxCollider = Utils.GetOrAddComponent<SphereCollider>(gameObject);
        _boxSpawnPosition = transform.position;
        BoxList = new MiniGameUnloadBoxList();
        BoxList.SetBoxList(maxSpawnBoxIndex);
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
    }
    
    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            Managers.MiniGame.CurrentGame.PlayerController.InteractionActionNumber = (int)MiniGameUnloadInteractionAction.None;
        }
    }
}
