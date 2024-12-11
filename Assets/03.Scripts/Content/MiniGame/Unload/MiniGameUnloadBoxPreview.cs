using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnloadBoxPreview : MonoBehaviour
{
    [Header("Game Information")]
    private float _boxSpawnInterval = 0;


    private UIBoxPreview _uiBoxPreview;
    private TimerBase _timer;
   
    private Queue<MiniGameUnloadBox> _previewQueue = new Queue<MiniGameUnloadBox>();
    private MiniGameUnloadBoxSpawnPoint _miniGameUnloadBoxSpawnPoint;

    public void SetBoxPreview(UIBoxPreview uiBoxPreview, float boxSpawnInterval, MiniGameUnloadBoxSpawnPoint miniGameUnloadBoxSpawnPoint)
    {
        if(_timer == null)
            _timer = new TimerBase();
        
        _uiBoxPreview = uiBoxPreview;
        _boxSpawnInterval = boxSpawnInterval;
        _miniGameUnloadBoxSpawnPoint = miniGameUnloadBoxSpawnPoint;

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
        if (!_miniGameUnloadBoxSpawnPoint.BoxList.IsFull)
        {
            _timer.TimerUpdate();
            if (_timer.CurTime <= 0)
            {
                _timer.RestartTimer();
            }
        }
    }
    
    public void CreatePreviewBox()
    {   
        // 박스 생성 및 설정
        GameObject newBoxObj = Managers.Resource.Instantiate("Box", Managers.MiniGame.Root.transform);
        MiniGameUnloadBox newBox = newBoxObj.GetOrAddComponent<MiniGameUnloadBox>();
        newBox.SetRandomInfo();
            
        Vector3 currentScale = newBoxObj.GetOrAddComponent<BoxCollider>().size; 
        currentScale.y = newBox.Info.Size;
        newBoxObj.GetOrAddComponent<BoxCollider>().size = currentScale;
        
        newBox.SetInGameActive(false);
        
        EnqueueBox(newBox);
    }

    public void CreatInGameBox()
    {
        if(_previewQueue.Count > 0){
            MiniGameUnloadBox box = _previewQueue.Peek();
            if (box != null && _miniGameUnloadBoxSpawnPoint.TrySpawnBox(box))
            {
                DequeueBox();
                EnqueueBox(box);
            }
            else
            {
                Logger.LogWarning("Fail CreateInGameBox");
            }
        }
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
