using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct MiniGameUnloadBoxInfo
{
    public Define.BoxType BoxType;
    public int BoxNumber;
    public int Weight;
    public Define.BoxRegion Region;
    public bool IsFragileBox;
    
    public MiniGameUnloadBoxInfo(Define.BoxType boxType, int boxNumber, int weight, Define.BoxRegion region, bool isFragileBox)
    {
        BoxType = boxType;
        BoxNumber = boxNumber;
        Weight = weight;
        Region = region;
        IsFragileBox = isFragileBox;
    }
}

public class MiniGameUnloadBox : MonoBehaviour
{
    private MiniGameUnloadBoxInfo _info;
    
    public GameObject TextObj;
    
    public MiniGameUnloadBoxInfo Info
    {
        get => _info;
        private set => _info = value;
    }
    
    public void SetBoxInfo(MiniGameUnloadBoxInfo info)
    {
        Info = info;
    }

    public void SetBoxInfo(Define.BoxType boxType, int boxNumber, int weight, Define.BoxRegion region, bool isFragileBox)
    {
        
        _info = new MiniGameUnloadBoxInfo(boxType, boxNumber, weight, region, isFragileBox);
        
        TextObj.GetComponent<TextMeshPro>().SetText(
            $"{_info.BoxType.ToString()}\n" +
            $"{_info.BoxNumber.ToString()}\n" +
            $"{_info.Region.ToString()}\n" +
            $"{_info.Weight.ToString()}\n" +
            $"{_info.IsFragileBox.ToString()}\n"
            );
    }
}

