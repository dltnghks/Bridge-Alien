using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxWarpSkill : ChargeSkill
{
    private MiniGameUnloadBoxList _playerBoxList; // 플레이어 상자 더미 참조
    private MiniGameUnloadDeliveryPoint[] _deliveryPoints; // 배달 지점 목록
    
    [SerializeField]
    private GameObject warpEffectPrefab; // 워프 이펙트 프리팹

    private Action OnDropBox; // 상자 배달 완료 후 호출될 액션

    public override void Initialize(MGUSkillContext context)
    {
        _playerBoxList = context.BoxList;
        _deliveryPoints = context.DeliveryPoints.ToArray();
        OnDropBox = context.RemoveBoxFromPlayerAction;

        base.Initialize(context); // 부모 클래스의 초기화 호출
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        remainingCharges--; // 성공적으로 사용했으므로 횟수 감소
        OnCountChanged?.Invoke(remainingCharges); // 사용 횟수 감소 알림
        // 사용 횟수 감소는 성공적으로 사용했을 때만 처리
        StartCoroutine(BoxWarpProcess());
    }

    public override bool CanUseSkill()
    {
        // 횟수가 남았는지와 상자를 들고 있는지 함께 체크
        return base.CanUseSkill() && _playerBoxList != null && !_playerBoxList.IsEmpty;
    }

    private IEnumerator BoxWarpProcess()
    {
        MiniGameUnloadBox topBox = _playerBoxList.Peek();
        var effect = CreateBoxWarpEffect(topBox.transform); // 이펙트 재생

        // 이펙트 표시 1초
        yield return new WaitForSeconds(1.0f);

        // 이펙트 제거
        if (effect != null)
        {
            Destroy(effect);
        }

        // 상자 이동
        MiniGameUnloadBox box = _playerBoxList.Peek();

        foreach (var deliveryPoint in _deliveryPoints)
        {
            if (deliveryPoint.CheckBoxInfo(box.Info) && deliveryPoint.CanPlaceBox(box))
            {
                // 상자를 배달 지점으로 이동
                box.transform.position = deliveryPoint.transform.position;
                deliveryPoint.PlaceBox(box);
                OnDropBox?.Invoke(); // 상자 배달 완료 후 액션 호출
                Logger.Log($"Teleported box to {deliveryPoint.name}");
                isActive = false; // 스킬 사용 완료
                break;
            }
        }

        // 이펙트 표시
    }

    public override void TryActivate()
    {
        Logger.Log("BoxWarpSkill TryActivate");
        if (CanUseSkill())
        {
            OnActivate();
        }
        else
        {
            Logger.Log("Cannot use BoxWarpSkill: either no charges left or no box to teleport.");
        }
    }

    // 워프 이펙트 함수, 일정 시간 후 제거
    private GameObject CreateBoxWarpEffect(Transform boxTransform)
    {
        GameObject effect = Managers.Resource.Instantiate(warpEffectPrefab, boxTransform);

        effect.transform.localPosition = Vector3.zero; // 상자 위치에 맞춤
        effect.transform.localRotation = Quaternion.identity; // 회전 초기화
        // 지금 이미지 사이즈가 너무 커서 작게 조정
        effect.transform.localScale = Vector3.one * 0.2f;
        return effect;
    }
}
