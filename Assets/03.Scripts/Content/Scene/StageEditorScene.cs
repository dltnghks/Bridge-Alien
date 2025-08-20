using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEditorScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.StageEditor;

        Managers.MiniGame.LoadStageEditor();
        return true;
    }
}

