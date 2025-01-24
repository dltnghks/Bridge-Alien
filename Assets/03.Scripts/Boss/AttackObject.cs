using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AttackObject : MonoBehaviour
{
    [SerializeField] private float damage = 10f;    // 공격력
    private bool hasHit = false;                    // 이미 타격했는지 여부

    private void Start()
    {
        // Rigidbody 설정
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;                      // 중력 비활성화
        rb.isKinematic = false;                     // 물리 영향 받도록 설정
        
        // 트리거 콜라이더로 설정
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;  // 이미 타격했다면 무시

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                //player.TakeDamage(damage);
                hasHit = true;
                
                // 타격 후 오브젝트 제거
                Destroy(gameObject);
            }
        }
    }

    // 데미지 설정 함수
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
}
