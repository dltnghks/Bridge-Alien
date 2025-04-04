using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPositionPicker : MonoBehaviour
{
    private float _groundMinWidth;
    private float _groundMaxWidth;
    
    private float _groundMinHeight;
    private float _groundMaxHeight;

    private void Awake()
    {
        _groundMinWidth = transform.position.x - (transform.localScale.x * 0.5f);
        _groundMaxWidth = transform.position.x + (transform.localScale.x * 0.5f);
        
        _groundMinHeight = transform.position.z - (transform.localScale.z * 0.5f);
        _groundMaxHeight = transform.position.z + (transform.localScale.z * 0.5f);
    }
    
    public float GetPositionX(float xPercent)
    {
        return GetPosition(xPercent, 0f).x;
    }
    
    public float GetPositionY(float yPercent)
    {
        return GetPosition(0f, yPercent).y;
    }
    
    public Vector2 GetPosition(float xPercent, float yPercent)
    {
        float x = Mathf.Lerp(_groundMinWidth, _groundMaxWidth, xPercent);
        float y = Mathf.Lerp(_groundMinHeight, _groundMaxHeight, yPercent);
        return new Vector2(x, y);
    }

    public void OnDrawGizmosSelected()
    {
        // Awake가 호출되기 전에도 Gizmo가 그려질 수 있도록 값 계산
        float minWidth = transform.position.x - (transform.localScale.x * 0.5f);
        float maxWidth = transform.position.x + (transform.localScale.x * 0.5f);
        float minHeight = transform.position.z - (transform.localScale.z * 0.5f);
        float maxHeight = transform.position.z + (transform.localScale.z * 0.5f);
        
        // 경계선 그리기
        Gizmos.color = Color.green;
        
        // 사각형 그리기
        Vector3 corner1 = new Vector3(minWidth, transform.position.y, minHeight);
        Vector3 corner2 = new Vector3(maxWidth, transform.position.y, minHeight);
        Vector3 corner3 = new Vector3(maxWidth, transform.position.y, maxHeight);
        Vector3 corner4 = new Vector3(minWidth, transform.position.y, maxHeight);
        
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner4);
        Gizmos.DrawLine(corner4, corner1);
        
        // 라벨 표시
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(corner1, $"Min: ({minWidth:F2}, {minHeight:F2})");
        UnityEditor.Handles.Label(corner3, $"Max: ({maxWidth:F2}, {maxHeight:F2})");
    }
}
