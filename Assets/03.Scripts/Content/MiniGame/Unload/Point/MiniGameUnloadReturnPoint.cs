using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MiniGameUnloadReturnPoint : MiniGameUnloadBasePoint, IBoxPlacePoint, IBoxPickupPoint
{

    // 반송 큐 및 박스 풀
    private Queue<MiniGameUnloadBox> _returnBoxQueue = new Queue<MiniGameUnloadBox>();
    private MiniGameUnloadBoxList _boxList;
    [SerializeField] private Transform _dropTransform; // Inspector에서 할당
    [SerializeField] private Transform _returnTransform;
    // 박스 생성 간격
    [SerializeField] private float _spawnInterval = 5f;

    private BoxCollider _boxCollider;
    private Vector3 _boxSpawnPosition;
    private float _boxHeight = 0;
    private float _boxHeightOffset = 0.8f;

    // 초기화
    public void Awake()
    {
        _boxCollider = Utils.GetOrAddComponent<BoxCollider>(gameObject);
        _boxCollider.isTrigger = true;
        _boxSpawnPosition = transform.position;
        _boxList = new MiniGameUnloadBoxList();
        _boxList.SetBoxList(100);
    }
    // 5초마다 큐에서 박스를 드롭 지점으로 이동
    private IEnumerator AutoSpawnBox()
    {
        yield return new WaitForSeconds(_spawnInterval);
        if (_returnBoxQueue.Count > 0)
        {
            MiniGameUnloadBox box = _returnBoxQueue.Dequeue();
            MoveBoxToDropPoint(box);
        }
    }

    // 박스를 드롭 지점으로 이동 후 boxList에 추가
    private void MoveBoxToDropPoint(MiniGameUnloadBox box)
    {
        box.SetInGameActive(true, _returnTransform.position);

        box.transform.DOMove(_dropTransform.position, 1f)
            .OnComplete(() =>
            {
                _boxList.TryPush(box); // boxList에 반환
                _boxHeight += _boxHeightOffset;
                Vector3 spawnPos = _boxSpawnPosition + Vector3.up * _boxHeight;

                // z-ordering, 겹치면 렌더링 충돌나서 z를 살짝 조절, 위로 올라갈수록 앞으로
                spawnPos.z += -(_boxHeight / ((float)_boxList.MaxUnloadBoxIndex * 100f));
                box.SetInGameActive(true, spawnPos);
            });
    }

    // IBoxPlaceable: 다른 포인트에서 박스 반송 요청
    public bool CanPlaceBox(MiniGameUnloadBox box)
    {
        return true;
    }

    public void PlaceBox(MiniGameUnloadBox box)
    {
        box.BoxType = Define.BoxType.Disposal;
        _returnBoxQueue.Enqueue(box);

        StartCoroutine(AutoSpawnBox());
    }

    // 트리거 영역 진입/탈출 시 플레이어 상호작용 변경
    private void OnTriggerStay(Collider coll)
    {
        if (coll.CompareTag("Player") && CanPickupBox())
        {
            OnTriggerAction?.Invoke((int)MiniGameUnloadInteractionAction.PickUpBox);
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            OnTriggerAction?.Invoke((int)MiniGameUnloadInteractionAction.None);
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
            _boxHeight -= _boxHeightOffset;
            return box;
        }
        return null;
    }
    
    // 게임 종료될 때 남은 박스 수만큼 점수 감소
    public void ReturnBoxScore()
    {
        foreach (var box in _boxList.BoxList)
        {
            if (box != null)
            {
                // 박스가 폐기되었을 때 점수 감소
                OnScoreAction?.Invoke(-50);
            }
        }
    }
}
