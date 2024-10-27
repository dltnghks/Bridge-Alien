using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NPC : MonoBehaviour
{
    private Camera mainCamera;
    
    private void Start()
    {
        // 메인 카메라 참조
        mainCamera = Camera.main;
        
        if (mainCamera == null)
        {
            Debug.LogError("메인 카메라를 찾을 수 없습니다!");
            enabled = false;
        }
    }
    
    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            // 카메라를 향해 회전
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}