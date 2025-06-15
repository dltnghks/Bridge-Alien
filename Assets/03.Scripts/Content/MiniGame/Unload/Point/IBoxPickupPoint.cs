using UnityEngine;

public interface IBoxPickupable
{
    bool CanPickupBox();
    MiniGameUnloadBox PickupBox();
}
