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
        MiniGameSkill,
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
        STORY_D1_07,

        STORY_D2_01,
        STORY_D2_02,
        STORY_D2_03,
        STORY_D2_04,
        STORY_D2_05,
        STORY_D2_06,
        STORY_D2_07,

        STORY_D3_01,
        STORY_D3_02,
        STORY_D3_03,
        STORY_D3_04,
        STORY_D3_05,

        STORY_D4_01,
        STORY_D4_02,
        STORY_D4_03,
        STORY_D4_04,
        STORY_D4_05,

        STORY_D11_01,
        STORY_D11_02,
        STORY_D11_03,
        STORY_D11_04,
        STORY_D11_05,
        STORY_D11_06,

        STORY_D14A_01,
        STORY_D14A_02,
        STORY_D14A_03,
        STORY_D14A_04,
        STORY_D14A_05,

        STORY_D14B_01,
        STORY_D14B_02,
        STORY_D14B_03,
        STORY_D14B_04,

        STORY_D14C_01,
        STORY_D14C_02,
        STORY_D14C_03,

        STORY_END_A,
        STORY_END_B,
        STORY_END_C,
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

        // 배송게임 스킬
        //TestSkill,
        EmergencyRepairSkill,
        EmergencyRocketSkill
    }
}
