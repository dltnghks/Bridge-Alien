using System;
using System.Collections.Generic;
using System.Linq;

public class MGUSkillContext : ISkillContext
{
    public readonly Player Player;
    public readonly MiniGameUnloadPlayer UnloadPlayer;
    public readonly MiniGameUnloadBoxList BoxList;
    public readonly IReadOnlyList<MiniGameUnloadDeliveryPoint> DeliveryPoints;
    public readonly Action RemoveBoxFromPlayerAction;
    public readonly Action<bool> SetCoolingSkillAction;
    public readonly Action<bool> SetSpeedUpSkillAction;

    public MGUSkillContext(
        Player player,
        MiniGameUnloadPlayer unloadPlayer,
        MiniGameUnloadBoxList boxList,
        IReadOnlyList<MiniGameUnloadBasePoint> allPoints,
        Action removeBoxFromPlayerAction,
        Action<bool> setCoolingSkillAction,
        Action<bool> setSpeedUpSkillAction)
    {
        Player = player;
        UnloadPlayer = unloadPlayer;
        BoxList = boxList;
        DeliveryPoints = allPoints.OfType<MiniGameUnloadDeliveryPoint>().ToList().AsReadOnly();
        RemoveBoxFromPlayerAction = removeBoxFromPlayerAction;
        SetCoolingSkillAction = setCoolingSkillAction;
        SetSpeedUpSkillAction = setSpeedUpSkillAction;
    }
}