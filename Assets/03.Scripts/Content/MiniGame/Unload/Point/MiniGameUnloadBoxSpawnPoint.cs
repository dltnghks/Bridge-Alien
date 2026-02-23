using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnloadBoxSpawnPoint : MiniGameUnloadBasePoint, IBoxSpawnPoint, IBoxPickupPoint
{
    private Vector3 _boxSpawnPosition;
    private TimerBase _timer;
    private float _boxHeight = 1.0f;
    private float _boxHeightOffset = 0.8f;
    private Define.BoxType _lastSpawnedBoxType = Define.BoxType.Unknown;

    [Header("Spawn Setting")]
    [SerializeField] private float _boxSpawnInterval = 5.0f;
    [SerializeField] private int _maxSpawnBoxIndex = 3;
    [SerializeField] private Define.BoxType[] _spawnBoxType = new Define.BoxType[] { Define.BoxType.Common, Define.BoxType.Cold, Define.BoxType.Fragile };

    public MiniGameUnloadBoxList BoxList { get; set; }

    public void SetBoxSpawnPoint()
    {
        Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.Truck.ToString(), gameObject);

        _boxSpawnPosition = transform.position;
        BoxList = new MiniGameUnloadBoxList();
        BoxList.SetBoxList(_maxSpawnBoxIndex);
        _lastSpawnedBoxType = Define.BoxType.Unknown;
        InitTimer();
    }

    private void InitTimer()
    {
        if (_timer == null)
        {
            _timer = new TimerBase();
        }

        _timer.OffTimer();
        _timer.SetTimer(_boxSpawnInterval, 0.5f);
        _timer.OnEndTime = SpawnBox;
    }

    public void Update()
    {
        if (!Managers.MiniGame.IsAcitveObject)
        {
            return;
        }

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

        bool shouldSpawnHidden = ShouldSpawnHiddenBox();
        Define.BoxType boxType = GetRandomBoxType();

        MiniGameUnloadBox newBox = CreateBoxInstance(boxType, shouldSpawnHidden);
        if (newBox == null)
        {
            CancelHiddenBoxReservation(shouldSpawnHidden);
            return;
        }

        if (shouldSpawnHidden)
        {
            newBox.SetHiddenInfo();
        }
        else
        {
            newBox.SetRandomInfo();
        }
        newBox.SetInGameActive(false);

        if (!newBox.gameObject.activeSelf && BoxList.TryPush(newBox))
        {
            _boxHeight += _boxHeightOffset;
            Vector3 spawnPos = _boxSpawnPosition + Vector3.up * _boxHeight;
            spawnPos.z += -(_boxHeight / ((float)BoxList.MaxUnloadBoxIndex * 100f));

            newBox.SetSpawnBox(spawnPos);
            _lastSpawnedBoxType = boxType;
            NotifyBoxSpawned(newBox);
        }
        else
        {
            CancelHiddenBoxReservation(shouldSpawnHidden);
        }
    }

    public void SpawnBox(BoxTypeDecision boxTypeDecision)
    {
        Logger.Log("SpawnBox");
        if (!CanSpawnBox())
        {
            return;
        }

        bool shouldSpawnHidden = ShouldSpawnHiddenBox();

        Define.BoxType boxType = boxTypeDecision.BoxType;
        if (boxType == Define.BoxType.Unknown)
        {
            boxType = GetRandomBoxType();
        }
        else if (ShouldAvoidFragileOnTop(boxType))
        {
            boxType = GetRandomBoxType(excludeFragile: true);
        }
        else if (ShouldAvoidColdOnStage3(boxType))
        {
            boxType = GetRandomBoxType(excludeCold: true);
        }

        MiniGameUnloadBox newBox = CreateBoxInstance(boxType, shouldSpawnHidden);
        if (newBox == null)
        {
            CancelHiddenBoxReservation(shouldSpawnHidden);
            return;
        }

        if (shouldSpawnHidden)
        {
            newBox.SetHiddenInfo();
        }
        else
        {
            newBox.SetRandomInfo();
            newBox.SetRegion(boxTypeDecision.BoxRegion);
        }
        newBox.SetInGameActive(false);

        if (!newBox.gameObject.activeSelf && BoxList.TryPush(newBox))
        {
            _boxHeight += _boxHeightOffset;
            Vector3 spawnPos = _boxSpawnPosition + Vector3.up * _boxHeight;
            spawnPos.z += -(_boxHeight / ((float)BoxList.MaxUnloadBoxIndex * 100f));

            newBox.SetSpawnBox(spawnPos);
            _lastSpawnedBoxType = boxType;
            NotifyBoxSpawned(newBox);
        }
        else
        {
            CancelHiddenBoxReservation(shouldSpawnHidden);
        }
    }

    private MiniGameUnloadBox CreateBoxInstance(Define.BoxType boxType, bool hidden)
    {
        string prefabName = hidden ? "CommonBox" : $"{boxType}Box";
        GameObject newBoxObj = Managers.Resource.Instantiate($"MiniGameUnloadBox/{prefabName}", transform);
        if (newBoxObj == null)
        {
            return null;
        }

        return newBoxObj.GetOrAddComponent<MiniGameUnloadBox>();
    }

    private bool ShouldSpawnHiddenBox()
    {
        if (Managers.MiniGame.CurrentGame is MiniGameUnload miniGameUnload)
        {
            return miniGameUnload.TryReserveHiddenBoxSpawn(this);
        }

        return false;
    }

    private void NotifyBoxSpawned(MiniGameUnloadBox box)
    {
        if (Managers.MiniGame.CurrentGame is MiniGameUnload miniGameUnload)
        {
            miniGameUnload.NotifyBoxSpawned(this, box);
        }
    }

    private void CancelHiddenBoxReservation(bool hiddenReserved)
    {
        if (!hiddenReserved)
        {
            return;
        }

        if (Managers.MiniGame.CurrentGame is MiniGameUnload miniGameUnload)
        {
            miniGameUnload.CancelReservedHiddenBoxSpawn();
        }
    }

    private Define.BoxType GetRandomBoxType(bool excludeFragile = false, bool excludeCold = false)
    {
        bool avoidFragile = excludeFragile || ShouldAvoidFragileOnTop(Define.BoxType.Fragile);
        bool avoidCold = excludeCold || ShouldAvoidColdOnStage3(Define.BoxType.Cold);
        List<Define.BoxType> candidates = new List<Define.BoxType>();

        foreach (var boxType in _spawnBoxType)
        {
            if (avoidFragile && boxType == Define.BoxType.Fragile)
            {
                continue;
            }
            if (avoidCold && boxType == Define.BoxType.Cold)
            {
                continue;
            }
            candidates.Add(boxType);
        }

        if (candidates.Count == 0)
        {
            int randomIndex = Random.Range(0, _spawnBoxType.Length);
            return _spawnBoxType[randomIndex];
        }

        int index = Random.Range(0, candidates.Count);
        return candidates[index];
    }

    private bool ShouldAvoidFragileOnTop(Define.BoxType nextType)
    {
        if (Managers.Stage == null || Managers.Stage.CurrentStageType != Define.ChapterType.CH1)
        {
            return false;
        }

        if (nextType != Define.BoxType.Fragile)
        {
            return false;
        }

        MiniGameUnloadBox topBox = BoxList?.Peek();
        return topBox != null && topBox.BoxType == Define.BoxType.Fragile;
    }

    private bool ShouldAvoidColdOnStage3(Define.BoxType nextType)
    {
        if (Managers.Stage == null || Managers.Stage.CurrentStageType != Define.ChapterType.CH3)
        {
            return false;
        }

        if (nextType != Define.BoxType.Cold)
        {
            return false;
        }

        if (BoxList == null || BoxList.BoxList == null)
        {
            return false;
        }

        int coldCount = 0;
        for (int i = 0; i < BoxList.BoxList.Count; i++)
        {
            MiniGameUnloadBox box = BoxList.BoxList[i];
            if (box != null && box.BoxType == Define.BoxType.Cold)
            {
                coldCount++;
                if (coldCount >= 2)
                {
                    return true;
                }
            }
        }

        return false;
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

        return null;
    }
}
