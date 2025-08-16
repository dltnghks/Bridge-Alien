using System;
using System.Collections.Generic;
using System.Resources;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/Data/StageData")]
public class StageDataSO : ScriptableObject
{
    public SerializedDictionary<Define.StageType, StageData> stageData;
}



