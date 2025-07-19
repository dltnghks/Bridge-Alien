using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MiniGameUnloadDisposalPoint : MiniGameUnloadBasePoint, IBoxPlacePoint
{
    // 폐기하는 구역
    [Header("Setting")]
    private int _maxIndex = 3;

    [SerializeField] private Transform _dropTransform; // Inspector에서 할당
    [SerializeField] private Vector3 _boxDropPosition;
    private float _boxHeight = 0;
    private float _boxHeightOffset = 0.8f;

    private MiniGameUnloadBoxList _boxList = new MiniGameUnloadBoxList();
    private Action _triggerAction;
    private Action<int> _scoreAction;

    public void Start()
    {
        AllowedTypes = new Define.BoxType[] { Define.BoxType.Disposal };
        _boxList.SetBoxList(_maxIndex);

        // 드롭 위치 설정
        _boxDropPosition = _dropTransform != null ? _dropTransform.position : transform.position;
    }

    public void SetDisposalPoint(Action triggerAction)
    {
        _triggerAction = triggerAction;
    }

    public override bool CanProcess(Define.BoxType boxType)
    {
        return base.CanProcess(boxType);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            if (Managers.MiniGame.CurrentGame.PlayerController.ChangeInteraction((int)MiniGameUnloadInteractionAction.DropBox))
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
        if (box != null && _boxList.TryPush(box))
        {
            // 박스 상태 업데이트
            box.transform.SetParent(transform);
            box.SetIsGrab(false);

            box.transform.DOMove(_boxDropPosition + (Vector3.up * _boxHeight * _boxHeightOffset), 1f).OnComplete(() =>
            {
                if (_boxList.IsFull)
                {
                    DisposeAllBoxes();
                }
            });

            // 드롭 위치 높이 업데이트
            _boxHeight++;
        }
    }

    // 리스트에 있는 박스 모두 폐기
    public void DisposeAllBoxes()
    {
        foreach (var box in _boxList.BoxList)
        {
            if (box != null)
            {
                box.SetInGameActive(false);
            }
        }
        _boxList.ClearBoxList();
        _boxHeight = 0; // 높이 초기화

        Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.Discard.ToString(), gameObject);
    }
}
