using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public enum MiniGameUnloadInteractionAction
{
    None,
    PickUpBox,
    DropBox,   
}

public class MiniGameUnloadPlayerController : IPlayerController, ISkillController
{
    private MiniGameUnloadBoxList _boxList = new MiniGameUnloadBoxList();
    private float _interactionRange = 5f;
    private float _boxHeight = 0f;
    private float _boxOffset = 0.8f;
    private float _moveSpeedReductionRatio = 2.0f;
    private float _moveSpeedBonus = 1f;
    private List<MiniGameUnloadBasePoint> _cachedPoints = new List<MiniGameUnloadBasePoint>();
    private UnityAction<List<MiniGameUnloadBox>> OnBoxListChanged;
    private bool _isPointsCached;
    private MiniGameUnloadPlayer _unloadPlayer;
    private MiniGameUnloadBasePoint _interactionObject = null;
    private Camera _mainCamera;
    public Player Player { get; set ; }
    public int InteractionActionNumber { get; set; }
    public bool IsDropBox { get; set; }
    public SkillBase[] SkillList { get; set; }

    public MiniGameUnloadPlayerController(){}
    public MiniGameUnloadPlayerController(Player player, float radius, float moveSpeedReductionRatio, UnityAction<List<MiniGameUnloadBox>> OnBoxListChangedAction)
    {
        Init(player);
        _moveSpeedReductionRatio = moveSpeedReductionRatio;
        _moveSpeedBonus = Managers.Player.GetGravityAdaptationBonusMultiplier();

        OnBoxListChanged = OnBoxListChangedAction;
    }

    public void Init(Player player)
    {
        Player = player;
        _unloadPlayer = Player as MiniGameUnloadPlayer;
        _unloadPlayer.SpeedUpMultiplier(_moveSpeedBonus);
        _mainCamera = Camera.main;

        _boxList.SetBoxList(3);
        InteractionActionNumber = (int)MiniGameUnloadInteractionAction.None;
        CacheAllPoints();
    }

    public void SetSkillList(SkillBase[] skillList)
    {
        SkillList = skillList;

        var context = new MGUSkillContext(
            Player,
            _unloadPlayer,
            _boxList,
            _cachedPoints,
            RemoveBoxFromPlayer,
            _unloadPlayer.SetCoolingSkill,
            _unloadPlayer.SetSpeedUpSkill
        );

        foreach (var skill in SkillList)
        {
            skill.Initialize(context);
        }
    }

    public void OnSkill(int skillIndex)
    {
        if (!Managers.MiniGame.IsAcitvePlayer)
        {
            return;
        }

        if (SkillList == null || skillIndex < 0 || skillIndex >= SkillList.Length)
        {
            Logger.LogError($"Invalid skill index: {skillIndex}");
            return;
        }

        string stageId = Managers.Stage.CurrentStageType.ToString();
        SkillBase skill = SkillList[skillIndex];
        bool canUse = skill != null && skill.CanUseSkill();
        if (!canUse)
        {
            skill?.TryActivate();
            return;
        }

        string skillType = GetSkillTypeName(skill);
        int skillLevel = GetSkillLevel(skill);

        skill.TryActivate();
        Managers.Analytics.TrackSkillUse(stageId, "Unload", skillType, skillLevel, true, null);
    }

    private static string GetSkillTypeName(SkillBase skill)
    {
        if (skill is DurationSkill durationSkill)
        {
            return durationSkill.SkillData.Type.ToString();
        }

        if (skill is ChargeSkill chargeSkill)
        {
            return chargeSkill.SkillData.Type.ToString();
        }

        return skill?.GetType().Name ?? "Unknown";
    }

    private static int GetSkillLevel(SkillBase skill)
    {
        if (skill is DurationSkill durationSkill)
        {
            return Managers.Player.GetSkillLevel(durationSkill.SkillData.Type);
        }

        if (skill is ChargeSkill chargeSkill)
        {
            return Managers.Player.GetSkillLevel(chargeSkill.SkillData.Type);
        }

        return 0;
    }


    private void CacheAllPoints()
    {
        if (_isPointsCached) return;
        _cachedPoints.Clear();
        _cachedPoints.AddRange(Object.FindObjectsOfType<MiniGameUnloadBasePoint>());

        foreach (var item in Object.FindObjectsOfType<MiniGameUnloadBasePoint>())
        {
            Logger.Log(item);
        }
        _isPointsCached = true;
    }

    public void InputJoyStick(Vector2 input)
    {
        if (!Managers.MiniGame.IsAcitvePlayer)
        {
            return;
        }

        input = input - (input * (_boxList.CurrentUnloadBoxIndex * (_moveSpeedReductionRatio / 100.0f)));
        input = ConvertToCameraRelativeInput(input);
        Player.PlayerMovement(input);
    }

    private Vector2 ConvertToCameraRelativeInput(Vector2 input)
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        if (_mainCamera == null)
        {
            return input;
        }

        Vector3 camRight = _mainCamera.transform.right;
        Vector3 camForward = _mainCamera.transform.forward;
        camRight.y = 0f;
        camForward.y = 0f;

        if (camRight.sqrMagnitude < 0.0001f || camForward.sqrMagnitude < 0.0001f)
        {
            return input;
        }

        camRight.Normalize();
        camForward.Normalize();

        Vector3 worldMove = (camRight * input.x) + (camForward * input.y);
        float horizontal = Vector3.Dot(worldMove, Player.transform.right);
        float vertical = Vector3.Dot(worldMove, Player.transform.forward);
        return new Vector2(horizontal, vertical);
    }
    public void Interaction()
    {
        if (!Managers.MiniGame.IsAcitvePlayer || InteractionActionNumber == (int)MiniGameUnloadInteractionAction.None)
        {
            return;
        }
        
        FindNearestValidPoint();
        var interactionPoint = _interactionObject;
        if(interactionPoint == null)
        {
            return;
        }

        if(!CheckTutorialObject())
        {
            return;
        }

        switch ((MiniGameUnloadInteractionAction)InteractionActionNumber)
        {
            case MiniGameUnloadInteractionAction.None:
                Logger.LogWarning($"{InteractionActionNumber} : Undefined Interaction");
                break;
            case MiniGameUnloadInteractionAction.PickUpBox:
                PickupBox();
                break;
            case MiniGameUnloadInteractionAction.DropBox:
                DropBox();
                break;
            default:
                Logger.LogWarning($"{InteractionActionNumber} : Undefined Interaction");
                break;
        }

        OnBoxListChanged?.Invoke(_boxList.BoxList);
    }

    private bool CheckTutorialObject()
    {
        if(TutorialManager.Instance != null && Managers.MiniGame.CurrentGame.IsTutorialActive)
        {
            var interactionObject = _interactionObject.GetComponent<InteractableObject>();
            string myTag = interactionObject.myTag; 
            string allowed = TutorialManager.Instance.AllowedInteractableTag;

            Logger.Log($"MyTag: {myTag}, AllowedTag: {allowed} {myTag==allowed}");
            
            // 허용된 태그가 비어있지 않은데, 내 태그랑 다르다면 무시
            if (allowed == "" || allowed != myTag)
            {
                Logger.Log("지금은 이 오브젝트와 상호작용할 수 없습니다.");
                return false; 
            }
            // 허용된 오브젝트이면
            else
            {
                Logger.Log("허용된 오브젝트와 상호작용 완료.");
                TutorialManager.Instance.CompleteCurrentStep();
                return true;
            }
        }
        
        return true;
    }

    public bool ChangeInteraction(int actionNum)
    {
        if (actionNum == (int)MiniGameUnloadInteractionAction.DropBox &&
            _boxList.IsEmpty)
        {
            Logger.Log("Drop box list is empty");
            return false;
        }

        InteractionActionNumber = actionNum;
        return true;
    }

    private void PickupBox()
    {
        if (_boxList.IsFull)
        {
            Logger.Log("Player Box is full");
            return;
        }

        MiniGameUnloadBasePoint nearestPoint = _interactionObject;
        MiniGameUnloadBox pickupBox = null;
        // 3. 포인트별 처리 (스폰포인트/냉동포인트 등)
        if (nearestPoint != null)
        {
            Logger.Log(nearestPoint);
            // 인터페이스 체크 최적화
            if (nearestPoint is IBoxPickupPoint pickupPoint)
            {
                if (pickupPoint.CanPickupBox())
                {
                    pickupBox = pickupPoint.PickupBox();
                }
            }
        }

        if (pickupBox == null)
        {
            return;
        }

        pickupBox.SetIsGrab(true);

        // 상자를 스택에 추가하고 위치 설정
        _boxList.TryPush(pickupBox);

        pickupBox.transform.SetParent(Player.CharacterTransform);
        Vector3 targePos = Vector3.right + Vector3.up * (_boxHeight);
        pickupBox.transform.DOLocalJump(targePos, 1f, 1, 0.2f);
        pickupBox.transform.localRotation = Quaternion.identity;
        _boxHeight += _boxOffset;

        CoolingSkill coolingSkill = SkillList.OfType<CoolingSkill>().FirstOrDefault();
        if (coolingSkill != null && pickupBox.BoxState == Define.BoxState.Cold)
        {
            coolingSkill.OnPickUpBox(pickupBox);
        }

        // 플레이어 애니메이션 상태 설정
        _unloadPlayer.SetHoldUp(true);

    }


    private void DropBox()
    {
        if (_boxList.IsEmpty) return;
        Logger.Log("Drop Box");

        MiniGameUnloadBasePoint nearestPoint = _interactionObject;
        if (nearestPoint == null)
        {
            Debug.Log("포인트가 감지되지 않음");
            return;
        }

        MiniGameUnloadBox carriedBox = _boxList.Peek();
        if (carriedBox == null) return;

        if (!CanDropBoxAtPoint(nearestPoint, carriedBox))
        {
            Debug.Log($"이 포인트에는 {carriedBox.Info.BoxState} 상자를 놓을 수 없음");
            return;
        }

        if (!TryPlaceBoxAtPoint(nearestPoint, carriedBox))
        {
            Debug.Log("처리 실패: 포인트가 가득 찼거나 조건 불일치");
            return;
        }

        // ColdBox를 올바른 지역에 놓았을 때 CoolingSkill Regain 호출
        if (carriedBox.Info.BoxState == Define.BoxState.Normal &&
            carriedBox is ColdBox && nearestPoint is MiniGameUnloadDeliveryPoint)
        {
            CoolingSkill coolingSkill = SkillList?.OfType<CoolingSkill>().FirstOrDefault();
            if (coolingSkill != null)
            {
                coolingSkill.RegainResource(2f); // 필요에 따라 amount 조정
            }
        }
        
        // 일반 박스를 올바른 지역에 놓았을 때 SpeedUpSkill Regain 호출
        else if (carriedBox.Info.BoxState == Define.BoxState.Normal &&
                carriedBox is CommonBox && nearestPoint is MiniGameUnloadDeliveryPoint)
        {
            SpeedUpSkill speedUpSkill = SkillList?.OfType<SpeedUpSkill>().FirstOrDefault();
            if (speedUpSkill != null)
            {
                speedUpSkill.RegainResource(2f); // 필요에 따라 amount 조정
            }
        }

        HandleFragileBox(carriedBox);

        RemoveBoxFromPlayer();
    }

    private bool CanDropBoxAtPoint(MiniGameUnloadBasePoint point, MiniGameUnloadBox box)
    {
        return point.CanProcess(box.Info.BoxState);
    }

    private bool TryPlaceBoxAtPoint(MiniGameUnloadBasePoint point, MiniGameUnloadBox box)
    {
        if (point is IBoxPlacePoint boxPlaceable)
        {
            if (boxPlaceable.CanPlaceBox(box))
            {
                boxPlaceable.PlaceBox(box);
                return true;
            }
        }
        return false;
    }

    private void HandleFragileBox(MiniGameUnloadBox box)
    {
        FragileBox fragileBox = box.GetComponent<FragileBox>();
        if (fragileBox != null)
        {
            fragileBox.CheckBrokenBox(_boxList.CurrentUnloadBoxIndex);
        }
    }

    private void RemoveBoxFromPlayer()
    {
        _boxList.TryPop();
        _boxHeight -= _boxOffset;
        OnBoxListChanged?.Invoke(_boxList.BoxList);

        if (_boxList.IsEmpty)
        {        
            // 플레이어 애니메이션 상태 설정
            _unloadPlayer.SetHoldUp(false);
        }
    }


    private void FindNearestValidPoint()
    {
        MiniGameUnloadBasePoint nearest = null;
        float minDist = float.MaxValue;
        foreach (var point in _cachedPoints)
        {
            float dist = Vector3.Distance(Player.transform.position, point.transform.position);
            if (dist < minDist && dist <= _interactionRange)
            {
                minDist = dist;
                nearest = point;
            }
        }
        _interactionObject = nearest;
        //return nearest;
    }
}





