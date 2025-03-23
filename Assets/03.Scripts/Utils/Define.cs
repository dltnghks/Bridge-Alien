using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        Dev,
        DataLoading,
        Title,
        House,
        GameMap,
        MiniGameUnload,
        MiniGameDelivery,
        Ending,
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
        Delivery,
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
        CapitalArea,
        ChungcheongArea,
        YeongnamArea,
        HonamArea,
        GangwonArea, 
    }

    public enum DailyEventType
    {
        Unknown,
        Dialog,
        MiniGame,
        End,
    }

    public enum DataType
    {
        Dialog,
        MiniGameSetting,
        Daily,
        PlayerStat,
        End,
    }

    public enum Dialog
    {
        Unknown,
        TUTORIAL_STORY_01,
        TUTORIAL_SOTRY_02,
    }

    public enum DialogType
    {
        Unknown,
        Monolog,
        Dialog,
        Choice,
        END,
    public enum PlayerStatType
    {
        Strength,
        Dexterity,
        Vitality,
        Intelligence,
        Luck,
    }
}
