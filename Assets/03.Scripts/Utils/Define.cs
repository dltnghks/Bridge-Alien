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
}
