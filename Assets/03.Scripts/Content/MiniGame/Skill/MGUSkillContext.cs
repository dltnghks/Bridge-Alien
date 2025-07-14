using System;
using System.Collections.Generic;
using System.Linq;

public class MGUSkillContext
{
    public readonly Player Player;
    public readonly MiniGameUnloadPlayer UnloadPlayer;
    public readonly MiniGameUnloadBoxList BoxList;
    public readonly IReadOnlyList<MiniGameUnloadDeliveryPoint> DeliveryPoints;
    public readonly Action RemoveBoxFromPlayerAction;
    public readonly Action<bool> SetCoolingSkillAction;

    public MGUSkillContext(
        Player player,
        MiniGameUnloadPlayer unloadPlayer,
        MiniGameUnloadBoxList boxList,
        IReadOnlyList<MiniGameUnloadBasePoint> allPoints,
        Action removeBoxFromPlayerAction,
        Action<bool> setCoolingSkillAction)
    {
        Player = player;
        UnloadPlayer = unloadPlayer;
        BoxList = boxList;
        DeliveryPoints = allPoints.OfType<MiniGameUnloadDeliveryPoint>().ToList().AsReadOnly();
        RemoveBoxFromPlayerAction = removeBoxFromPlayerAction;
        SetCoolingSkillAction = setCoolingSkillAction;
    }
}