using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int PlayerGold = 0;
    public SerializedDictionary<Define.PlayerStatType, int> Stats = new SerializedDictionary<Define.PlayerStatType, int>();
    // 플레이어가 고른 선택지
    // -1인 경우 아직 선택지를 고르지 않은 상태
    public int ChoiceNumber = -1;
    
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



