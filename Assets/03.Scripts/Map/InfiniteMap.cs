using System;
using UnityEngine;

public class InfiniteMap : MonoBehaviour
{
    [Header("맵 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Vector3 moveDirection = Vector3.right;
    [SerializeField] private float blockSize = 50f;

    private float _maxDistance = 0f;
    [SerializeField] private float totalDistance = 0f;

    private Transform[] _blocks;
    private bool _isInitialized = false;
    private float _threshold;
    
    private const int BLOCK_COUNT = 5; // 고정값으로 명시

    private Action<float> onUpdateDistance;

    public void Initialize(float maxDist, Action<float> updateAction)
    {
        _blocks = new Transform[BLOCK_COUNT];
        
        // 블록 초기화
        for (int i = 0; i < BLOCK_COUNT; i++)
        {
            _blocks[i] = transform.GetChild(i);
            if (_blocks[i] == null)
            {
                Logger.LogError($"블록 {i}를 찾을 수 없습니다!");
                this.enabled = false;
                return;
            }
        }

        // blockSize = 50
        // bS * 2.5 = 125f
        _threshold = blockSize * 2.5f;

        // 연속 배치 : -100, -50, 0, 50, 100
        for (int i = 0; i < BLOCK_COUNT; i++)
        {
            float xPosition = (i - 2) * blockSize; // -2, -1, 0, 1, 2
            _blocks[i].localPosition = moveDirection * xPosition;
        }

        _maxDistance = maxDist;
        onUpdateDistance = updateAction;
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized) return;
        
        // 최대 거리 도달 시 정지
        if (totalDistance >= _maxDistance)
        {
            this.enabled = false;
            return;
        }

        // 거리 업데이트
        float deltaMove = moveSpeed * Time.deltaTime;
        totalDistance += deltaMove;
        onUpdateDistance?.Invoke(totalDistance);

        // 블록 이동 및 재배치
        MoveAndRepositionBlocks(deltaMove);
    }

    private void MoveAndRepositionBlocks(float deltaMove)
    {
        Vector3 movement = moveDirection * deltaMove;
        
        for (int i = 0; i < _blocks.Length; i++)
        {
            // 블록 이동
            _blocks[i].localPosition += movement;
            
            float currentPos = Vector3.Dot(_blocks[i].localPosition, moveDirection);
            
            if (currentPos > _threshold)
                _blocks[i].localPosition -= moveDirection * (blockSize * BLOCK_COUNT);
            else if (currentPos < -_threshold)
                _blocks[i].localPosition += moveDirection * (blockSize * BLOCK_COUNT);
        }
    }

    // 런타임에 속도 변경
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = Mathf.Max(0, newSpeed);
    }

    public float GetProgress()
    {
        return _maxDistance > 0 ? Mathf.Clamp01(totalDistance / _maxDistance) : 0f;
    }

    public void ResetMap()
    {
        if (!_isInitialized) return;
        
        totalDistance = 0f;
        
        for (int i = 0; i < BLOCK_COUNT; i++)
        {
            float xPosition = (i - 2) * blockSize;
            _blocks[i].localPosition = moveDirection * xPosition;
        }
        
        this.enabled = true;
        Debug.Log("맵 리셋 완료");
    }
}