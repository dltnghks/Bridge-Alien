using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IMiniGame
{
    bool IsActive { get; set; }
    bool IsPause { get; set; }
    // 자기 씬 UI는 따로 선언하기
    UIScene GameUI { get; set; }
    
    Player PlayerCharacter { get; set; }
    IPlayerController PlayerController { get; set; }
    
    void StartGame();    // 게임 시작
    void PauseGame();    // 게임 일시정지
    void ResumeGame();   // 게임 재개
    void EndGame();      // 게임 종료
    void InitializeUI() {}  // UI 초기화
}
