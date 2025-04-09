using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GroundPositionCalculator : MonoBehaviour
{
    [SerializeField] private float mapWidthOffset = .0f;
    [SerializeField] private float mapHeightOffset = .0f;

    [Space(10)]
    [SerializeField] private GameObject groundPrefab;
    private float _mapWidth = .0f;
    private float _mapHeight = .0f;

    private float _resultWidth;
    private float _resultHeight;

    private Rect _resultRect;
    
    private void Initialized()
    {
        if (groundPrefab == null)
        {
            Debug.LogError("Ground Prefab is not assigned.");
            return;
        }

        _mapWidth = groundPrefab.transform.localScale.x;
        _mapHeight = groundPrefab.transform.localScale.z;

        _resultWidth = _mapWidth * mapWidthOffset;
        _resultHeight = _mapHeight * mapHeightOffset;
        
        // groundPrefab의 위치를 기준으로 resultRect 설정
        Vector3 groundPosition = groundPrefab.transform.position;
        // x, y 위치와 width, height를 설정 (3D 공간이므로 y는 z값을 사용)
        _resultRect = new Rect(groundPosition.x - _resultWidth/2, groundPosition.z - _resultHeight/2, 
            _resultWidth, _resultHeight);
    }

    private void OnDrawGizmosSelected()
    {
        Initialized();
    
        Gizmos.color = Color.red;
        Vector3 center = new Vector3(_resultRect.x + _resultRect.width / 2, transform.position.y + 2f, _resultRect.y + _resultRect.height / 2);
        Vector3 size = new Vector3(_resultRect.width, 0.1f, _resultRect.height);
        Gizmos.DrawWireCube(center, size);
    }
}
