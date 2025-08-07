using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameUnloadBasePoint : MonoBehaviour
{
    public Define.BoxType[] AllowedTypes; // 허용할 상자 타입 배열

    public Action<int> OnTriggerAction;
    public Action<int, MiniGameUnloadBox> OnScoreAction;

    // 포인트와 상호작용할 수 있는 박스타입인지 확인
    public virtual bool CanProcess(Define.BoxType boxType)
    {
        foreach (var t in AllowedTypes)
        {
            if (t == boxType) return true;
        }
        return false;
    }
}
