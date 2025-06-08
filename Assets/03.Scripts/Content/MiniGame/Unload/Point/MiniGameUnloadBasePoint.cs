using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameUnloadBasePoint : MonoBehaviour
{
    public Define.BoxType[] AllowedTypes; // 허용할 상자 타입 배열

    public virtual bool CanProcess(Define.BoxType boxType)
    {
        foreach (var t in AllowedTypes)
        {
            if (t == boxType) return true;
        }
        return false;
    }

    public abstract bool  ProcessBox(GameObject box);
}
