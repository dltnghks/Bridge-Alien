using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMiniGame
{
    bool IsActive { get; set; }
    bool IsPause { get; set; }
    UIScene GameUI { get; set; }
    
    void StartGame();    // 게임 시작
    void PauseGame();    // 게임 일시정지
    void ResumeGame();   // 게임 재개
    void EndGame();      // 게임 종료
    
    void InitializeUI() {}  // UI 초기화
}
