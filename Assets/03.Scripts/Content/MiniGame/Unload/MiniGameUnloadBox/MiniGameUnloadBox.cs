using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;

[System.Serializable]
public struct MiniGameUnloadBoxInfo
{   
    public string BoxNumber;
    public Define.BoxType BoxType;
    public Define.BoxState BoxState;
    public Define.BoxRegion Region;
    public bool IsGrab;
    public bool IsBroken;
    public bool IsUnloaded;

    public MiniGameUnloadBoxInfo(string boxNumber, Define.BoxRegion region)
    {
        BoxNumber = boxNumber;
        Region = region;
        BoxState = Define.BoxState.Normal;
        IsBroken = false;
        IsGrab = false;
        IsUnloaded = false;
        BoxType = Define.BoxType.Common;
    }

    // 랜덤 정보 생성
    public void SetRandomInfo()
    {
        BoxNumber = GenerateRandomString();  // AAA-0000형태

        Region = (Define.BoxRegion)Random.Range(0, (int)Define.BoxRegion.D + 1); // 지역 선택
    }
    
    private string GenerateRandomString()
    {
        // 알파벳과 숫자를 랜덤으로 생성하여 결합
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string digits = "0123456789";

        // 3개의 알파벳과 4개의 숫자 생성 후 결합
        return $"{GetRandomChars(letters, 3)}-{GetRandomChars(digits, 4)}";
    }

    private string GetRandomChars(string charSet, int length)
    {
        // 랜덤 생성기
        System.Random random = new System.Random();
        
        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = charSet[random.Next(charSet.Length)];
        }
        return new string(result);
    }

    public string GetBoxRegion()
    {
        return Region.ToString();
    }
}

public class MiniGameUnloadBox : MonoBehaviour
{
    protected int _defaultBoxLayer;
    protected int _grabBoxLayer;

    [SerializeField] protected MiniGameUnloadBoxInfo _info;
    
    protected Rigidbody boxRigidbody;
    protected BoxCollider boxCollider;

    public MiniGameUnloadBoxInfo Info
    {
        get => _info;
        private set => _info = value;
    }

    public bool IsUnloaded
    {
        get { return _info.IsUnloaded; } 
        set { _info.IsUnloaded = value; }
    }

    public Define.BoxState BoxState
    {
        get { return _info.BoxState; } 
        set { _info.BoxState = value; }
    }
    
    public Define.BoxType BoxType
    {
        get { return _info.BoxType; }
        set { _info.BoxType = value; }
    }

    protected virtual void Init()
    {
        boxRigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public void SetInGameActive(bool value, Vector3 pos = default(Vector3))
    {
        _defaultBoxLayer = LayerMask.NameToLayer("DefaultBox");
        _grabBoxLayer = LayerMask.NameToLayer("GrabBox");


        gameObject.SetActive(value);
        if (value)
        {
            transform.position = pos;
            transform.localRotation = Quaternion.Euler(Vector3.zero);

            Vector3 currentScale = boxCollider.size;
            currentScale.x = 1f;
            currentScale.z = 1f;
            currentScale.y = 0.8f;
            // 콜라이더 크기 설정 

            boxCollider.size = currentScale;

            // offset 계산 및 적용
            // float frontHeight = 100f;   // 앞면 높이
            // float totalHeight = 100f + 100f * 0.4f; // 전체 높이

            boxCollider.size = currentScale;

            // 생성될 때는 false
            boxCollider.isTrigger = false;
            IsUnloaded = false;

            // 이전 중력이 남아있음. -> 속도 초기화
            boxRigidbody.velocity = Vector3.zero;

            PlayBoxPutSound();

        }
    }

    public void SetSpawnBox(Vector3 spawnPos)
    {
        SetInGameActive(true, spawnPos);
        boxRigidbody.constraints = RigidbodyConstraints.FreezeAll ^ RigidbodyConstraints.FreezePositionY;
    }

    public void SetIsGrab(bool value)
    {
        _info.IsGrab = value;
        if (value)
        {
            gameObject.layer = _grabBoxLayer;

            // 상자의 Rigidbody 비활성화
            boxRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            // 놓을 때는 true
            boxCollider.isTrigger = true;

            PlayBoxHoldSound();

        }
        else
        {
            gameObject.layer = _defaultBoxLayer;

            // 상자의 Rigidbody 활성화
            boxRigidbody.constraints = RigidbodyConstraints.FreezeAll;

            PlayBoxPutSound();
        }
    }

    private void PlayBoxHoldSound()
    {
        Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.BoxHold.ToString()); 
    }
    
    private void PlayBoxPutSound()
    {
        Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.BoxPut.ToString(),gameObject); 
    }

    public virtual void SetRandomInfo()
    {
        Init();
        _info.SetRandomInfo();
    }
}

