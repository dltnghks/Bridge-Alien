using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MiniGameUnloadBoxInfo
{
    public Define.BoxType BoxType;
    public int BoxNumber;
    public int Weight;
    public string Region;
    public bool IsFragileBox;
}

public class MiniGameUnloadBox : MonoBehaviour
{
    private MiniGameUnloadBoxInfo _info;

    public MiniGameUnloadBoxInfo Info
    {
        get => _info;
        private set => _info = value;
    }
    
    public void SetBoxInfo(MiniGameUnloadBoxInfo info)
    {
        Info = info;
    }

    public void SetBoxInfo(Define.BoxType boxType, int boxNumber, int weight, string region, bool isFragileBox)
    {
        _info.BoxType = boxType;
        _info.BoxNumber = boxNumber;
        _info.Weight = weight;
        _info.Region = region;
        _info.IsFragileBox = isFragileBox;
    }

}

