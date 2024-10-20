using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private int _order = 0;
    private Stack<UIPopup> _popupStack = new Stack<UIPopup>();

    private readonly string _subItemPath = "Prefab/UI/SubItem/";

    public UIScene SceneUI
    {
        get;
        private set;
    }
    
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                root = new GameObject(name = "@UI_Root");
            }

            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            _order++;
            canvas.sortingOrder = _order;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UIBase
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject prefab = Managers.Resource.Load<GameObject>($"{_subItemPath + name}");

        GameObject go = Managers.Resource.Instantiate(prefab);
        if (parent != null)
        {
            go.transform.SetParent(parent);
        }

        go.transform.localScale = Vector3.one;
        go.transform.localPosition = prefab.transform.position;
        
        return Utils.GetOrAddComponent<T>(go);
    }

    public T ShowSceneUI<T>(string name = null) where T : UIScene
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject go = GameObject.Find($"{name}");
        T sceneUI = Utils.GetOrAddComponent<T>(go);
        SceneUI = sceneUI;
        
        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public T ShowPopUI<T>(string name = null, Transform parent = null) where T : UIPopup
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject prefab = Managers.Resource.Load<GameObject>($"Prefab/UI/Popup/{name}");
        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");

        T popup = Utils.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        if (parent != null)
        {
            go.transform.SetParent(parent);
        }
        else if (SceneUI != null)
        {
            go.transform.SetParent(SceneUI.transform);
        }
        else
        {
            go.transform.SetParent(Root.transform);
        }

        go.transform.localScale = Vector3.one;
        go.transform.localPosition = prefab.transform.position;

        return popup;
    }
    
    public T FindPopup<T>() where T : UIPopup
    {
        return _popupStack.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T;
    }

    public void ClosePopupUI(UIPopup popup)
    {
        if (_popupStack.Count == 0)
        {
            return;
        }

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }
        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
        {
            return;
        }

        UIPopup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

    public void CloseSceneUI()
    {
        SceneUI.gameObject.SetActive(false);
    }

    public void LoadScene(string loadSceneName)
    {
        SceneManager.LoadScene(loadSceneName);
    }
    
    
}


