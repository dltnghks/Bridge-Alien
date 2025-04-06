using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UITaskGroup : UISubItem
{
    enum Buttons
    {
        TaskButton,   
    }

    enum Objects
    {
        TaskButtonGroup,
        ScrollbarVertical,
    }

    private List<UITaskButton> _taskButtons = new List<UITaskButton>();
    
    private UIPlayerTaskPopup _controller;
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));
        BindObject(typeof(Objects));
        
        _taskButtons.Add(GetButton((int)Buttons.TaskButton).GetOrAddComponent<UITaskButton>());
        
        return true;
    }

    public void Init(UIPlayerTaskPopup controller)
    {
        Init();
        _controller = controller;
        GetButton((int)Buttons.TaskButton).GetOrAddComponent<UITaskButton>().Init(_controller);
    }
    
    // 데이터가 들어오면 task button 세팅해주기
    public void Setup(Define.TaskType type)
    {
        Init();
        // type으로 데이터 가져오기
        //List<string> tasks = Managers.Data.Task.GetData(type);
        List<string> tasks = new List<string>(){$"{type}", "test2", "test3"};
        
        int i = 0;
        for (i = 0; i < tasks.Count; i++)
        {
            if (_taskButtons.Count <= i)
            {
                AddTaskButton();
            }
            _taskButtons[i].gameObject.SetActive(true);
            _taskButtons[i].SetData(tasks[i]);
        }

        for (; i < _taskButtons.Count; i++)
        {
            _taskButtons[i].gameObject.SetActive(false);
        }
    }

    private void AddTaskButton()
    {
        GameObject taskButtonPrefab = GetButton((int)Buttons.TaskButton).gameObject;
        GameObject newTaskButtonObj = Managers.Resource.Instantiate(taskButtonPrefab, GetObject((int)Objects.TaskButtonGroup).transform);
        UITaskButton newTaskButton = newTaskButtonObj.GetComponent<UITaskButton>();
        newTaskButton.Init(_controller);
        _taskButtons.Add(newTaskButton);
    }
    
}
