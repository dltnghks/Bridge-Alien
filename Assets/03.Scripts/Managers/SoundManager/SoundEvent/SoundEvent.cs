using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public enum SoundType
{
    SceneBGM,
    CommonSFX,
    MiniGameUnloadSFX,

}

public enum SceneBGM{
    MiniGameUnloadBGM,
}
public enum CommonSoundSFX{
    //common
    MenuButtonClick,
}
public enum MiniGameUnloadSoundSFX{
    
    //sfx
    PostPut,
    SmallBoxPut,
    StandardBoxPut,
    LargeBoxPut,

    PostHold,
    SmallBoxHold,
    StandardBoxHold,
    LargeBoxPHold,
}

[CreateAssetMenu(menuName = "Sound/SoundEvent")]
public class SoundEvent : ScriptableObject
{
    public SerializableDictionary<SoundType, SerializableDictionary<string, string>> EventDict = new SerializableDictionary<SoundType, SerializableDictionary<string, string>>();

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
            SoundType.CommonSFX => typeof(CommonSoundSFX),
            SoundType.MiniGameUnloadSFX => typeof(MiniGameUnloadSoundSFX),
            _ => null,
        };

        // Enum 타입이 null일 경우 반환
        if (enumType == null)
            return;

        // Dictionary 초기화
        if (!EventDict.ContainsKey(type))
            EventDict[type] = new SerializableDictionary<string, string>();


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
                string eventName = $"Play_{keyString}";
                EventDict[type].Add(keyString, eventName);
            }
            else if(string.IsNullOrEmpty(EventDict[type][keyString])){
                string eventName = $"Play_{keyString}";
                EventDict[type][keyString] = eventName;
            }
        }
    }
}
