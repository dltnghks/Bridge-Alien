using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnloadBoxPreview : MonoBehaviour
{
    private UIBoxPreview _uiBoxPreview;
    private TimerBase _timer;
    private float _boxSpawnInterval = 0;
    private Vector3 _boxSpawnPosition;
    
    private Queue<MiniGameUnloadBox> _previewQueue = new Queue<MiniGameUnloadBox>();
    
    public void SetBoxPreview(UIBoxPreview uiBoxPreview, float boxSpawnInterval)
    {
        if(_timer == null)
            _timer = new TimerBase();
        
        _uiBoxPreview = uiBoxPreview;
        _boxSpawnInterval = boxSpawnInterval;
        _timer.OffTimer();
        _timer.SetTimer(_uiBoxPreview.UITimer, _boxSpawnInterval, CreatInGameBox);
        
        _previewQueue.Clear();
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
            _timer.RestartTimer();
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
        GameObject newBoxObj = Managers.Resource.Instantiate("Box", Managers.MiniGame.Root.transform);
        newBoxObj.SetActive(false);
        MiniGameUnloadBox newBox = newBoxObj.GetOrAddComponent<MiniGameUnloadBox>();
        newBox.SetBoxInfo(randomBoxType, randomWeight, randomSize, randomRegion, randomIsFragile);
        
        EnqueueBox(newBox);
    }

    public void CreatInGameBox()
    {
        MiniGameUnloadBox box = DequeueBox();
        box.gameObject.SetActive(true);
    }
    
    private void EnqueueBox(MiniGameUnloadBox box)
    {
        _previewQueue.Enqueue(box);
    }
    
    private MiniGameUnloadBox DequeueBox()
    {
        if (_previewQueue.Count > 0)
        {
            MiniGameUnloadBox box = _previewQueue.Dequeue();
            
            if (_previewQueue.Count > 0)
            {
                _uiBoxPreview.SetPreviewBoxInfo(_previewQueue.Peek());
            }
            else
            {
                Debug.LogWarning("No preview box available");
            }
            
            return box;
        }
        
        Debug.Log("No preview box found");
        return null;
    }
}
