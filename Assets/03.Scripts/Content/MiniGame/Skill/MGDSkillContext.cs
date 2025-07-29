using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MGDSkillContext : ISkillContext
{
    public readonly Player Player;
    public readonly MiniGameDeliveryPlayer DeliveryPlayer;

    public readonly Action OnRocketSkillAction;
    public readonly Action OnRepairSkillAction;

    public MGDSkillContext(Player player, MiniGameDeliveryPlayer deliveryPlayer
    , Action onRocketSkillAction, Action onRepairSkillAction )
    {
        Player = player;
        DeliveryPlayer = deliveryPlayer;
        OnRocketSkillAction = onRocketSkillAction;
        OnRepairSkillAction = onRepairSkillAction;
    }
}
