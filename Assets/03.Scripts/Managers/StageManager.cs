using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private StageData _currentStage;
    public void Init()
    {
        _currentStage = null;
    }

    public void LoadStage(Define.StageType stageType)
    {
        var stageData = Managers.Data.StageData.GetStageData(stageType);
        if (stageData is null)
        {
            return;
        }

        _currentStage = stageData;
    }

    public void CompleteStage(Define.StageType stageType, int playerScore) {
        int starCount = 0;

        foreach (var score in _currentStage.ClearScoreList)
        {
            if (playerScore >= score)
            {
                starCount++;
            }
        }

        Managers.Player.SaveStageProgress(stageType, starCount);
    }

    public bool CheckStageLockStatus(Define.StageType stageType)
    {
        if (_currentStage.IsLocked == true) {
            return true;
        }

        if (_currentStage.RequiredStars >= Managers.Player.GetTotalStars())
        {
            return true;
        }


        return false;
    }

}
