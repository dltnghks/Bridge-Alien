using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager
{
    private int _order = 0;
    private Stack<UIPopup> _popupStack = new Stack<UIPopup>();
    private Queue<PopupRequestInfo> _popupReservationQueue = new Queue<PopupRequestInfo>();

    private readonly string _subItemPath = "Prefab/UI/SubItem/";

    private UIBlurBackground _blurBackground = null;
    
    public UIScene SceneUI
    {
        get;
        private set;
    }
    
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UIRoot");
            if (root == null)
            {
                root = new GameObject { name = "@UIRoot" };
            }

            return root;
        }
    }

    public void Init()
    {
        _blurBackground = null;
    }
    
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);
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
        SceneUI.Init();
        
        go.transform.SetParent(Root.transform);

        
        // 백그라운드 블러 생성
        // TODO : 씬이 변경될 때마다 만들고 있음. Manager에 붙여놓고 사용가능하도록?
        _blurBackground = null;
        if (_blurBackground == null)
        {
            _blurBackground = ShowPopUI<UIBlurBackground>();
            _blurBackground.Init();
            _popupStack.Pop();
            
        }
        _blurBackground.SetActive(false);
        
        return sceneUI;
    }
    
    // 팝업 예약
    public void RequestPopup<T>(object data = null, string name = null, Transform parent = null, bool reserve = false) where T : UIPopup
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        PopupRequestInfo request = new PopupRequestInfo(typeof(T), name, parent, data);

        _popupReservationQueue.Enqueue(request);
        Logger.Log($"Popup Reserved: {request.Name}");
        // 현재 활성화된 팝업이 없을 경우, 예약된 팝업 즉시 시도
        if (_popupStack.Count == 0)
        {
            ShowNextReservedPopup();
        }
    }
    
    public T ShowPopUI<T>(string name = null, Transform parent = null, bool IsblurBG = true) where T : UIPopup
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

        // blurbackground가 꺼져있는 경우에만 켜기
        // blurbackground가 켜져있는데 끄고 싶은 경우에는 끄기
        if ((IsblurBG == true && _blurBackground?.IsBlurActive == false) ||
        (IsblurBG == false && _blurBackground.IsBlurActive == true))
        {
            _blurBackground?.SetActive(IsblurBG);
        }
    
        return popup;
    }
    
    // System.Type을 파라미터로 받는 버전
    private UIPopup ShowPopUIByType(PopupRequestInfo request)
    {
        string name = request.Name;
        System.Type popupType = request.Type;

        GameObject prefab = Managers.Resource.Load<GameObject>($"Prefab/UI/Popup/{name}");
        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        
        // Component 추가 (System.Type 사용)
        // Utils.GetOrAddComponent의 Type 버전 사용, 반환 타입은 Component이므로 캐스팅 필요
        UIPopup popupInstance = Utils.GetOrAddComponent(go, popupType) as UIPopup;
        if (popupInstance == null)
        {
            Managers.Resource.Destroy(go); // 컴포넌트 추가 실패 시 인스턴스 파괴
            Logger.LogError($"Failed to get or add UIPopup component of type {popupType.Name} to GameObject {name}");
            return null;
        }

        // 데이터 전달
        if (request.Data != null && popupInstance)
        {
            popupInstance.Init(request.Data);
        }
        
        
        _popupStack.Push(popupInstance);

        // 부모 설정
        Transform actualParent = request.Parent;
        if (actualParent == null)
        {
            actualParent = SceneUI != null ? SceneUI.transform : Root.transform;
        }
        go.transform.SetParent(actualParent);

        // 스케일 및 위치 설정
        go.transform.localScale = Vector3.one;
        // 로컬 포지션은 프리팹 원본을 따르거나 (0,0,0) 등으로 설정
        go.transform.localPosition = prefab.transform.position; // 또는 Vector3.zero 등
        
        return popupInstance;
    }
    
    private void ShowNextReservedPopup()
    {
        if (_popupReservationQueue.Count > 0)
        {
            if (_popupStack.Count == 0) // 현재 활성화된 팝업이 없을 때만 다음 예약 팝업을 보여줌
            {
                PopupRequestInfo nextRequest = _popupReservationQueue.Dequeue();
                Logger.Log($"Processing Reserved Popup: {nextRequest.Name}");
                ShowPopUIByType(nextRequest);
            }
            else
            {
                Logger.Log("Cannot show reserved popup, another popup is active.");
            }
        }
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
            Logger.Log("Close Popup Failed!");
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
        _order--;
        
        
        if (_popupStack.Count <= 0) // 모든 팝업이 닫혔다면
        {
            ShowNextReservedPopup();
        }

        if (_popupStack.Count == 0)
        {
            _blurBackground?.SetActive(false);
        }
    }

    // 자동으로 꺼지거나 배경 클릭으로 종료하고 싶지않은 팝업의 경우, false
    // 해당 팝업이 종료되면 true로 다시 변경해줘야 됨.
    public void SetInputBackground(bool isEnabled)
    {
        if (_blurBackground == null)
        {
            return;
        }
        _blurBackground.IsInputEnabled = isEnabled;
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


