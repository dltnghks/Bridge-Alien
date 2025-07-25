using UnityEngine;

public interface IBoxSpawnPoint
{
    bool CanSpawnBox();
    void SpawnBox(MiniGameUnloadBox box);
}