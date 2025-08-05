using System;

public class ComboSystem
{
    public int CurrentCombo { get; private set; }
    public int MaxCombo { get; private set; }

    // 이벤트: 콤보 수치가 변경될 때 UI 업데이트용
    public event Action<int> OnComboChanged;
    // 이벤트: 콤보가 깨졌을 때 UI 효과용
    public event Action OnComboBroken;

    public ComboSystem()
    {
        Reset();
        MaxCombo = 0;
    }

    // 배달 성공 시 호출
    public void RegisterSuccess()
    {
        Logger.Log("Combo!!");
        CurrentCombo++;
        if (CurrentCombo > MaxCombo)
        {
            MaxCombo = CurrentCombo;
        }
        OnComboChanged?.Invoke(CurrentCombo);
    }

    // 콤보 초기화 (마이너스 점수 받을 때 외부에서 호출)
    public void BreakCombo()
    {
        if (CurrentCombo > 1) // 1콤보는 깨졌다고 알릴 필요 없음
        {
            OnComboBroken?.Invoke();
        }
        Reset();
    }
    
    private void Reset()
    {
        CurrentCombo = 0;
        OnComboChanged?.Invoke(CurrentCombo);
    }

    // 현재 콤보에 따른 점수 배율 반환
    public float GetScoreMultiplier()
    {
        if (CurrentCombo < 5) return 1.0f;
        if (CurrentCombo < 10) return 1.2f;
        if (CurrentCombo < 20) return 1.5f;
        return 2.0f; // 20콤보 이상은 2배
    }
}
