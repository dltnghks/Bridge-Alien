using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        Dev,
        Title,
        House,
        GameMap,
        GameUnload,
    }
    
    public enum UIEvent
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
        BeginDrag,
        Drag,
        EndDrag,
    }

    public enum MiniGameType
    {
        Unknown,
        Unload,
        
    }
    
    public enum BoxType
    {
        Post,           // 우편
        SmallBox,    // 소형 택배
        StandardBox, // 일반 택배
        LargeBox     // 대형 택배
    }

    public enum BoxRegion
    {
        North,
        South,
        East,
        West,
        Central, 
    }
}
