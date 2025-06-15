using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnloadBoxPreview : MonoBehaviour
{
    [Header("Game Information")]
    private float _boxSpawnInterval = 0;

    private GameObject[] _boxPrefabList;
    private TimerBase _timer;
   
    private Queue<MiniGameUnloadBox> _previewQueue = new Queue<MiniGameUnloadBox>();
    private MiniGameUnloadBoxSpawnPoint _miniGameUnloadBoxSpawnPoint;

    public void SetBoxPreview(float boxSpawnInterval, MiniGameUnloadBoxSpawnPoint miniGameUnloadBoxSpawnPoint, GameObject[] boxPrefabList)
    {
        if (_timer == null)
            _timer = new TimerBase();

        _boxSpawnInterval = boxSpawnInterval;
        _miniGameUnloadBoxSpawnPoint = miniGameUnloadBoxSpawnPoint;

        _timer.OffTimer();
        _timer.SetTimer(null, _boxSpawnInterval, CreatInGameBox);

        _previewQueue.Clear();
        _previewQueue.Enqueue(null);

        _boxPrefabList = boxPrefabList;
        // 박스 만들기
        CreatePreviewBox();

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
        // 박스 생성 및 설정, 오브젝트 풀에서 5개씩 가져와서 queue에다 넣기
        for (int i = 0; i < 5; i++)
        {
            int randomIndex = Random.Range(0, _boxPrefabList.Length);  
            GameObject newBoxObj = Managers.Resource.Instantiate(_boxPrefabList[randomIndex], Managers.MiniGame.Root.transform);

            MiniGameUnloadBox newBox = newBoxObj.GetOrAddComponent<MiniGameUnloadBox>();

            newBox.SetRandomInfo();
            newBox.SetInGameActive(false);

            EnqueueBox(newBox);
        }
    }

    public void CreatInGameBox()
    {
        // 여유 박스가 없으면 생성해서 넣기
        if (_previewQueue.Count <= 1)
        {
            CreatePreviewBox();
        }
        
        if(_previewQueue.Count > 0){
            MiniGameUnloadBox box = _previewQueue.Peek();
            if (box != null && _miniGameUnloadBoxSpawnPoint.CanSpawnBox())
            {
                _miniGameUnloadBoxSpawnPoint.SpawnBox(box);
                DequeueBox();
            }
            else
            {
                Logger.LogWarning("Fail CreateInGameBox");
            }
        }
        else
        {
            Logger.LogWarning("Fail CreateInGameBox");
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

            }
            else
            {
                Logger.LogWarning("No preview box available");
            }
            
            return box;
        }
        
        Logger.Log("No preview box found");
        return null;
    }
}
