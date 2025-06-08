using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameUnloadBoxSpawnPoint : MonoBehaviour
{
    private SphereCollider _boxCollider;
    private Vector3 _boxSpawnPosition;
    public MiniGameUnloadBoxList BoxList { get; set; }
    public float _boxHeight = 1.0f;
    private float _boxHeightOffset = 1.0f;

    private UnityAction _triggerAction;

    public void SetBoxSpawnPoint(int maxSpawnBoxIndex, UnityAction triggerAction = null)
    {
        Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.Truck.ToString(), gameObject);

        _boxCollider = Utils.GetOrAddComponent<SphereCollider>(gameObject);
        _boxSpawnPosition = transform.position;
        BoxList = new MiniGameUnloadBoxList();
        BoxList.SetBoxList(maxSpawnBoxIndex);
        _triggerAction = triggerAction;
    }

    public bool TrySpawnBox(MiniGameUnloadBox box){
        if(!box.gameObject.activeSelf && BoxList.TryAddInGameUnloadBoxList(box))
        {
            _boxHeight += _boxHeightOffset;
            Vector3 spawnPos = _boxSpawnPosition + Vector3.up * _boxHeight;

            // z-ordering, 겹치면 렌더링 충돌나서 z를 살짝 조절, 위로 올라갈수록 앞으로
            spawnPos.z += -(_boxHeight / ((float)BoxList.MaxUnloadBoxIndex * 100f));
            
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
            _boxHeight -= _boxHeightOffset;
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
