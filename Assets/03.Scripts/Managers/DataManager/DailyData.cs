using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class DailyData
{
    public int Date = 0;
    public Define.DialogType EventType = Define.DialogType.TUTORIAL_STORY_01;
    public Define.MiniGameType MiniGameType = Define.MiniGameType.Unload;
}

