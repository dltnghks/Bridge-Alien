using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBase 
{
    private UIScoreBoard _uiScoreBoard;

    private int _score = 0;

    public UIScoreBoard ScoreBoard { get { return _uiScoreBoard; } }
    public int CurrentScore { get { return _score; } }
    
    public void SetScore(UIScoreBoard uiScoreBoard, int score)
    {
        _uiScoreBoard = uiScoreBoard;
        
        _score = score;
        ScoreBoard.SetScore(CurrentScore);
    }
    
    public void AddScore(int score)
    {
        _score += score;
        
        if(_score <= 0) _score = 0;

        ScoreBoard.SetScore(CurrentScore);
    }
}
