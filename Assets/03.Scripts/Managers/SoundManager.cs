using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public void Init(){

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

    public void PlayBGM(string eventName, GameObject soundGameObject = null){
        PlaySound($"Play_{eventName}BGM", soundGameObject);
    }

    public void PlaySFX(string eventName, GameObject soundGameObject = null){
        PlaySound($"Play_{eventName}", soundGameObject);
    }

    private void PlaySound(string eventName, GameObject soundGameObject){
        // 유효성 검사.. 어떻게 함

        if(soundGameObject == null)
        {
            soundGameObject = gameObject;
        }
        AkUnitySoundEngine.PostEvent(eventName, soundGameObject);
    }

    public void PauseSFX(){

    }
}