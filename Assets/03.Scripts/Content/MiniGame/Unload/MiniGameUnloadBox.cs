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
        
        Region = (Define.BoxRegion)Random.Range(0, (int)Define.BoxRegion.Central + 1); // 지역 선택

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
}

public class MiniGameUnloadBox : MonoBehaviour
{
    private int _defaultBoxLayer;
    private int _grabBoxLayer;

    [SerializeField] private MiniGameUnloadBoxInfo _info;
    
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer regionSpriteRenderer;

    [Header("Box Sprites")]
    [SerializeField] private List<Sprite> _boxSpriteList = new List<Sprite>();  
    [SerializeField] private List<Sprite> _regionSpriteList = new List<Sprite>();

    private Rigidbody boxRigidbody;
    private BoxCollider boxCollider;

    public MiniGameUnloadBoxInfo Info
    {
        get => _info;
        private set => _info = value;
    }

    public void SetInGameActive(bool value, Vector3 pos = default(Vector3))
    {
        _defaultBoxLayer = LayerMask.NameToLayer("DefaultBox");
        _grabBoxLayer = LayerMask.NameToLayer("GrabBox");
        
        gameObject.SetActive(value);
        if(value)
        {
            transform.position = pos;
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

        }else{
            gameObject.layer = _defaultBoxLayer;

            // 상자의 Rigidbody 활성화
            boxRigidbody.constraints = RigidbodyConstraints.FreezePositionX |
                                        RigidbodyConstraints.FreezePositionZ |
                                        RigidbodyConstraints.FreezeRotation;
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
        }
        else
        {
            Logger.LogWarning($"Not enough box sprite list{_boxSpriteList.Count}, {boxType}");
        }

        
        if (_regionSpriteList.Count > regionType)
        {
            regionSpriteRenderer.sprite = _regionSpriteList[boxType];   
        }
        else
        {
            Logger.LogWarning($"Not enough region sprite list{_regionSpriteList.Count}, {regionType}");
        }

        boxRigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }
    
    private void AddBoxSprite()
    {
        GameObject spriteObj = new GameObject("BoxSprite");
        spriteObj.transform.SetParent(gameObject.transform);
        spriteObj.transform.localPosition = Vector3.zero;
        spriteRenderer = spriteObj.GetOrAddComponent<SpriteRenderer>();

        GameObject regionSpriteObj = new GameObject("region");
        regionSpriteObj.transform.SetParent(gameObject.transform);
        regionSpriteObj.transform.localPosition = Vector3.zero;
        regionSpriteRenderer = regionSpriteObj.GetOrAddComponent<SpriteRenderer>();
    }
}

