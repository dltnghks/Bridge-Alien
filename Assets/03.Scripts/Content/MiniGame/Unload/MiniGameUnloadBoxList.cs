using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnloadBoxList
{
    public int MaxUnloadBoxIndex{get; private set;}
    public int CurrentUnloadBoxIndex
    {
        get{ return _inGameUnloadBoxList.Count;} 
    }

    public bool IsFull
    {
        get{ return MaxUnloadBoxIndex <= CurrentUnloadBoxIndex;}
    }

    public bool IsEmpty
    {
        get { return CurrentUnloadBoxIndex == 0; }
    }

    private List<MiniGameUnloadBox> _inGameUnloadBoxList = new List<MiniGameUnloadBox>();

    public void SetBoxList(int maxIndex)
    {
        MaxUnloadBoxIndex = maxIndex;
    }

    public bool TryAddInGameUnloadBoxList(MiniGameUnloadBox newInGameUnloadBox)
    {
        if(MaxUnloadBoxIndex > CurrentUnloadBoxIndex)
        {
            _inGameUnloadBoxList.Add(newInGameUnloadBox);
            return true;
        }
        else
        {
            return false;
        }
    }

    public MiniGameUnloadBox RemoveAndGetTopInGameUnloadBoxList()
    {
        MiniGameUnloadBox returnBox = null;
        if(_inGameUnloadBoxList.Count > 0)
        {
            returnBox = _inGameUnloadBoxList[_inGameUnloadBoxList.Count-1];
            _inGameUnloadBoxList.RemoveAt(_inGameUnloadBoxList.Count-1);
            return returnBox;
        }
        else
        {
            return null;
        }
    }
}
