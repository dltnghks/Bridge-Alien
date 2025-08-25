using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Rendering;

public class MiniGameSettingBase
{
    public float GamePlayTime = 60.0f;
}

[System.Serializable]
public class MiniGameUnloadSetting :MiniGameSettingBase
{
    public float DetectionBoxRadius = 2.0f;
    public float MoveSpeedReductionRatio = 2.0f;
    public int MaxColdAreaBoxIndex = 1;
}

[System.Serializable]
public class MiniGameDeliverySetting : MiniGameSettingBase 
{
}

