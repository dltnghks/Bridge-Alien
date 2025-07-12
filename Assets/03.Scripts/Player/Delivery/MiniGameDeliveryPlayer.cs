using System.Collections.Generic;
using UnityEngine;

public class MiniGameDeliveryPlayer : Player
{
    private DamageHandler _damageHandler;
    
    public void Start()
    {
        base.Start();
        _damageHandler = GetComponent<DamageHandler>();
    }
    private void OnTriggerEnter(Collider other)
    {
        
    }
}