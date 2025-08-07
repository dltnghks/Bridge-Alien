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

    public void AddComboBox(MiniGameUnloadBox box)
    {

        if (_comboBoxList.TryPush(box))
        {
            OnChangedComboBox?.Invoke(_comboBoxList);

            if (_comboBoxList.IsFull)
            {
                OnComboBoxFull?.Invoke();
            }   
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
        _comboBoxList.ClearBoxList();
        CurrentCombo = 0;
        OnComboChanged?.Invoke(CurrentCombo);
        OnChangedComboBox?.Invoke(_comboBoxList);
    }
    
}