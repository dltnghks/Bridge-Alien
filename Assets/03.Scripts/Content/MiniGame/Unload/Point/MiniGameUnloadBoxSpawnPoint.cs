using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameUnloadBoxSpawnPoint : MiniGameUnloadBasePoint, IBoxSpawnPoint, IBoxPickupPoint
{
    private Vector3 _boxSpawnPosition;
    private TimerBase _timer;
    private float _boxHeight = 1.0f;
    private float _boxHeightOffset = 0.8f;

    [Header("Spawn Setting")]
    [SerializeField]
    private float _boxSpawnInterval = 5.0f;
    [SerializeField]
    private int _maxSpawnBoxIndex = 3;
    [SerializeField]
    private Define.BoxType[] _spawnBoxType = new Define.BoxType[] { Define.BoxType.Common, Define.BoxType.Cold, Define.BoxType.Fragile };
    public MiniGameUnloadBoxList BoxList { get; set; }

    public void SetBoxSpawnPoint()
    {
        Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.Truck.ToString(), gameObject);

        _boxSpawnPosition = transform.position;
        BoxList = new MiniGameUnloadBoxList();
        BoxList.SetBoxList(_maxSpawnBoxIndex);
        InitTimer();
    }

    private void InitTimer()
    {
        if (_timer == null)
            _timer = new TimerBase();

        _timer.OffTimer();
        _timer.SetTimer(_boxSpawnInterval);
        _timer.OnEndTime = SpawnBox;
    }

    public void Update()
    {
        if (!Managers.MiniGame.CurrentGame.IsActive || Managers.MiniGame.CurrentGame.IsPause)
            return;

        if (CanSpawnBox())
        {
            _timer.TimerUpdate();
            if (_timer.CurTime <= 0)
            {
                _timer.RestartTimer();
            }
        }
    }

    private void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            OnTriggerAction?.Invoke((int)MiniGameUnloadInteractionAction.PickUpBox);
        }
    }
    
    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            OnTriggerAction?.Invoke((int)MiniGameUnloadInteractionAction.None);
        }
       
    }

    public bool CanSpawnBox()
    {
        return !BoxList.IsFull;
    }

    public void SpawnBox()
    {
        Logger.Log("SpawnBox");
        if (!CanSpawnBox())
        {
            return;
        }
        
        int randomIndex = Random.Range(0, _spawnBoxType.Length);
        Define.BoxType boxType = _spawnBoxType[randomIndex];

        GameObject newBoxObj = Managers.Resource.Instantiate($"MiniGameUnloadBox/{boxType}Box", transform);
        MiniGameUnloadBox newBox = newBoxObj.GetOrAddComponent<MiniGameUnloadBox>();

        newBox.SetRandomInfo();
        newBox.SetInGameActive(false);

        if (!newBox.gameObject.activeSelf && BoxList.TryPush(newBox))
        {
            _boxHeight += _boxHeightOffset;
            Vector3 spawnPos = _boxSpawnPosition + Vector3.up * _boxHeight;

            // z-ordering, 겹치면 렌더링 충돌나서 z를 살짝 조절, 위로 올라갈수록 앞으로
            spawnPos.z += -(_boxHeight / ((float)BoxList.MaxUnloadBoxIndex * 100f));

            newBox.SetSpawnBox(spawnPos);
        }
    }

    public bool CanPickupBox()
    {
        return !BoxList.IsEmpty;        
    }

    public MiniGameUnloadBox PickupBox()
    {
        MiniGameUnloadBox box = BoxList.TryPop();
        Logger.Log(box);
        if (box != null)
        {
            _boxHeight -= _boxHeightOffset;
            return box;
        }
        else
        {
            return null;
        }
    }
}
