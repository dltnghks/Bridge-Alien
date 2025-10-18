using System;
using System.Collections.Generic;
using System.Resources;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/Data/StageData")]
public class StageDataSO : ScriptableObject
{
    public SerializedDictionary<Define.ChapterType, StageData> stageData;

    public StageData GetStageData(Define.ChapterType stageType)
    {
        if (stageData.ContainsKey(stageType))
        {
            return stageData[stageType];
        }

        Logger.LogError($"{stageType} is not stage data");

        return null;
    }
}



