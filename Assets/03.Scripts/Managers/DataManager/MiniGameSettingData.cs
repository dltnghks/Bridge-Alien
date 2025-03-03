using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MiniGameUnloadSetting
{
    public int GamePlayTime = 60;
    public int MaxSpawnBoxIndex = 3;
    public float BoxSpawnInterval = 3.0f;
    public float DetectionBoxRadius = 2.0f;
    public float MoveSpeedReductionRatio = 2.0f;
}

[System.Serializable]
public class MiniGameDeliverySetting
{
    public int GamePlayTime;
}