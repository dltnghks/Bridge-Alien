using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.PlayerLoop;

public enum SoundType
{
    SceneBGM,
    CommonSoundSFX,
    MiniGameUnloadSFX,
    PrologueSFX,
}

public enum SceneBGM
{
    Title,
    House,
    Prologue,
    MiniGameUnload,
    MiniGameDelivery,
}
public enum CommonSoundSFX
{
    CommonButtonClick,
    FootStepPlayerCharacter,
    GameStart,
}

public enum PrologueSFX
{
    Voice,
}

public enum MiniGameUnloadSoundSFX
{

    //sfx
    BoxPut,
    BoxHold,
    BrokenBox,

    PlusScore,
    MinusScore,

    CoolingComplete,
    CoolingSkill,
    Glitch,
    SpeedUpSkill,
    Discard,
    LastScore,
    DisposalUnitOpenDoor,
    Siren,

    // Ambient
    Conveyor,
    Truck,
    CoolingMachine,

}

[CreateAssetMenu(fileName = "SoundEvent", menuName = "Sound/SoundEvent")]
public class SoundEvent : ScriptableObject
{
    public SerializedDictionary<SoundType, SerializedDictionary<string, AK.Wwise.Event>> EventDict = new SerializedDictionary<SoundType, SerializedDictionary<string, AK.Wwise.Event>>();

    public void InitEventDict()
    {
        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            InitSoundDict(type);
        }
    }

    private void InitSoundDict(SoundType type)
    {
        // SoundType에 따라 관련 Enum 타입 가져오기
        // 새로운 타입이 추가되면 여기에 추가
        Type enumType = type switch
        {
            SoundType.SceneBGM => typeof(SceneBGM),
            SoundType.CommonSoundSFX => typeof(CommonSoundSFX),
            SoundType.MiniGameUnloadSFX => typeof(MiniGameUnloadSoundSFX),
            _ => null,
        };

        // Enum 타입이 null일 경우 반환
        if (enumType == null)
            return;

        // Dictionary 초기화
        if (!EventDict.ContainsKey(type))
            EventDict[type] = new SerializedDictionary<string, AK.Wwise.Event>();


        // Enum 값의 Key를 HashSet으로 수집 (빠른 검사용)
        HashSet<string> validKeys = new HashSet<string>(Enum.GetNames(enumType));

        // 현재 Dictionary에서 유효하지 않은 Key를 제거
        var keysToRemove = new List<string>();
        foreach (var existingKey in EventDict[type].Keys)
        {
            if (!validKeys.Contains(existingKey))
            {
                keysToRemove.Add(existingKey);
            }
        }

        foreach (var key in keysToRemove)
        {
            EventDict[type].Remove(key);
        }

        // Enum 값을 기반으로 Key-Value 초기화
        foreach (int value in Enum.GetValues(enumType))
        {
            string keyString = Enum.GetName(enumType, value);
            if (!string.IsNullOrEmpty(keyString) && !EventDict[type].ContainsKey(keyString))
            {
                EventDict[type].Add(keyString, null);
            }
        }
    }
}
