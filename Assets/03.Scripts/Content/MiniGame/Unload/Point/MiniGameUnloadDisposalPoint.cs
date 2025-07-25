using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MiniGameUnloadDisposalPoint : MiniGameUnloadBasePoint, IBoxPlacePoint
{
    [Header("LampParticle")]
    [SerializeField] private ParticleSystem _lampParticle;

    [Header("Box Drop Area")]
    [SerializeField] private Transform _dropAreaLeftFloor;
    [SerializeField] private Transform _dropAreaRightFloor;
    [SerializeField] private float _moveDistance = 0.3f; // 좌우 이동 거리

    // 폐기하는 구역
    [Header("Setting")]
    [SerializeField] private int _maxIndex = 3;
    [SerializeField] private Transform _dropTransform; // Inspector에서 할당
    [SerializeField] private Vector3 _boxDropPosition;
    private float _boxHeight = 0;
    private float _boxHeightOffset = 0.8f;
    private bool _isDisposing = false;

    private MiniGameUnloadBoxList _boxList = new MiniGameUnloadBoxList();
    private Action _triggerAction;
    private Action<int> _scoreAction;

    public void Start()
    {
        AllowedTypes = new Define.BoxType[] { Define.BoxType.Disposal };
        _boxList.SetBoxList(_maxIndex);

        // 드롭 위치 설정
        _boxDropPosition = _dropTransform != null ? _dropTransform.position : transform.position;
        _boxDropPosition.y += 0.5f; // 드롭 위치 높이 설정
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
                if (_boxList.IsFull && !_isDisposing)
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
        // 처리 중이거나 박스가 없으면 반환
        if (_isDisposing || _boxList.IsEmpty)
            return;

        _isDisposing = true;
        // DisposalBody 에 있는 사이렌 점등 -> DisposalArea 바닥이 열리는 애니메이션
        _lampParticle.Play();

        Sequence disposeSequence = DOTween.Sequence();

        // DropArea가 좌우로 이동
        disposeSequence.Append(
            _dropAreaLeftFloor.DOLocalMoveX(_dropAreaLeftFloor.localPosition.x - _moveDistance, 3f)
        );
        disposeSequence.Join(
            _dropAreaRightFloor.DOLocalMoveX(_dropAreaRightFloor.localPosition.x + _moveDistance, 3f)
        );

        disposeSequence.AppendInterval(0.1f);
        foreach (var box in _boxList.BoxList)
        {
            if (box != null)
            {
                disposeSequence.Join(box.transform.DOLocalMoveY(-5, 1f).OnComplete(() => box.SetInGameActive(false))); // 박스가 폐기되는 위치로 이동
            }
        }

        // DropArea가 원래 위치로 돌아옴
        disposeSequence.Append(
            _dropAreaLeftFloor.DOLocalMoveX(_dropAreaLeftFloor.localPosition.x, 1f)
        );
        disposeSequence.Join(
            _dropAreaRightFloor.DOLocalMoveX(_dropAreaRightFloor.localPosition.x, 1f)
        );

        disposeSequence.OnComplete(() =>
        {
            // 이펙트 중지 및 초기화
            _lampParticle.Pause();
            _lampParticle.Clear();

            _boxList.ClearBoxList();
            _boxHeight = 0; // 높이 초기화

            Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.Discard.ToString(), gameObject);
            _isDisposing = false;
        });
        
        disposeSequence.Play();
    }
}
