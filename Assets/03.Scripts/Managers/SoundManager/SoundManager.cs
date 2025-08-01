using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private SoundEvent _soundEvent;

    // Sound Parameters 
    public float AllVolume { get; private set; }
    public float BGMVolume { get; private set; }
    public float SFXVolume { get; private set; }

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
            Logger.Log("SoundEvent ScriptableObject loaded successfully.");
            _soundEvent.InitEventDict(); // 초기화
        }
    }

    public void LoadSoundBank(string bankName)
    {
        // SoundBank의 ID가 저장될 변수
        uint bankID = AkBankManager.LoadBank(bankName, false, false);

        Debug.Log($"SoundBank '{bankName}' loaded successfully! Bank ID: {bankID}");

    }

    public void UnloadAllSoundBank()
    {
        AkBankManager.UnloadAllBanks();
    }

    public void UnloadSoundBank(string bankName)
    {
        AkBankManager.UnloadBank(bankName);
    }

    public void PlayBGM(string eventName){
        PlaySound(SoundType.SceneBGM, eventName);
    }

    public void PauseBGM()
    {
        Define.Scene type = Managers.Scene.CurrentSceneType;
        string eventName = $"Pause_BGM_{type}";
        Logger.Log(eventName);
        BGMPauseSound(type, eventName);
    }

    public void ResumeBGM()
    {
        Define.Scene type = Managers.Scene.CurrentSceneType;
        string eventName = $"Resume_BGM_{type}";
        Logger.Log(eventName);
        BGMResumeSound(type, eventName);
    }

    public void StopBGM()
    {
        AkUnitySoundEngine.StopAll();
    }
    
    public void PlaySFX(SoundType type, string eventName, GameObject soundGameObject = null){
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
        if (_soundEvent == null || !_soundEvent.BGMPauseEventDic.ContainsKey(type))
        {
            Debug.LogWarning($"SoundType {type} not found!");
            return;
        }

        if (_soundEvent.BGMPauseEventDic.TryGetValue(type, out AK.Wwise.Event soundEvent))
        {
            //Debug.Log($"Playing sound: {soundEvent}");
            // 이벤트 호출
            PlayEvent(soundEvent, soundGameObject);
        }
        else
        {
            Debug.LogWarning($"Key {key} not found in SoundType {type}!");
        }
    }

    private void BGMResumeSound(Define.Scene type, string key, GameObject soundGameObject = null)
    {
        if (_soundEvent == null || !_soundEvent.BGMResumeEventDic.ContainsKey(type))
        {
            Debug.LogWarning($"SoundType {type} not found!");
            return;
        }

        if (_soundEvent.BGMResumeEventDic.TryGetValue(type, out AK.Wwise.Event soundEvent))
        {
            //Debug.Log($"Playing sound: {soundEvent}");
            // 이벤트 호출
            PlayEvent(soundEvent, soundGameObject);
        }
        else
        {
            Debug.LogWarning($"Key {key} not found in SoundType {type}!");
        }
    }

    private void PlaySound(SoundType type, string key, GameObject soundGameObject = null)
    {
        if (_soundEvent == null || !_soundEvent.EventDict.ContainsKey(type))
        {
            Debug.LogWarning($"SoundType {type} not found!");
            return;
        }

        if (_soundEvent.EventDict[type].TryGetValue(key, out AK.Wwise.Event soundEvent))
        {
            //Debug.Log($"Playing sound: {soundEvent}");
            // 이벤트 호출
            PlayEvent(soundEvent, soundGameObject);
        }
        else
        {
            Debug.LogWarning($"Key {key} not found in SoundType {type}!");
        }
    }

    private void PlayEvent(AK.Wwise.Event soundEvent, GameObject soundGameObject){

        if(soundGameObject == null)
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
        Debug.Log($"All Volume set to {AllVolume}");
    }
    
    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp(volume, 0f, 100f);
        AkUnitySoundEngine.SetRTPCValue("SFXVolume", SFXVolume);
        Debug.Log($"SFX Volume set to {SFXVolume}");
    }
    
    public void SetBGMVolume(float volume)
    {
        BGMVolume = Mathf.Clamp(volume, 0f, 100f);
        AkUnitySoundEngine.SetRTPCValue("BGMVolume", BGMVolume);
        Debug.Log($"BGM Volume set to {BGMVolume}");
    }
}