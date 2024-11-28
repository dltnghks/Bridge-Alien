using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;

[System.Serializable]
public struct MiniGameUnloadBoxInfo
{
    public Define.BoxType BoxType;
    public int BoxNumber;
    public int Weight;
    public Define.BoxRegion Region;
    public bool IsFragileBox;
    public float Size;
    public bool IsBroken;
    public bool IsGrab;
    public MiniGameUnloadBoxInfo(Define.BoxType boxType, int boxNumber, int weight, Define.BoxRegion region, bool isFragileBox, float size)
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
}

public class MiniGameUnloadBox : MonoBehaviour
{
    [SerializeField] private MiniGameUnloadBoxInfo _info;
    
    private SpriteRenderer spriteRenderer;

    public GameObject TextObj;
    
    public MiniGameUnloadBoxInfo Info
    {
        get => _info;
        private set => _info = value;
    }

    public void SetBoxInfo(MiniGameUnloadBoxInfo info)
    {
        Info = info;
        GameObject spriteObj = Utils.FindChild(gameObject, "BoxSprite", true);
        spriteObj.transform.SetParent(gameObject.transform);
        spriteObj.transform.localPosition = Vector3.zero;
        spriteRenderer = spriteObj.GetOrAddComponent<SpriteRenderer>();
        spriteObj.AddComponent<SpriteBillboard>();
    }

    public void SetInGameActive(bool value, Vector3 pos = default(Vector3))
    {
        gameObject.SetActive(value);
        if(value)
        {
            transform.position = pos;    
        
        }
    }

    public void SetBoxInfo(Define.BoxType boxType, int boxNumber, int weight, Define.BoxRegion region, bool isFragileBox, float size)
    {
        _info = new MiniGameUnloadBoxInfo(boxType, boxNumber, weight, region, isFragileBox, size);
        
        TextObj.GetComponent<TextMeshPro>().SetText(
            $"{_info.BoxType.ToString()}\n" +
            $"{_info.BoxNumber.ToString()}\n" +
            $"{_info.Region.ToString()}\n" +
            $"{_info.Weight.ToString()}\n" +
            $"{_info.IsFragileBox.ToString()}\n" +
            $"Size : {_info.Size.ToString()}\n"
            );
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
    }
}

