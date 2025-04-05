using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GroundManager : MonoBehaviour
{
    [SerializeField] private List<GroundPositionPicker> groundPositionPickers;

    [SerializeField] private float startRatio;
    [SerializeField] private float endRatio;
    
    private float _startWidth;
    private float _endWidth;

    private void Start()
    {
        if (groundPositionPickers != null)
        {
            _startWidth = groundPositionPickers[0].GetPosition(startRatio, startRatio).x;
            _endWidth = groundPositionPickers[^1].GetPosition(endRatio, endRatio).x;
        }
    }

    private void OnDrawGizmosSelected()
    {
        _startWidth = groundPositionPickers[0].GetPosition(startRatio, startRatio).x;
        _endWidth = groundPositionPickers[^1].GetPosition(endRatio, endRatio).x;
        
        Vector3 startPosition = new Vector3(_startWidth, 1f, transform.position.z);
        Vector3 endPosition = new Vector3(_endWidth, 1f, transform.position.z);
        
        Debug.DrawLine(startPosition, endPosition, Color.black);
        
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.Label(startPosition, $"Start Point: ({startPosition.x:F2}, {startPosition.z:F2})");
        UnityEditor.Handles.Label(endPosition, $"End Point : ({endPosition.x:F2}, {endPosition.z:F2})");
    }
}
