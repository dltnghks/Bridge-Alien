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
        EventScene,
        Title,
        House,
        MiniGameUnload,
        StageEditor,
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
        Unknown,
        Common,
        Cold,
        Fragile,
    }

    public enum BoxState
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
    }

    public enum EventType
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
        Event,
        MiniGameSetting,
        PlayerStat,
        PlayerTask,
        MiniGameSkill,
        Stage,
        End,
    }

    public enum EventDataID
    {
        Unknown,
        Event_C1,
        Event_C2,
        Event_C3,
        Event_C4,
        Event_C5,

        Event_C1_Clear,
        Event_C2_Clear,
        Event_C3_Clear,
        Event_C4_Clear,
        Event_C5_Clear,
    }

    public enum Dialog
    {
        Unknown,
        P1_01,
        S1_01,
        S1_02,
        S1_03,
        S1_04,
        S1_05,

        S2_01,
        S2_02,
        S2_03,
        S2_04,

        S3_01,
        S3_02,

        S4_01,
        S4_02,
        S4_03,

        S5_01,
        S5_02,
        S5_03,
        S5_04,
        S5_05,
    }

    public enum DialogType
    {
        Unknown,
        Monolog,
        Dialog,
        Choice,
        End,
    }

    public enum DialogSceneType
    {
        Unknown,
        House,             // 집
        Station,           // 정거장
        Office,            // 사무실
        UnloadWorkplace,   // 하차 작업장
        DeliveryGarage,    // 배송 차고지
        ManagerRoom,       // 소장실
        END_BACKGROUND,    // 엔딩
    }

    public enum DialogSpeakerType
    {
        UNKNOWN,
        KIM_DEFAULT,            // 김이민 기본 
        KIM_CALL,               // 김이민 전화
        KIM_HARD,               // 김이민 힘든 상태
        SHIN_DEFAULT,           // 신팀장 기본
        HWANG_DEFAULT,          // 황반장 기본
        HWANG_CALL,             // 황반장 전화
        MANAGER_DEFAULT,        // 소장 기본
        MANAGER_ANGRY,          // 소장 화남
        PIMPI_DEFAULT,          // 핌피 기본
        PIMPI_CALL,             // 핌피 전화
        GUIDE,                  // 가이드  
    }

    public enum DialogSpeakerPosType
    {
        Unknown,
        Left,   // 왼쪽
        Right,  // 오른쪽
    }

    public enum PlayerStatsType
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

    // 전체 스킬
    public enum MiniGameSkillType
    {
        // 하차게임 스킬
        CoolingSkill,
        BoxWarpSkill,
        SpeedUpSkill,
    }

    public enum ChapterType
    {
        CH1,
        CH2,
        CH3,
        CH4,
        CH5,
    }
}
