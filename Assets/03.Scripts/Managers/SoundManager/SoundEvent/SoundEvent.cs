using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public enum SoundType
{
    SceneBGM = 0,
    CommonSoundSFX = 1,
    MiniGameUnloadSFX = 2,
    MiniGameDeliverySFX = 3,
    PrologueSFX = 4,
    DialogBGM = 5,
}

public enum SceneBGM
{
    Title,
    House,
    Prologue,
    MiniGameUnload,
    MiniGameDelivery,
    StageEditor,
    Ending,
}

public enum DialogBGM
{
    Default,
    Default2,
    Second_Half,
    End,
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
    Conveyor,
    Truck,
    CoolingMachine,
}

public enum MiniGameDeliverySoundSFX
{
    Player_BeAttacked,
    Idle_Flying,
    Minigame_Start,
    Minigame_Lever,
    Minigame_Success,
    Minigame_Fail,
    BoosterSkill,
    RepairSkill,
}

[CreateAssetMenu(fileName = "SoundEvent", menuName = "Sound/SoundEvent")]
public class SoundEvent : ScriptableObject
{
    public SerializedDictionary<Define.Scene, AK.Wwise.Event> BGMPauseEventDic = new SerializedDictionary<Define.Scene, AK.Wwise.Event>();
    public SerializedDictionary<Define.Scene, AK.Wwise.Event> BGMResumeEventDic = new SerializedDictionary<Define.Scene, AK.Wwise.Event>();
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
        Type enumType = type switch
        {
            SoundType.SceneBGM => typeof(SceneBGM),
            SoundType.DialogBGM => typeof(DialogBGM),
            SoundType.CommonSoundSFX => typeof(CommonSoundSFX),
            SoundType.MiniGameUnloadSFX => typeof(MiniGameUnloadSoundSFX),
            SoundType.MiniGameDeliverySFX => typeof(MiniGameDeliverySoundSFX),
            SoundType.PrologueSFX => typeof(PrologueSFX),
            _ => null,
        };

        if (enumType == null)
        {
            return;
        }

        if (EventDict.ContainsKey(type) == false)
        {
            EventDict[type] = new SerializedDictionary<string, AK.Wwise.Event>();
        }

        HashSet<string> validKeys = new HashSet<string>(Enum.GetNames(enumType));
        List<string> keysToRemove = new List<string>();
        foreach (var existingKey in EventDict[type].Keys)
        {
            if (validKeys.Contains(existingKey) == false)
            {
                keysToRemove.Add(existingKey);
            }
        }

        foreach (var key in keysToRemove)
        {
            EventDict[type].Remove(key);
        }

        foreach (string keyString in Enum.GetNames(enumType))
        {
            if (EventDict[type].ContainsKey(keyString) == false)
            {
                EventDict[type].Add(keyString, null);
            }
        }
    }
}
