 using System;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEngine.Serialization;

public class InfiniteMap : MonoBehaviour
{
    [Header("맵 설정")]                                               
    [SerializeField] private float moveSpeed = 5f;                          // 맵 이동 속도
    [SerializeField] private Vector3 moveDirection = Vector3.right;         // 이동 방향
    [SerializeField] private float blockSize = 50f;                         // 각 블록의 크기

    // 최대 이동 거리
    private float _maxDistance = .0f;
    // 누적 이동 거리의 합
    [SerializeField] private float totalDistance = .0f;

    private Transform[] _blocks;                                             // 맵 블록들을 저장할 배열
    private bool _isInitialized = false;                                     // 초기화 완료 여부
    private float _threshold;                                                // 블록 재배치 임계값 (블록 크기의 1.5배가 넘어가면 재배치됩니다 넉넉해야 깜빡이는 현상이 없습니다)

    private Action<float> onUpdateDistance;

    //~ InitializeBlocks() 메서드는 맵 블록들을 초기화합니다.
    public void InitializeMap(float maxDist, Action<float> updateAction)
    {
        _blocks = new Transform[5];
        
        int count = transform.childCount;
        
        for (int i = 0; i < count; i++)
        {
            _blocks[i] = transform.GetChild(i);
            if (_blocks[i] == null)
            {
                Logger.LogError($"블록 {i}를 찾을 수 없습니다!");
                this.enabled = false;
                return;
            }
        }

        if (blockSize <= 0)
            blockSize = _blocks[0].localScale.x;

        _threshold = blockSize * 1.5f;

        for (int i = 0; i < count; i++)
            _blocks[i].localPosition = moveDirection * ((i - 1) * blockSize);

        _maxDistance = maxDist;
        onUpdateDistance = updateAction;
        
        _isInitialized = true;
    }

    //~ Update() 메서드에서는 맵 블록들을 이동시키고 재배치합니다.
    private void Update()
    {               
        if (!_isInitialized) return;
        
        // 누적 거리 합
        totalDistance += moveSpeed * Time.deltaTime;
        onUpdateDistance?.Invoke(totalDistance);
        
        if (totalDistance >= _maxDistance)
            return;

        for (int i = 0; i < _blocks.Length; i++)                                         
        {
            _blocks[i].localPosition += moveDirection * (moveSpeed * Time.deltaTime);    
            
            float currentPos = Vector3.Dot(_blocks[i].localPosition, moveDirection);

            if (currentPos >= _threshold)
                _blocks[i].localPosition -= moveDirection * (blockSize * 3);
            else if (currentPos <= -_threshold)
                _blocks[i].localPosition += moveDirection * (blockSize * 3);
        }
    }
}