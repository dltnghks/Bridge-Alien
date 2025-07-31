using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameDelivery : MonoBehaviour, IMiniGame
{
	[Header("Game Setting")]
    [SerializeField] private MiniGameDeliverySetting _gameSetting;
    [SerializeField] private SkillBase[] _skillList;
    
    [Header("게임 종료 시간")]
    [SerializeField] private float gameTime = 1500f;

    [Header("플레이어 최대 이동 거리")]
    [SerializeField] private float maxDistance = .0f;
    [SerializeField] private float totalDistance = .0f;
    
    public bool IsActive { get; set; }
    public bool IsPause { get; set; }

    public Player PlayerCharacter { get; set; }
    public IPlayerController PlayerController { get; set; }
    
    public UIScene GameUI { get; set; }
    private UIGameDeliveryScene _uiGameDeliveryScene;
    private MiniGameDeliveryPathProgress _pathProgressBar;
    private DeliveryMap _deliveryMap;
    private DamageHandler _damageHandler;

    private UnityEvent<bool> _onChangeActive; 
    
    private void Update()
    {
        if (!IsActive || IsPause)
        {
            return;
        }

        totalDistance += Time.deltaTime * _deliveryMap.GroundSpeed;
        _pathProgressBar?.ProgressUpdate(totalDistance);
    }
    
    public void StartGame()
    {
        _pathProgressBar = new MiniGameDeliveryPathProgress();
        
        PlayerCharacter = Utils.FindChild<MiniGameDeliveryPlayer>(gameObject, "Player", true);
        if (PlayerCharacter == null)
        {
            Debug.Log("Player 캐릭터가 존재하지 않아요.");
            return;
        }
        
        _damageHandler = Utils.FindChild<DamageHandler>(gameObject, "Player", true);
        if (_damageHandler == null)
        {
            Debug.Log("Damage Handler를 찾을 수 없음.");
            return;
        }

        PlayerController = new MiniGameDeliveryPlayerController(PlayerCharacter, _damageHandler);
        if (PlayerController == null)
        {
            Debug.Log("플레이어 컨트롤러가 존재하지 않아요.");
            return;
        }
        
        _deliveryMap = Utils.FindChild<DeliveryMap>(gameObject, "Map", true);
        if (_deliveryMap == null)
        {
            Debug.Log("D.M이 존재하지 않습니다.");
            return;
        }
        
        if (PlayerController is ISkillController skillController)
        {
            _uiGameDeliveryScene.UIPlayerInput.SetSkillInfo(_skillList);
            _uiGameDeliveryScene.UIPlayerInput.SetSkillAction(skillController.OnSkill);

            ((MiniGameDeliveryPlayerController)PlayerController).onRocketAction = OnRocketSkill;
            skillController.SetSkillList(_skillList);
        }
        
        // Event Chain
        _onChangeActive = new UnityEvent<bool>();
        _onChangeActive?.AddListener(_deliveryMap.UpdateIsActive);
        
        // Initialize
        _deliveryMap.Initialize();
        _damageHandler.Initialize(() => Debug.Log("Damage Full"));
        
        ((MiniGameDeliveryPlayer)PlayerCharacter).damageHandler = _damageHandler;
        (PlayerController as MiniGameDeliveryPlayerController)?.SetGroundSize(_deliveryMap.GroundRect);
        
        ChangeActive(true);
        
        _pathProgressBar.SetProgressBar(_uiGameDeliveryScene.UIPathProgressBar, maxDistance, EndGame);
    }

    public void OnRocketSkill(bool isActive)
    {
        (PlayerCharacter as MiniGameDeliveryPlayer)?.OnRocketEffect(isActive);
        _deliveryMap.UpdateSpeedMultiplier(isActive ? 5f : 1f);
    }
    
    public bool PauseGame()
    {
        if (!IsActive || IsPause)
        {
            return false;
        }
        IsPause = true;
        return true;
    }

    public void ResumeGame()
    {
        if (!IsActive || !IsPause)
        {
            return;
        }
        IsPause = false;
    }

    public void EndGame()
    {
        if (!IsActive)
        {
            return;
        }
        
        IsActive = false;
        Managers.UI.ShowPopUI<UIGameUnloadResultPopup>().SetResultScore((int)_pathProgressBar.CurValue * 100);
    }
    
    public void InitializeUI()
    {
        _uiGameDeliveryScene = Managers.UI.ShowSceneUI<UIGameDeliveryScene>();
        GameUI = _uiGameDeliveryScene;
        GameUI.Init();
    }

    public void ChangeActive(bool flag)
    {
        IsActive = flag;
        _onChangeActive?.Invoke(IsActive);
    }

    public void UpdateTotalDistance(float distance)
    {
        totalDistance = distance;
    }

    public float GetDistance()
    {
        return totalDistance;
    }

    private void UpdateDistanceFromMap(float distance)
    {
        UpdateTotalDistance(distance);
    }
}
