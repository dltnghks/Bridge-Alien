using System;
using System.Collections.Generic;
using System.Resources;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public enum StagePopupTemplateType
{
    Default,
    Ending,
}

[System.Serializable]
public class StageData
{
    public Sprite StageImage;               // 스테이지 이미지
    public List<Sprite> PopupThumbnails = new List<Sprite>(); // 팝업 썸네일(첫 번째 이미지는 기본 썸네일로 사용)
    public StagePopupTemplateType PopupTemplateType = StagePopupTemplateType.Default; // 팝업 템플릿 타입
    public Sprite EndingLockedThumbnail;    // 엔딩 썸네일 잠금 이미지
    public List<Sprite> EndingLockedThumbnails = new List<Sprite>(); // 엔딩 썸네일별 잠금 이미지
    public int EndingHiddenBoxThreshold = 2; // threshold 이상이면 Good Ending 분기
    public Define.EventDataID EndingEventOnLowHiddenBox = Define.EventDataID.Unknown;
    public Define.EventDataID EndingEventOnHighHiddenBox = Define.EventDataID.Unknown;
    public string StageName;                // 스테이지 이름 - 해당 스테이지 프리팹 로드할 때 사용
    public string StageDescription;         // 스테이지 설명
    public bool IsLocked;                   // 초기 잠금, 1스테이지는 false로 설정
    public int RequiredStars;               // 스테이지 해금에 필요한 별 개수
    public int MinimumWage;                 // 최소 임금
    public int ClearReward;                 // 클리어 보상
    public int[] ClearScoreList;            // 클리어 스코어 - 크기 = 최대 별 개수
    public Define.EventDataID EventID;
    public Define.EventDataID ClearEventID;

    public List<Sprite> GetPopupThumbnails()
    {
        if (PopupThumbnails != null && PopupThumbnails.Count > 0)
        {
            return PopupThumbnails;
        }

        var thumbnails = new List<Sprite>();
        if (StageImage != null)
        {
            thumbnails.Add(StageImage);
        }

        return thumbnails;
    }

    public Define.EventDataID GetEndingEventByHiddenBoxDisposedCount(int hiddenBoxDisposedCount)
    {
        if (hiddenBoxDisposedCount >= EndingHiddenBoxThreshold)
        {
            return EndingEventOnHighHiddenBox;
        }

        return EndingEventOnLowHiddenBox;
    }

    public int GetEndingThumbnailIndexByHiddenBoxDisposedCount(int hiddenBoxDisposedCount)
    {
        return hiddenBoxDisposedCount >= EndingHiddenBoxThreshold ? 1 : 0;
    }

    public Sprite GetEndingLockedThumbnail(int index)
    {
        if (EndingLockedThumbnails != null && EndingLockedThumbnails.Count > index && EndingLockedThumbnails[index] != null)
        {
            return EndingLockedThumbnails[index];
        }

        if (EndingLockedThumbnail != null)
        {
            return EndingLockedThumbnail;
        }

        return StageImage;
    }
}



