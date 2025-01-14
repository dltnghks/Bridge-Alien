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
            if (!box.IsUnloaded)
            {
                box.IsUnloaded = true;
                MoveBoxToEndPoint(box);
            }
        }
    }

    private void MoveBoxToEndPoint(MiniGameUnloadBox box)
    {
        int score = 0;
        if(!box.Info.IsGrab){
            if(box.Info.IsBroken)
            { 
                Logger.Log("broken box");
                score = -50;
                
                Managers.Resource.Destroy(box.gameObject);
                Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.BrokenBox.ToString(), gameObject);
                GenerateScoreTextObj(score, Color.red);
                return;
            }
            else if (CheckBoxInfo(box.Info))
            {
                Logger.Log("True Region");
                score = box.Info.Weight * 10;
            }
            else
            {
                Logger.Log("False Region");
                score = -box.Info.Weight * 10;
            }
        }

        box.transform.DOMove(_endPointTransform.position, 1).OnComplete(() =>
            {
                _action?.Invoke(score);
                
                // 획득 점수 표시
                if(score < 0)
                    GenerateScoreTextObj(score, Color.red);
                else
                    GenerateScoreTextObj(score, Color.green);
                
                Managers.Resource.Destroy(box.gameObject);
            }
        );
    }

    private void GenerateScoreTextObj(int amount, Color color)
    {
        InGameTextIndicator scoreTextObj = Managers.Resource.Instantiate("ScoreTextObj", transform).GetOrAddComponent<InGameTextIndicator>();
        scoreTextObj.Init(transform.position, amount, color, 0.5f);
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
