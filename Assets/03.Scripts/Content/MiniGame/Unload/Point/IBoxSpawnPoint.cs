using UnityEngine;

public interface IBoxSpawnable
{
    bool CanSpawnBox();
    void SpawnBox(MiniGameUnloadBox box);
}