using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MiniGameDeliveryPlayer : Player
{
    // Player 데이터에 관련된 모든 클래스는 이 클래스 내부에서 처리한다.
    private DamageHandler _damageHandler;
    
    public void Start()
    {
        base.Start();
        
        _damageHandler = GetComponent<DamageHandler>();
        
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag($"Enemy"))
        {
            // 이 곳에서 플레이어 충돌에 대한 처리를 하면 됨.
            
            // 1. 플레이어 오토바이 내구도 감소. ( 임시 매직 넘버 )
            _damageHandler.AddDamageRate(0.2f);
            // 2. 플레이어는 N초간 무적 상태에 돌입.
            
            // 3. SFX 재생
        }
    }
}