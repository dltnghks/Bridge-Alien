using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public float PlayerGold = 0;
    public SerializedDictionary<Define.PlayerStatType, int> Stats = new SerializedDictionary<Define.PlayerStatType, int>();
    
    public PlayerData()
    {
        foreach (Define.PlayerStatType type in Enum.GetValues(typeof(Define.PlayerStatType)))
        {
            Logger.Log("PlayerStatData");
            Stats[type] = 0; // 기본값 초기화
            
            // 피로도는 기본 30
            if (type == Define.PlayerStatType.Fatigue)
            {
                Stats[type] = 30;
            }
        }
    }
    
}



