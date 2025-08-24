using System;
using System.Collections.Generic;
using System.Resources;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public Sprite StageImage;               // 스테이지 이미지
    public string StageName;                // 스테이지 이름 - 해당 스테이지 프리팹 로드할 때 사용
    public bool IsLocked;                   // 초기 잠금, 1스테이지는 false로 설정
    public int RequiredStars;               // 스테이지 해금에 필요한 별 개수
    public int ClearReward;                 // 클리어 보상
    public int[] ClearScoreList;            // 클리어 스코어 - 크기 = 최대 별 개수
    
}



