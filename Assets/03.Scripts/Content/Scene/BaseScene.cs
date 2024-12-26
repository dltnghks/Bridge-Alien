using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType = Define.Scene.Unknown;
    protected bool _init = false;

    private void Start()
    {
        Init();

        string sceneTypeStr =  System.Enum.GetName(typeof(Define.Scene), SceneType);
        Managers.Sound.UnloadAllSoundBank();
        if (SceneType == Define.Scene.MiniGameUnload)
        {
            Managers.Sound.LoadSoundBank(sceneTypeStr);
            Managers.Sound.PlayBGM(sceneTypeStr);
        }
    }

    protected virtual bool Init()
    {
        if (_init)
            return false;

        _init = true;
        GameObject go = GameObject.Find("EventSystem");
        if (go == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";

        return true;
    }

    public virtual void Clear() { }
}
