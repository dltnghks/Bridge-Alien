using UnityEngine;

public interface IBoxPlaceable
{
    bool CanPlaceBox(MiniGameUnloadBox box);
    void PlaceBox(MiniGameUnloadBox box);
}
