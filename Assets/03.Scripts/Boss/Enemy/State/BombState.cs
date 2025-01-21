using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombState : MonoBehaviour
{
    public MiniGameDeliveryEnemy Enemy { get; set; }
    
    public BombState(MiniGameDeliveryEnemy enemy)
    {
        Enemy = enemy;
    }
    
    public void EnterState()
    {
        Debug.Log("Enter Charge State");
    }

    public void UpdateState()
    {
        // 돌진 중 동작 로직 (추적 등)
    }

    public void ExitState()
    {
        Debug.Log("Exit Charge State");
    }
}
