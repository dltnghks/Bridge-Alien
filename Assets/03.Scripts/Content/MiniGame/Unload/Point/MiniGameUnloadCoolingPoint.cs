using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameUnloadCoolingPoint : MiniGameUnloadBasePoint, IBoxPlacePoint, IBoxPickupPoint
{
    [Header("Setting")]
    private int _maxIndex = 1;

    private MiniGameUnloadBoxList _boxList = new MiniGameUnloadBoxList();
    private UnityAction _triggerAction;

    public MiniGameUnloadBox CurrentBox { get { return _boxList.Peek(); } }

    [SerializeField]
    private CoolingGauge _coolingGauge;
    [SerializeField]
    private CoolingTimer _coolingTimer;
    [SerializeField]
    private ParticleSystem _coolingEffect;

    private void Start()
    {
        AllowedTypes = new Define.BoxType[] { Define.BoxType.Cold };
        _boxList.SetBoxList(_maxIndex);

        if (_coolingGauge == null)
        {
            Logger.LogError("CoolingGauge component not found on this object!");
        }

        if (_coolingTimer == null)
        {
            Logger.LogError("CoolingTimer component not found in children of this object!");
        }
    }

    public void SetColdArea(int maxIndex, UnityAction triggerAction)
    {
        _maxIndex = maxIndex;
        _triggerAction = triggerAction;
        Managers.Sound.PlayAMB(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.CoolingMachine.ToString(), gameObject);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            var box = CurrentBox;

            if (box != null)
            {
                if (box.Info.BoxType == Define.BoxType.Normal &&
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
            else if (Managers.MiniGame.CurrentGame.PlayerController.ChangeInteraction((int)MiniGameUnloadInteractionAction.DropBox))
            {
                _triggerAction?.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            if (Managers.MiniGame.CurrentGame.PlayerController.ChangeInteraction((int)MiniGameUnloadInteractionAction.None))
            {
                _triggerAction?.Invoke();
            }
        }
    }

    public bool CanPlaceBox(MiniGameUnloadBox box)
    {
        return CanProcess(box.BoxType) && !_boxList.IsFull;
    }

    public void PlaceBox(MiniGameUnloadBox box)
    {
        ColdBox coldBox = box.GetComponent<ColdBox>();
        if (coldBox != null && _boxList.TryPush(coldBox))
        {        
            _coolingEffect.Play();
            coldBox.EnterCoolingArea(ViewCoolingProcess);
            coldBox.transform.DOMove(transform.position, 1f);

            // 박스 상태 업데이트
            box.transform.SetParent(transform);
            box.SetIsGrab(false);
        }
    }

    public bool CanPickupBox()
    {
        return !_boxList.IsEmpty;
    }

    public MiniGameUnloadBox PickupBox()
    {
        MiniGameUnloadBox box = _boxList.TryPop();
        if (box != null)
        {
            // 이펙트 중지 및 초기화
            _coolingEffect.Pause();
            _coolingEffect.Clear();
            return box;
        }
        return null;
    }

    public void ViewCoolingProcess(float coolingTime)
    {
        // 쿨링 타이머와 게이지 업데이트
        _coolingTimer.SetTimerText(coolingTime);
        _coolingGauge.SetValue(coolingTime);
    }
}
