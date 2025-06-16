using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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

public class MiniGameUnloadDeliveryPoint : MiniGameUnloadBasePoint, IBoxPlacePoint
{
    [SerializeField] private MiniGameUnloadDeliveryPointInfo _info;
    [SerializeField] private TextMeshPro _ViewDeliveryRegionText;
    
    private Transform _unloadPointTransform;
    private Transform _endPointTransform;
    
    private Action<int> _action;
    private Action _triggerAction;
    private Action<MiniGameUnloadBox> _returnAction;
    private BoxCollider _boxCollider;

    public void Start()
    {
        AllowedTypes = new Define.BoxType[] { Define.BoxType.Cold, Define.BoxType.Normal};
        
        _boxCollider = Utils.GetOrAddComponent<BoxCollider>(gameObject);
        _unloadPointTransform = Utils.FindChild<Transform>(gameObject, "UnloadPoint", true);
        _endPointTransform = Utils.FindChild<Transform>(gameObject, "EndPoint", true);
        _ViewDeliveryRegionText = Utils.FindChild<TextMeshPro>(gameObject,"ViewDeliveryRegionText", true);
        
        string regionName = _info.Region.ToString();
        _ViewDeliveryRegionText.SetText(regionName);
    }

    public void SetAction(Action<int> action, Action triggerAction = null, Action<MiniGameUnloadBox> returnAction = null)
    {
        _action = action;
        _triggerAction = triggerAction;
        _returnAction = returnAction;
        Managers.Sound.PlayAMB(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.Conveyor.ToString(), gameObject);
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

    private void MoveToUnloadPoint(MiniGameUnloadBox box)
    {
        box.transform.DOMove(_unloadPointTransform.position, 1).OnComplete(() =>
            {
                MoveBoxToEndPoint(box);
            }
        );
    }
    
    private void MoveBoxToEndPoint(MiniGameUnloadBox box)
    {
        int score = 0;
        bool reutnrBox = false;
        
        if (!box.Info.IsGrab)
        {
            if (box.BoxType != Define.BoxType.Normal)
            {
                score = -50;
                reutnrBox = true;
            }
            else if (box.Info.IsBroken)
            {
                Logger.Log("broken box");
                score = -50;
                reutnrBox = true;

                Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.BrokenBox.ToString(), gameObject);
            }
            else if (CheckBoxInfo(box.Info))
            {
                Logger.Log("True Region");
                score = 100;
            }
            else
            {
                Logger.Log("False Region");
                score = -50;
            }
        }

        _action?.Invoke(score);
                
        // 획득 점수 표시
        if(score < 0)
            GenerateScoreTextObj(score, Color.red);
        else
            GenerateScoreTextObj(score, Color.green);


        box.transform.DOMove(_endPointTransform.position, 1).OnComplete(() =>
            {
                if (reutnrBox)
                {
                    ReturnBox(box);
                }
                else
                {
                    Managers.Resource.Destroy(box.gameObject);
                }
            }
        );
    }

    private void ReturnBox(MiniGameUnloadBox box)
    {
        _returnAction.Invoke(box);
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

    public bool CanPlaceBox(MiniGameUnloadBox box)
    {
        return CanProcess(box.BoxType);
    }

    public void PlaceBox(MiniGameUnloadBox box)
    {
        MiniGameUnloadBox boxComponent = box.GetComponent<MiniGameUnloadBox>();
        if (boxComponent != null)
        {
            MoveToUnloadPoint(boxComponent);
        }
    }
}

