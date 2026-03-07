using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private SoundEvent _soundEvent;

    public float AllVolume { get; private set; }
    public float BGMVolume { get; private set; }
    public float SFXVolume { get; private set; }
    public SoundType? CurrentBGMType { get; private set; }
    public string CurrentBGMEventName { get; private set; }

    private string _basePath;

    public void Init()
    {
        SetAllVolume(100f);
        SetSFXVolume(100f);
        SetBGMVolume(100f);
        LoadSoundEvent();
    }

    private void LoadSoundEvent()
    {
        _soundEvent = Managers.Resource.Load<SoundEvent>("Sound/SoundEvent");

        if (_soundEvent == null)
        {
            Logger.LogError("SoundEvent ScriptableObject could not be loaded!");
        }
        else
        {
            _soundEvent.InitEventDict();
        }
    }

    public void LoadSoundBank(string bankName)
    {
        AkBankManager.LoadBank(bankName, false, false);
    }

    public void UnloadAllSoundBank()
    {
        AkBankManager.UnloadAllBanks();
    }

    public void UnloadSoundBank(string bankName)
    {
        AkBankManager.UnloadBank(bankName);
    }

    public void PlayBGM(string eventName)
    {
        PlaySceneBGM(eventName);
    }

    public void PlaySceneBGM(string eventName)
    {
        Logger.Log($"Scene BGM Start: {eventName}");
        CurrentBGMType = SoundType.SceneBGM;
        CurrentBGMEventName = eventName;
        PlaySound(SoundType.SceneBGM, eventName);
    }

    public void PlayDialogBGM(DialogBGM dialogBGM)
    {
        string eventName = dialogBGM.ToString();
        Logger.Log($"Dialog BGM Start: {eventName}");
        CurrentBGMType = SoundType.DialogBGM;
        CurrentBGMEventName = eventName;
        PlaySound(SoundType.DialogBGM, eventName);
    }

    public void PauseBGM()
    {
        Define.Scene type = Managers.Scene.CurrentSceneType;
        string eventName = $"{type}";
        Logger.Log($"BGM Pause: {eventName}");
        BGMPauseSound(type, eventName);
    }

    public void ResumeBGM()
    {
        Define.Scene type = Managers.Scene.CurrentSceneType;
        string eventName = $"{type}";
        Logger.Log($"BGM Resume: {eventName}");
        BGMResumeSound(type, eventName);
    }

    public void StopBGM()
    {
        Logger.Log("BGM Stop: StopAll");
        CurrentBGMType = null;
        CurrentBGMEventName = null;
        AkUnitySoundEngine.StopAll();
    }

    public void PlaySFX(SoundType type, string eventName, GameObject soundGameObject = null)
    {
        PlaySound(type, eventName, soundGameObject);
    }

    public void PlayAMB(SoundType type, string eventName, GameObject soundGameObject)
    {
        if (soundGameObject == null)
        {
            Logger.LogWarning("Sound GameObject is null!");
            return;
        }

        PlaySound(type, eventName, soundGameObject);
    }

    private void BGMPauseSound(Define.Scene type, string key, GameObject soundGameObject = null)
    {
        if (_soundEvent == null || _soundEvent.BGMPauseEventDic.ContainsKey(type) == false)
        {
            Logger.LogWarning($"SoundType {type} not found!");
            return;
        }

        if (_soundEvent.BGMPauseEventDic.TryGetValue(type, out AK.Wwise.Event soundEvent))
        {
            PlayEvent(soundEvent, soundGameObject);
        }
        else
        {
            Logger.LogWarning($"Key {key} not found in SoundType {type}!");
        }
    }

    private void BGMResumeSound(Define.Scene type, string key, GameObject soundGameObject = null)
    {
        if (_soundEvent == null || _soundEvent.BGMResumeEventDic.ContainsKey(type) == false)
        {
            Logger.LogWarning($"SoundType {type} not found!");
            return;
        }

        if (_soundEvent.BGMResumeEventDic.TryGetValue(type, out AK.Wwise.Event soundEvent))
        {
            PlayEvent(soundEvent, soundGameObject);
        }
        else
        {
            Logger.LogWarning($"Key {key} not found in SoundType {type}!");
        }
    }

    private void PlaySound(SoundType type, string key, GameObject soundGameObject = null)
    {
        if (_soundEvent == null || _soundEvent.EventDict.ContainsKey(type) == false)
        {
            Logger.LogWarning($"SoundType {type} not found!");
            return;
        }

        if (_soundEvent.EventDict[type].TryGetValue(key, out AK.Wwise.Event soundEvent))
        {
            PlayEvent(soundEvent, soundGameObject);
        }
        else
        {
            Logger.LogWarning($"Key {key} not found in SoundType {type}!");
        }
    }

    private void PlayEvent(AK.Wwise.Event soundEvent, GameObject soundGameObject)
    {
        if (soundGameObject == null)
        {
            soundGameObject = gameObject;
        }

        soundEvent.Post(soundGameObject);
    }

    public void PauseSFX()
    {
    }

    public void SetAllVolume(float volume)
    {
        AllVolume = Mathf.Clamp(volume, 0f, 100f);
        AkUnitySoundEngine.SetRTPCValue("AllVolume", AllVolume);
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp(volume, 0f, 100f);
        AkUnitySoundEngine.SetRTPCValue("SFXVolume", SFXVolume);
    }

    public void SetBGMVolume(float volume)
    {
        BGMVolume = Mathf.Clamp(volume, 0f, 100f);
        AkUnitySoundEngine.SetRTPCValue("BGMVolume", BGMVolume);
    }
}
