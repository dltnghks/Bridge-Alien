using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct MiniGameUnloadDeliveryPointInfo
{
    public Define.BoxRegion Region;

    public MiniGameUnloadDeliveryPointInfo(Define.BoxRegion region)
    {
        Region = region;
    }
}

public class MiniGameUnloadDeliveryPoint : MiniGameUnloadBasePoint, IBoxPlacePoint
{
    [SerializeField] private MiniGameUnloadDeliveryPointInfo _info;
    [SerializeField] private TextMeshPro _ViewDeliveryRegionText;
    [SerializeField] private bool _isMirrored;
    [SerializeField, HideInInspector] private Vector3 _defaultLocalScale = Vector3.one;
    [SerializeField, HideInInspector] private Vector3 _defaultTextLocalScale = Vector3.one;

    private Transform _unloadPointTransform;
    private Transform _endPointTransform;

    public Action<MiniGameUnloadBox> OnReturnAction;

    private void Awake()
    {
        Initialize();
        CaptureMirrorDefaultsIfNeeded();
        ApplyMirror();
    }

    private void OnValidate()
    {
        Initialize();
        CaptureMirrorDefaultsIfNeeded();
        ApplyMirror();
    }

    [ContextMenu("Refresh Mirror Defaults")]
    private void RefreshMirrorDefaults()
    {
        CaptureMirrorDefaults();
        ApplyMirror();
    }

    private void Initialize()
    {
        AllowedTypes = new[] { Define.BoxState.Cold, Define.BoxState.Normal };

        _unloadPointTransform = Utils.FindChild<Transform>(gameObject, "UnloadPoint", true);
        _endPointTransform = Utils.FindChild<Transform>(gameObject, "EndPoint", true);
        _ViewDeliveryRegionText = Utils.FindChild<TextMeshPro>(gameObject, "ViewDeliveryRegionText", true);

        UpdateRegionText();
    }

    private void UpdateRegionText()
    {
        if (_ViewDeliveryRegionText == null)
        {
            return;
        }

        _ViewDeliveryRegionText.SetText(_info.Region.ToString());
    }

    private void CaptureMirrorDefaultsIfNeeded()
    {
        if (_defaultLocalScale == Vector3.zero)
        {
            _defaultLocalScale = Vector3.one;
        }

        if (_defaultTextLocalScale == Vector3.zero)
        {
            _defaultTextLocalScale = Vector3.one;
        }

        if (Mathf.Approximately(_defaultLocalScale.x, 1f) && !Mathf.Approximately(Mathf.Abs(transform.localScale.x), 1f))
        {
            CaptureMirrorDefaults();
            return;
        }

        if (_ViewDeliveryRegionText != null &&
            Mathf.Approximately(_defaultTextLocalScale.x, 1f) &&
            !Mathf.Approximately(Mathf.Abs(_ViewDeliveryRegionText.transform.localScale.x), 1f))
        {
            CaptureMirrorDefaults();
        }
    }

    private void CaptureMirrorDefaults()
    {
        _defaultLocalScale = transform.localScale;
        _defaultLocalScale.x = Mathf.Abs(_defaultLocalScale.x);

        if (_ViewDeliveryRegionText != null)
        {
            _defaultTextLocalScale = _ViewDeliveryRegionText.transform.localScale;
            _defaultTextLocalScale.x = Mathf.Abs(_defaultTextLocalScale.x);
        }
    }

    private void ApplyMirror()
    {
        Vector3 localScale = _defaultLocalScale;
        localScale.x = _isMirrored ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
        transform.localScale = localScale;

        if (_ViewDeliveryRegionText == null)
        {
            return;
        }

        Vector3 textLocalScale = _defaultTextLocalScale;
        textLocalScale.x = _isMirrored ? -Mathf.Abs(textLocalScale.x) : Mathf.Abs(textLocalScale.x);
        _ViewDeliveryRegionText.transform.localScale = textLocalScale;
    }

    public void SetAction()
    {
        Managers.Sound.PlayAMB(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.Conveyor.ToString(), gameObject);
    }

    private void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            OnTriggerAction?.Invoke((int)MiniGameUnloadInteractionAction.DropBox);
        }
    }

    private void MoveToUnloadPoint(MiniGameUnloadBox box)
    {
        box.transform.DOMove(_unloadPointTransform.position, 1).OnComplete(() =>
            {
                MoveBoxToEndPoint(box);
            }
        );
    }

    private void MoveBoxToEndPoint(MiniGameUnloadBox box)
    {
        int score = 0;
        bool returnBox = false;

        if (!box.Info.IsGrab)
        {
            if (box.BoxState != Define.BoxState.Normal)
            {
                score = -50;
                returnBox = true;
            }
            else if (box.Info.IsBroken)
            {
                Logger.Log("broken box");
                score = -50;
                returnBox = true;

                Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.BrokenBox.ToString(), gameObject);
            }
            else if (CheckBoxInfo(box.Info))
            {
                Logger.Log("True Region");
                score = 100;
            }
            else
            {
                Logger.Log("False Region");
                score = -50;
                returnBox = true;
            }
        }

        box.transform.DOMove(_endPointTransform.position, 1).OnComplete(() =>
            {
                if (returnBox)
                {
                    ReturnBox(box);
                }
                else
                {
                    Managers.Resource.Destroy(box.gameObject);
                }
            }
        );

        OnScoreAction?.Invoke(score, box);
    }

    private void ReturnBox(MiniGameUnloadBox box)
    {
        if (OnReturnAction == null)
        {
            Managers.Resource.Destroy(box.gameObject);
            return;
        }

        OnReturnAction.Invoke(box);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            OnTriggerAction?.Invoke((int)MiniGameUnloadInteractionAction.None);
        }
    }

    public bool CheckBoxInfo(MiniGameUnloadBoxInfo boxInfo)
    {
        return boxInfo.Region == _info.Region;
    }

    public bool CanPlaceBox(MiniGameUnloadBox box)
    {
        return CanProcess(box.BoxState);
    }

    public void PlaceBox(MiniGameUnloadBox box)
    {
        if (box is null)
        {
            Logger.LogError("놓으려는 박스가 없는 상태");
            return;
        }

        box.transform.SetParent(transform);
        box.SetIsGrab(false);
        MoveToUnloadPoint(box);
    }
}
