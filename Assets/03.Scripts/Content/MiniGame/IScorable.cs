using System;

public interface IScorable
{
    UIScoreBoard ScoreBoard { get; set; }
    int CurrentScore { get; set; }

    void SetScore(UIScoreBoard uiScoreBoard, int score);        // 점수 세팅
    void AddScore(int score);        // 점수 증가 메서드
}