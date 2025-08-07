using System;
using System.Collections.Generic;

public class ComboSystem
{
    public int CurrentCombo { get; private set; }
    public int MaxCombo { get; private set; }

    // 이벤트: 콤보 수치가 변경될 때 UI 업데이트용
    public event Action<int> OnComboChanged;
    // 이벤트: 콤보가 깨졌을 때 UI 효과용
    public event Action OnComboBroken;
    public event Action<MiniGameUnloadBoxList> OnChangedComboBox;
    // 이벤트: 콤보 박스가 가득 찼을 때
    public event Action OnComboBoxFull;

    public MiniGameUnloadBoxList _comboBoxList = new MiniGameUnloadBoxList();

    public ComboSystem()
    {
        Reset();
        MaxCombo = 0;
        _comboBoxList.SetBoxList(3);
    }

    // 배달 성공 시 호출
    public void RegisterSuccess(MiniGameUnloadBox box)
    {
        Logger.Log("Combo!!");
        CurrentCombo++;
        if (CurrentCombo > MaxCombo)
        {
            MaxCombo = CurrentCombo;
        }
        OnComboChanged?.Invoke(CurrentCombo);
        AddComboBox(box);
    }

    public void AddComboBox(MiniGameUnloadBox box, bool success = true)
    {
        if (_comboBoxList.IsFull)
        {
            // 이미 꽉 차있다면 더 이상 박스를 추가하지 않음
            // 또는 여기서 바로 클리어하고 새로 시작하게 할 수도 있습니다.
            // 지금은 추가 동작을 막는 것으로 구현
            return;
        }
        
        if (success)
        {
            _comboBoxList.TryPush(box);
        }
        else
        {
            _comboBoxList.TryPush(null);
        }
        
        OnChangedComboBox?.Invoke(_comboBoxList);

        if (_comboBoxList.IsFull)
        {
            OnComboBoxFull?.Invoke();
        }
    }
    
    // 콤보 박스 리스트를 비우는 함수
    public void ClearComboBoxList()
    {
        _comboBoxList.ClearBoxList();
        OnChangedComboBox?.Invoke(_comboBoxList);
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