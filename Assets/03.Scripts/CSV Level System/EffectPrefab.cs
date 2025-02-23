using UnityEngine;

public class EffectPrefab : MonoBehaviour
{
    private float colliderDisableTime = 0.1f;    // 충돌체 비활성화까지의 시간
    private float destroyTime = 2f;            // 오브젝트 파괴까지의 시간

    private void Start()
    {
        // 1초 후에 충돌체 비활성화
        Invoke(nameof(DisableCollider), colliderDisableTime);
        
        // 2초 후에 오브젝트 파괴
        Destroy(gameObject, destroyTime);
    }

    private void DisableCollider()
    {
        // 모든 충돌체 비활성화
        var colliders = GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }
} 