using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        Dev,
        Title,
        House,
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
        Normal,     // 일반 배송 가능 상태
        Disposal,   // 폐기 상태
        Cold,       // 냉동이 요구되는 상태, 냉동이 완료되면 Normal로 변경
    }

    public enum BoxRegion
    {
        A,
        B,
        C,
        D,
        E,
    }

    public enum DailyEventType
    {
        Unknown,
        Dialog,
        MiniGame,
        Task,
        End,
    }

    public enum DataType
    {
        Dialog,
        MiniGameSetting,
        Daily,
        PlayerStat,
        PlayerTask,
        End,
    }

    public enum Dialog
    {
        Unknown,
        STORY_D1_01,
        STORY_D1_02,
        STORY_D1_03,
        STORY_D1_04,
        STORY_D1_05,
        STORY_D1_06,
        TUTORIAL_STORY_01,
    }

    public enum DialogType
    {
        Unknown,
        Monolog,
        Dialog,
        Choice,
        End,
    }

    public enum PlayerStatType
    {
        Fatigue,               // 피로도
        Experience,            // 작업 숙련
        GravityAdaptation,     // 중력 적응
        Intelligence,          // 지능
        Luck,                  // 운
    }

    public enum TaskType
    {
        Unknown,
        SelfDevelopment,    // 자기개발
        Fortune,         // 투자
        Entertainment,      // 유흥
        
    }
}
