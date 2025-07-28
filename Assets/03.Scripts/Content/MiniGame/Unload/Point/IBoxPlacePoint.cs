using UnityEngine;

public interface IBoxPlacePoint
{
    bool CanPlaceBox(MiniGameUnloadBox box);
    void PlaceBox(MiniGameUnloadBox box);
}
