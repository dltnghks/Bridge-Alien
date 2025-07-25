using UnityEngine;

public interface IBoxPickupPoint
{
    bool CanPickupBox();
    MiniGameUnloadBox PickupBox();
}
