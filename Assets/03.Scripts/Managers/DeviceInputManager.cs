
using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 모바일 뒤로가기, 홈, 앱 전환 등 시스템 레벨의 입력을 중앙에서 관리하는 싱글톤 매니저.
/// 뒤로가기 버튼의 동작은 스택(Stack)을 이용해 우선순위를 관리합니다.
/// </summary>
public class DeviceInputManager : MonoBehaviour
{
    public void Init()
    {

    }

    /// <summary>
    /// 뒤로가기 버튼을 눌렀을 때 실행될 액션(함수)들을 담는 스택.
    /// 가장 마지막에 추가된 액션이 가장 먼저 실행됩니다. (LIFO: Last-In, First-Out)
    /// </summary>
    private Stack<Action> _backButtonActions = new Stack<Action>();

    /// <summary>
    /// 앱이 일시정지되거나 다시 활성화될 때 발생하는 이벤트.
    /// bool 값은 일시정지 상태(true)인지, 활성화 상태(false)인지를 나타냅니다.
    /// </summary>
    public event Action<bool> OnApplicationPaused;

    private void Update()
    {
        // 안드로이드의 뒤로가기 버튼 또는 PC의 ESC 키 입력을 감지합니다.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 스택에 등록된 액션이 있는지 확인합니다.
            if (_backButtonActions.Count > 0)
            {
                // 스택의 가장 위(가장 최근에 등록된) 액션을 꺼내서 실행합니다.
                Action lastAction = _backButtonActions.Pop();
                lastAction?.Invoke();
            }
            else
            {
                // 로딩이 아닐 때만 상호작용
                if (Managers.Scene.IsLoding == false)
                {
                    // 스택에 아무것도 없으면 기본 동작을 수행합니다.
                    // 여기서는 앱 종료 확인 팝업을 띄우거나, 바로 종료할 수 있습니다.
                    // UI 띄우기
                    Managers.UI.ShowPopUI<UIGameMenuPopup>();
                }
            }
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (Managers.Scene.CurrentSceneType != Define.Scene.Title &&
        Managers.Scene.CurrentSceneType != Define.Scene.Dev &&
        Managers.Scene.CurrentSceneType != Define.Scene.Unknown 
        )
        {
            Managers.Save.Save();    
        }
        // 홈 버튼, 앱 전환, 전화 수신 등으로 앱의 활성 상태가 변경될 때 이벤트를 발생시킵니다.
        OnApplicationPaused?.Invoke(pauseStatus);
    }

    /// <summary>
    /// 뒤로가기 버튼에 대한 새로운 액션을 스택에 추가합니다.
    /// UI 팝업이나 메뉴가 열릴 때 이 함수를 호출하여 자신의 '닫기' 함수를 등록합니다.
    /// </summary>
    /// <param name="action">뒤로가기 버튼을 눌렀을 때 실행될 함수</param>
    public void PushBackButtonAction(Action action)
    {
        _backButtonActions.Push(action);
    }

    /// <summary>
    /// 스택의 가장 위에 있는 액션을 제거합니다.
    /// UI가 뒤로가기 버튼이 아닌 다른 방식으로 닫혔을 때, 스택에 등록된 액션을 정리하기 위해 사용합니다.
    /// </summary>
    public void PopBackButtonAction()
    {
        if (_backButtonActions.Count > 0)
        {
            _backButtonActions.Pop();
        }
    }
}
