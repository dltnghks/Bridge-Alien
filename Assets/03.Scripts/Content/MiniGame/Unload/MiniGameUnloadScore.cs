using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MiniGameUnloadScore : IScorable
{
    public UIScoreBoard ScoreBoard { get; set; }
    public int CurrentScore { get; set; }
    public void SetScore(UIScoreBoard uiScoreBoard, int score)
    {
        ScoreBoard = uiScoreBoard;
        
        CurrentScore = score;
        ScoreBoard.SetScore(CurrentScore);
    }

    public void AddScore(int score)
    {
        CurrentScore += score;
        ScoreBoard.SetScore(CurrentScore);
    }
}
