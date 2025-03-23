using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[Serializable]
public class PlayerStatData
{
    public SerializedDictionary<Define.PlayerStatType, int> Stats = new SerializedDictionary<Define.PlayerStatType, int>();

    public PlayerStatData()
    {
        foreach (Define.PlayerStatType type in Enum.GetValues(typeof(Define.PlayerStatType)))
        {
            
            Logger.Log("PlayerStatData");
            Stats[type] = 0; // 기본값 초기화
        }
    }
}



