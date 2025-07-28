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
    private Rect _groundRect;
    
    public MiniGameDeliveryPlayerController(Player player){ Init(player); }

    public void Init(Player player)
    {
        Player = player;
    }

    public void SetGroundSize(Rect size)
    {
        _groundRect = size;
    }
    
    public void InputJoyStick(Vector2 input)
    {
        if (Managers.MiniGame.CurrentGame.IsPause) return;

        float moveSpeed = Player.MoveSpeed;
        Vector3 delta = new Vector3(input.x, input.y, 0f) * (moveSpeed * Time.deltaTime);
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
        if(Managers.MiniGame.CurrentGame.IsPause){
           return; 
        }

        Logger.Log("Jump");
    }

    public bool ChangeInteraction(int actionnNum)
    {
        throw new System.NotImplementedException();
    }

    public void SetSkillList(SkillBase[] skillList)
    {
        throw new System.NotImplementedException();
    }
    public void OnSkill(int skillIndex)
    {
        throw new System.NotImplementedException();
    }
}
