using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageEditorScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.StageEditor;

#if UNITY_EDITOR
        var stageType = GetStageTypeFromEditorPrefs();
        if (stageType.HasValue)
        {
            Managers.Stage.SetCurrentStage(stageType.Value);
        }
#endif

        Managers.MiniGame.LoadStageEditor();
        return true;
    }

#if UNITY_EDITOR
    private Define.ChapterType? GetStageTypeFromEditorPrefs()
    {
        string selectedStageName = EditorPrefs.GetString("StageEditor_SelectedStage", "");
        if (string.IsNullOrEmpty(selectedStageName))
        {
            return null;
        }

        if (System.Enum.TryParse<Define.ChapterType>(selectedStageName, out var stageType))
        {
            return stageType;
        }

        return null;
    }
#endif
}

