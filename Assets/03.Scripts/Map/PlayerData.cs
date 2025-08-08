using System;
using System.Collections.Generic;
using System.Resources;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int PlayerGold = 0;
    public SerializedDictionary<Define.PlayerStatsType, int> Stats = new SerializedDictionary<Define.PlayerStatsType, int>();
    // 플레이어가 고른 선택지
    // -1인 경우 아직 선택지를 고르지 않은 상태
    public int ChoiceNumber = -1;

    // 스킬 데이터, level이 0이면 스킬 해금X, 1이상부터 해금
    public SerializedDictionary<Define.MiniGameSkillType, int> MiniGameUnloadSkillLevel = new SerializedDictionary<Define.MiniGameSkillType, int>();

    public PlayerData()
    {
        // 스탯 초기화
        foreach (Define.PlayerStatsType type in Enum.GetValues(typeof(Define.PlayerStatsType)))
        {
            Logger.Log("PlayerStatData");
            Stats[type] = 0; // 기본값 초기화

            // 피로도는 기본 30
            if (type == Define.PlayerStatsType.Fatigue)
            {
                Stats[type] = 30;
            }
        }

        // 하차 게임 스킬 데이터 초기화
        foreach (Define.MiniGameSkillType skillType in Enum.GetValues(typeof(Define.MiniGameSkillType)))
        {
            MiniGameUnloadSkillLevel[skillType] = 1;
        }
    }
    
}



