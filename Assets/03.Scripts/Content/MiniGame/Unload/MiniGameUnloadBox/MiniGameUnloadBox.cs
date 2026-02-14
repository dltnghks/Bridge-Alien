using UnityEngine;

[System.Serializable]
public struct MiniGameUnloadBoxInfo
{
    public string BoxNumber;
    public Define.BoxType BoxType;
    public Define.BoxState BoxState;
    public Define.BoxRegion Region;
    public bool IsGrab;
    public bool IsBroken;
    public bool IsUnloaded;
    public bool IsHidden;

    public MiniGameUnloadBoxInfo(string boxNumber, Define.BoxRegion region)
    {
        BoxNumber = boxNumber;
        Region = region;
        BoxState = Define.BoxState.Normal;
        IsBroken = false;
        IsGrab = false;
        IsUnloaded = false;
        IsHidden = false;
        BoxType = Define.BoxType.Common;
    }

    public void SetRandomInfo()
    {
        BoxNumber = GenerateRandomString();
        Region = (Define.BoxRegion)Random.Range(0, (int)Define.BoxRegion.D + 1);
        IsHidden = false;
    }

    public void SetHiddenInfo()
    {
        BoxNumber = $"CHR-{Random.Range(0, 10000):D4}";
        Region = (Define.BoxRegion)Random.Range(0, (int)Define.BoxRegion.D + 1);
        BoxState = Define.BoxState.Normal;
        IsBroken = false;
        IsHidden = true;
        BoxType = Define.BoxType.Common;
    }

    public void SetRegion(Define.BoxRegion region)
    {
        Logger.Log("region : " + region);
        Region = (Define.BoxRegion)System.Math.Clamp((int)region, (int)Define.BoxRegion.A, (int)Define.BoxRegion.D);
    }

    private string GenerateRandomString()
    {
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string digits = "0123456789";
        return $"{GetRandomChars(letters, 3)}-{GetRandomChars(digits, 4)}";
    }

    private string GetRandomChars(string charSet, int length)
    {
        System.Random random = new System.Random();

        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = charSet[random.Next(charSet.Length)];
        }
        return new string(result);
    }

    public string GetBoxRegion()
    {
        return Region.ToString();
    }
}

public class MiniGameUnloadBox : MonoBehaviour
{
    protected int _defaultBoxLayer;
    protected int _grabBoxLayer;

    [SerializeField] protected MiniGameUnloadBoxInfo _info;
    [SerializeField] protected SpriteRenderer _returnStickerSprite;
    [SerializeField] protected int _returnStickerSortingOffset = 1;
    [SerializeField] protected float _sortingOrderScale = 100f;
    [SerializeField] protected int _sortingOrderBase = 0;

    protected SpriteRenderer boxSpriteRenderer;
    protected Rigidbody boxRigidbody;
    protected BoxCollider boxCollider;
    private int _lastBoxSortingLayerId = int.MinValue;
    private int _lastBoxSortingOrder = int.MinValue;

    public MiniGameUnloadBoxInfo Info
    {
        get => _info;
        private set => _info = value;
    }

    public bool IsUnloaded
    {
        get { return _info.IsUnloaded; }
        set { _info.IsUnloaded = value; }
    }

    public Define.BoxState BoxState
    {
        get { return _info.BoxState; }
        set { _info.BoxState = value; }
    }

    public Define.BoxType BoxType
    {
        get { return _info.BoxType; }
        set { _info.BoxType = value; }
    }

    protected virtual void Init()
    {
        boxSpriteRenderer = GetComponent<SpriteRenderer>();
        boxRigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        UpdateSortingFromZ();
        SyncReturnStickerSorting();
    }

    private void LateUpdate()
    {
        UpdateSortingFromZ();
        SyncReturnStickerSortingIfNeeded();
    }

    public void SetInGameActive(bool value, Vector3 pos = default(Vector3))
    {
        _defaultBoxLayer = LayerMask.NameToLayer("DefaultBox");
        _grabBoxLayer = LayerMask.NameToLayer("GrabBox");

        gameObject.SetActive(value);
        if (value)
        {
            transform.position = pos;
            transform.localRotation = Quaternion.Euler(Vector3.zero);

            Vector3 currentScale = boxCollider.size;
            currentScale.x = 1f;
            currentScale.z = 1f;
            currentScale.y = 0.8f;

            boxCollider.size = currentScale;
            boxCollider.isTrigger = false;
            IsUnloaded = false;

            boxRigidbody.velocity = Vector3.zero;

            PlayBoxPutSound();
        }
    }

    public void SetSpawnBox(Vector3 spawnPos)
    {
        SetInGameActive(true, spawnPos);
        boxRigidbody.constraints = RigidbodyConstraints.FreezeAll ^ RigidbodyConstraints.FreezePositionY;
    }

    public void SetIsGrab(bool value)
    {
        _info.IsGrab = value;
        if (value)
        {
            gameObject.layer = _grabBoxLayer;
            boxRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            boxCollider.isTrigger = true;
            PlayBoxHoldSound();
        }
        else
        {
            gameObject.layer = _defaultBoxLayer;
            boxRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            PlayBoxPutSound();
        }
    }

    private void PlayBoxHoldSound()
    {
        Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.BoxHold.ToString());
    }

    private void PlayBoxPutSound()
    {
        Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.BoxPut.ToString(), gameObject);
    }

    public virtual void SetRandomInfo()
    {
        Init();
        SetReturnSticker(false);
        _info.SetRandomInfo();

        boxRigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public virtual void SetHiddenInfo()
    {
        Init();
        SetReturnSticker(false);
        _info.SetHiddenInfo();

        boxRigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public void SetRegion(Define.BoxRegion region)
    {
        _info.SetRegion(region);
    }

    public void SetReturnSticker(bool value)
    {
        if (_returnStickerSprite == null)
        {
            return;
        }

        _returnStickerSprite.gameObject.SetActive(value);
    }

    private void SyncReturnStickerSorting()
    {
        if (_returnStickerSprite == null || boxSpriteRenderer == null)
        {
            return;
        }

        _returnStickerSprite.sortingLayerID = boxSpriteRenderer.sortingLayerID;
        _returnStickerSprite.sortingOrder = boxSpriteRenderer.sortingOrder + _returnStickerSortingOffset;

        Vector3 pos = _returnStickerSprite.transform.localPosition;
        pos.z = 0f;
        _returnStickerSprite.transform.localPosition = pos;
    }

    private void SyncReturnStickerSortingIfNeeded()
    {
        if (_returnStickerSprite == null || boxSpriteRenderer == null || !_returnStickerSprite.gameObject.activeSelf)
        {
            return;
        }

        if (_lastBoxSortingLayerId != boxSpriteRenderer.sortingLayerID ||
            _lastBoxSortingOrder != boxSpriteRenderer.sortingOrder)
        {
            _lastBoxSortingLayerId = boxSpriteRenderer.sortingLayerID;
            _lastBoxSortingOrder = boxSpriteRenderer.sortingOrder;
            SyncReturnStickerSorting();
        }
    }

    private void UpdateSortingFromZ()
    {
        if (boxSpriteRenderer == null)
        {
            return;
        }

        int order = _sortingOrderBase + Mathf.RoundToInt(-transform.position.z * _sortingOrderScale);
        if (order < 0)
        {
            order = 0;
        }
        if (boxSpriteRenderer.sortingOrder != order)
        {
            boxSpriteRenderer.sortingOrder = order;
        }
    }
}
