using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnloadBox
{
    public Define.BoxType BoxType { get; private set; }
    public int BoxNumber { get; private set; }
    public int Weight { get; private set; }
    public string Region { get; private set; }
    public bool IsFragileBox { get; private set; }
    
    
    public void SetBoxInfo(Define.BoxType boxType, int boxNumber, int weight, string region, bool isFragileBox)
    {
        BoxType = boxType;
        BoxNumber = boxNumber;
        Weight = weight;
        Region = region;
        IsFragileBox = isFragileBox;
    }
}

