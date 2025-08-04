using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBase 
{
    private int _score = 0;
    public int CurrentScore { get { return _score; } }

    public Action<int> OnChangedScore;

    public void SetScore(int score)
    {
        _score = score;
        OnChangedScore?.Invoke(_score);
    }

    public void AddScore(int score)
    {
        _score += score;

        if (_score <= 0) _score = 0;
        
        OnChangedScore?.Invoke(_score);
    }
}
