using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnloadBoxPreview
{
    private UIBoxPreview _uiBoxPreview;
    private TimerBase _timer;
    private float _boxSpawnInterval = 0;
    
    private Queue<MiniGameUnloadBox> _previewQueue = new Queue<MiniGameUnloadBox>();
    
    public void SetBoxPreview(UIBoxPreview uiBoxPreview, float boxSpawnInterval)
    {
        if(_timer == null)
            _timer = new TimerBase();
        
        _uiBoxPreview = uiBoxPreview;
        _boxSpawnInterval = boxSpawnInterval;
        _timer.SetTimer(_uiBoxPreview.UITimer, _boxSpawnInterval, CreatInGameBox);
        
        // 박스 만들기
        for (int i = 0; i < 20; i++)
        {
            CreatePreviewBox();
        }

        DequeueBox();
    }

    public void TimerUpdate()
    {
        _timer.TimerUpdate();
        if (_timer.CurTime <= 0)
        {
            _timer.ResetTimer();
        }
    }
    
    public void CreatePreviewBox()
    {   
        // 가능한 지역 이름 리스트
        string[] regions = { "North", "South", "East", "West", "Central" };

        // 랜덤 생성기
        System.Random random = new System.Random();

        // 랜덤 박스 정보 생성
        Define.BoxType randomBoxType = (Define.BoxType)random.Next(0, (int)Define.BoxType.LargeParcel);
        int randomWeight = random.Next(1, 101); // 무게: 1~100
        int randomSize = random.Next(1, 201);  // 크기: 1~200
        string randomRegion = regions[random.Next(regions.Length)]; // 지역 선택
        bool randomIsFragile = random.Next(0, 2) == 1; // true 또는 false

        // 박스 생성 및 설정
        MiniGameUnloadBox box = new MiniGameUnloadBox();
        box.SetBoxInfo(randomBoxType, randomWeight, randomSize, randomRegion, randomIsFragile);
        
        EnqueueBox(box);
    }

    public void CreatInGameBox()
    {
        DequeueBox();
    }
    
    private void EnqueueBox(MiniGameUnloadBox box)
    {
        _previewQueue.Enqueue(box);
    }
    
    private void DequeueBox()
    {
        if (_previewQueue.Count > 0)
        {
            _previewQueue.Dequeue();
            _uiBoxPreview.SetPreviewBoxInfo(_previewQueue.Peek());
        }
    }
}
