using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;

[System.Serializable]
public struct MiniGameUnloadBoxInfo
{   
    public Define.BoxType BoxType;
    public string BoxNumber;
    public int Weight;
    public Define.BoxRegion Region;
    public bool IsFragileBox;
    public float Size;
    public bool IsBroken;
    public bool IsGrab;
    public bool IsUnloaded;
    
    public MiniGameUnloadBoxInfo(Define.BoxType boxType, string boxNumber, int weight, Define.BoxRegion region, bool isFragileBox, float size)
    {
        BoxType = boxType;
        BoxNumber = boxNumber;
        Weight = weight;
        Region = region;
        IsFragileBox = isFragileBox;
        Size = size;
        IsBroken = false;
        IsGrab = false;
        IsUnloaded = false;
    }

    public void SetRandomInfo()
    {
        // 랜덤 박스 정보 생성
        BoxType = (Define.BoxType)Random.Range(0, (int)Define.BoxType.LargeBox + 1);

        if (BoxType == Define.BoxType.Post){ Weight = 1; }
        else if (BoxType == Define.BoxType.SmallBox) { Weight = 3;}
        else if (BoxType == Define.BoxType.StandardBox) { Weight = 5;}
        else if (BoxType == Define.BoxType.LargeBox) { Weight = 10;}

        BoxNumber = GenerateRandomString();  // AAA-0000형태
        
        Region = (Define.BoxRegion)Random.Range(0, (int)Define.BoxRegion.GangwonArea + 1); // 지역 선택

        Size = (float)Weight / 10f;
        
        IsFragileBox = Random.Range(0, 10) < 2; // 20% 확률로 취급주의
    }
    
    private string GenerateRandomString()
    {
        // 알파벳과 숫자를 랜덤으로 생성하여 결합
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string digits = "0123456789";

        // 3개의 알파벳과 4개의 숫자 생성 후 결합
        return $"{GetRandomChars(letters, 3)}-{GetRandomChars(digits, 4)}";
    }

    private string GetRandomChars(string charSet, int length)
    {
        // 랜덤 생성기
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
        switch (Region)
        {
            case Define.BoxRegion.GangwonArea: return "강원권";
            case Define.BoxRegion.ChungcheongArea: return "충남권";
            case Define.BoxRegion.YeongnamArea: return "영남권";
            case Define.BoxRegion.CapitalArea: return "수도권";
            case Define.BoxRegion.HonamArea: return "호남권";
        }

        return "정의되지 않은 배송지역";
    }

    public string GetBoxType()
    {
        switch (BoxType)
        {
            case Define.BoxType.Post: return "우편";
            case Define.BoxType.LargeBox: return "대형 택배";
            case Define.BoxType.SmallBox: return "소형 택배";
            case Define.BoxType.StandardBox: return "일반 택배";
        }

        return "정의되지 않은 박스 타입";
    }
}

public class MiniGameUnloadBox : MonoBehaviour
{
    private int _defaultBoxLayer;
    private int _grabBoxLayer;

    [SerializeField] private MiniGameUnloadBoxInfo _info;
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshPro regionText;

    [Header("Box Sprites")]
    [SerializeField] private List<Sprite> _boxSpriteList = new List<Sprite>();  
    [SerializeField] private List<Sprite> _regionSpriteList = new List<Sprite>();

    private Rigidbody boxRigidbody;
    private BoxCollider boxCollider;

    public SpriteRenderer SpriteRenderer
    {
        get { return spriteRenderer; }
    }

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



    public void SetInGameActive(bool value, Vector3 pos = default(Vector3))
    {
        _defaultBoxLayer = LayerMask.NameToLayer("DefaultBox");
        _grabBoxLayer = LayerMask.NameToLayer("GrabBox");
        
        
        gameObject.SetActive(value);
        if(value)
        {
            transform.position = pos;
            spriteRenderer.color = new Color(1, 1, 1, 1);
            if (Info.IsFragileBox)
            {
                spriteRenderer.color = new Color(1, 0, 0, 0.7f);
            }

            Vector3 currentScale = boxCollider.size; 
            currentScale.x = 1f;
            currentScale.z = 1f;
            boxCollider.size = currentScale;

            // 생성될 때는 false
            boxCollider.isTrigger = false;
            IsUnloaded = false;

            PlayBoxPutSound();
            
        }
    }
    
    public void CheckBrokenBox(int height)
    {
        if(_info.IsFragileBox && height > 0)
        {
            _info.IsBroken = true;
        }
        else
        {
            _info.IsBroken = false;
        }
    }

    public void SetIsGrab(bool value)
    {
        _info.IsGrab = value;
        if(value){
            gameObject.layer = _grabBoxLayer;
            
            // 상자의 Rigidbody 비활성화
            boxRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            // 놓을 때는 true
            boxCollider.isTrigger = true;
            
            PlayBoxHoldSound();

        }else{
            gameObject.layer = _defaultBoxLayer;

            // 상자의 Rigidbody 활성화
            boxRigidbody.constraints = RigidbodyConstraints.FreezePositionX |
                                        RigidbodyConstraints.FreezePositionZ |
                                        RigidbodyConstraints.FreezeRotation;

            PlayBoxPutSound();
        }
    }

    private void PlayBoxHoldSound()
    {
        switch (Info.BoxType)
        {
            case Define.BoxType.Post: 
                Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.PostHold.ToString()); 
                break;
            case Define.BoxType.SmallBox: 
                Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.SmallBoxHold.ToString()); 
                break;
            case Define.BoxType.StandardBox: 
                Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.StandardBoxHold.ToString()); 
                break;
            case Define.BoxType.LargeBox: 
                Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.LargeBoxPHold.ToString()); 
                break;
        }
    }
    
    private void PlayBoxPutSound()
    {
        switch (Info.BoxType)
        {
            case Define.BoxType.Post: 
                Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.PostPut.ToString(), gameObject); 
                break;
            case Define.BoxType.SmallBox: 
                Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.SmallBoxPut.ToString(), gameObject); 
                break;
            case Define.BoxType.StandardBox: 
                Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.StandardBoxPut.ToString(),gameObject); 
                break;
            case Define.BoxType.LargeBox: 
                Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.LargeBoxPut.ToString(),gameObject); 
                break;
        }
    }

    public void SetRandomInfo()
    {
        _info.SetRandomInfo();
        
        AddBoxSprite();
        
        int boxType = (int)_info.BoxType;
        int regionType = (int)_info.Region;
        if (_boxSpriteList.Count > boxType)
        {
            spriteRenderer.sprite = _boxSpriteList[boxType];   
            if(boxType != 0){
                regionText.SetText(_info.GetBoxRegion());
            }else{
                regionText.SetText("");
            }
        }
        else
        {
            Logger.LogWarning($"Not enough box sprite list{_boxSpriteList.Count}, {boxType}");
        }

        boxRigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }
    
    private void AddBoxSprite()
    {
        if (spriteRenderer == null)
        {
            GameObject spriteObj = new GameObject("BoxSprite");
            spriteObj.transform.SetParent(gameObject.transform);
            spriteObj.transform.localPosition = Vector3.zero;
            spriteRenderer = spriteObj.GetOrAddComponent<SpriteRenderer>();
            SpriteRenderer parentSpriteRenderer = gameObject.GetOrAddComponent<SpriteRenderer>();
            spriteRenderer.material = parentSpriteRenderer.material;
            spriteRenderer.receiveShadows = parentSpriteRenderer.receiveShadows;
            spriteRenderer.shadowCastingMode = parentSpriteRenderer.shadowCastingMode;
            
            
            spriteObj.AddComponent<SpriteBillboard>();
            
        }

    }
}

