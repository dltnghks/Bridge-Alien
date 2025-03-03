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
       
        Managers.Sound.PlayBGM(sceneTypeStr);
    
        // 기본적으로 DataLoadingScene에서 데이터를 로드하지만, 로드되지 않은 경우 여기서 함.
        Managers.Data.LoadAllData();
        
       
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
