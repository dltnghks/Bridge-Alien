using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameDeliveryPlayerController : IPlayerController, ISkillController
{
    // IPlayerController
    public Player Player { get; set; }
    public int InteractionActionNumber { get; set; }
    
    // ISkillController
    public SkillBase[] SkillList { get; set; }
    
    // ETC
    private MiniGameDeliveryPlayer _mgPlayer;
    private Rect _groundRect;
    private DamageHandler _damageHandler;
    [NonSerialized] public Action<bool> onRocketAction; 

    public MiniGameDeliveryPlayerController(Player player, DamageHandler damageHandler)
    {
        _damageHandler = damageHandler;
        Init(player);
    }

    public void Init(Player player)
    {
        Player = player;
        _mgPlayer = Player as MiniGameDeliveryPlayer;
        _mgPlayer.SetUp(_damageHandler);
    }

    public void SetSkillList(SkillBase[] skillList)
    {
        SkillList = skillList;

        MGDSkillContext context = new MGDSkillContext(
            Player,
            _mgPlayer,
            onRocketSkillAction : onRocketAction,
            _damageHandler.OnResetDamage
            );

        // Context 내부에는 SKill 사용 시 발동할 메서드가 포함이 되어야 한다.
        foreach (var skill in skillList)
        {
            skill.Initialize(context);
        }
    }
    
    public void SetGroundSize(Rect size)
    {
        _groundRect = size;
    }

    public void InputJoyStick(Vector2 input)
    {
        if (Managers.MiniGame.CurrentGame.IsPause)
        {
            Debug.Log("Player Pause");
            return;
        }

        Debug.Log("Player Speed: " + Player.MoveSpeed * _damageHandler.SpeedPenalty);
        PlayerMovement(input);
    }

    private void PlayerMovement(Vector2 inputData)
    {
        float moveSpeed = Player.MoveSpeed * _damageHandler.SpeedPenalty;
        Vector3 delta = new Vector3(inputData.x, inputData.y, 0f) * (moveSpeed * Time.deltaTime);
        Vector3 targetPos = Player.transform.position + delta;

        Vector2 center = _groundRect.center;
        // 80% 제한, Magic Number.
        Vector2 size = _groundRect.size * 0.8f;

        float minX = center.x - size.x / 2f;
        float maxX = center.x + size.x / 2f;
        float minY = center.y - size.y / 2f;
        float maxY = center.y + size.y / 2f;

        // 위치 제한
        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        Player.transform.position = targetPos;
    }

    public void Interaction()
    {
        if(Managers.MiniGame.CurrentGame.IsPause)
        {
           return; 
        }
        // 플레이어 속도가 일시적으로 감속되며, Bump 장애물을 피할 수 있다.
    }

    public bool ChangeInteraction(int actionnNum)
    {
        throw new System.NotImplementedException();
    }

    public void OnSkill(int skillIndex)
    {
        if (Managers.MiniGame.CurrentGame.IsPause)
            return;
        if (skillIndex < 0 || skillIndex >= SkillList.Length)
            return;
        
        SkillList[skillIndex].TryActivate();
    }
}
