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
        SmallParcel,    // 소형 택배
        StandardParcel, // 일반 택배
        LargeParcel     // 대형 택배
    }
}
