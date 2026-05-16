using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;

public enum InGameTextIndicatorStyle
{
    NormalScore,
    Penalty,
    Combo,
    Lucky
}

[RequireComponent(typeof(Rigidbody))]
public class InGameTextIndicator : MonoBehaviour
{
    [Header("프리팹의 리지드바디")]
    [SerializeField] Rigidbody mRigidBody;

    [Header("프리팹의 데미지 라벨")]
    [SerializeField] TextMeshPro mDamageLabel;

    [Header("프리팹의 자식 객체 (Look Camera)")]
    [SerializeField] Transform mChildTransform;

    [Header("텍스트 머티리얼 프리셋")]
    [SerializeField] Material _normalMaterial;
    [SerializeField] Material _penaltyMaterial;
    [SerializeField] Material _comboMaterial;
    [SerializeField] Material _luckyMaterial;

    [Header("세팅")]
    [SerializeField] private float _size = 0.2f;
    private Tween _moveTween;
    private Tween _scaleTween;
    private InGameTextIndicatorStyle _style = InGameTextIndicatorStyle.NormalScore;

    private void Update()
    {
        mChildTransform.LookAt(Camera.main.transform);
    }

    public void Init(Vector3 pos, string text)
    {
        Init(pos, text, InGameTextIndicatorStyle.NormalScore);
    }

    public void Init(Vector3 pos, string text, InGameTextIndicatorStyle style)
    {
        _style = style;
        mDamageLabel.SetText(text);
        ApplyMaterialPreset();
        transform.position = pos;

        OnText();
    }

    public void OnText()
    {
        _moveTween?.Kill();
        _scaleTween?.Kill();
        mRigidBody.angularVelocity = Vector3.zero;
        mRigidBody.linearVelocity = Vector3.zero;
        transform.localScale = Vector3.one * GetBaseScale();

        mRigidBody.AddForce(GetImpulse(), ForceMode.Impulse);

        _scaleTween = transform.DOScale(GetPeakScale(), 0.12f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                _scaleTween = transform.DOScale(GetBaseScale(), GetDuration() * 0.7f).SetEase(Ease.InQuad);
            });

        _moveTween = transform.DOLocalMoveY(GetMoveHeight(), GetDuration())
            .SetRelative(true)
            .SetEase(GetMoveEase())
            .OnComplete(() => Managers.Resource.Destroy(gameObject));
    }

    private float GetBaseScale()
    {
        switch (_style)
        {
            case InGameTextIndicatorStyle.Combo:
                return _size * 1.05f;
            case InGameTextIndicatorStyle.Lucky:
                return _size * 1.05f;
            case InGameTextIndicatorStyle.Penalty:
                return _size * 1.05f;
            default:
                return _size;
        }
    }

    private float GetPeakScale()
    {
        switch (_style)
        {
            case InGameTextIndicatorStyle.Combo:
                return GetBaseScale() * 1.12f;
            case InGameTextIndicatorStyle.Lucky:
                return GetBaseScale() * 1.16f;
            case InGameTextIndicatorStyle.Penalty:
                return GetBaseScale() * 1.15f;
            default:
                return GetBaseScale() * 1.18f;
        }
    }

    private Vector3 GetImpulse()
    {
        switch (_style)
        {
            case InGameTextIndicatorStyle.Combo:
                return new Vector3(Random.Range(-1.2f, 1.2f), 3.6f, Random.Range(-0.8f, 0.8f));
            case InGameTextIndicatorStyle.Lucky:
                return new Vector3(Random.Range(-0.8f, 0.8f), 4.4f, Random.Range(-0.6f, 0.6f));
            case InGameTextIndicatorStyle.Penalty:
                return new Vector3(Random.Range(-1.0f, 1.0f), 2.2f, Random.Range(-0.4f, 0.4f));
            default:
                return new Vector3(Random.Range(-2f, 2f), 3f, Random.Range(-1f, 1f));
        }
    }

    private float GetMoveHeight()
    {
        switch (_style)
        {
            case InGameTextIndicatorStyle.Combo:
                return 3.6f;
            case InGameTextIndicatorStyle.Lucky:
                return 4.2f;
            case InGameTextIndicatorStyle.Penalty:
                return 2.4f;
            default:
                return 3f;
        }
    }

    private float GetDuration()
    {
        switch (_style)
        {
            case InGameTextIndicatorStyle.Combo:
                return 1.15f;
            case InGameTextIndicatorStyle.Lucky:
                return 1.25f;
            case InGameTextIndicatorStyle.Penalty:
                return 0.95f;
            default:
                return 1f;
        }
    }

    private Ease GetMoveEase()
    {
        switch (_style)
        {
            case InGameTextIndicatorStyle.Lucky:
                return Ease.OutCubic;
            case InGameTextIndicatorStyle.Penalty:
                return Ease.OutQuad;
            default:
                return Ease.OutQuad;
        }
    }

    private void ApplyMaterialPreset()
    {
        if (mDamageLabel == null)
        {
            return;
        }

        Material targetMaterial = _normalMaterial;
        switch (_style)
        {
            case InGameTextIndicatorStyle.Penalty:
                targetMaterial = _penaltyMaterial != null ? _penaltyMaterial : _normalMaterial;
                break;
            case InGameTextIndicatorStyle.Combo:
                targetMaterial = _comboMaterial != null ? _comboMaterial : _normalMaterial;
                break;
            case InGameTextIndicatorStyle.Lucky:
                targetMaterial = _luckyMaterial != null ? _luckyMaterial : _normalMaterial;
                break;
        }

        if (targetMaterial == null)
        {
            return;
        }

        mDamageLabel.fontSharedMaterial = targetMaterial;
        mDamageLabel.fontMaterial = targetMaterial;
        mDamageLabel.UpdateMeshPadding();
        mDamageLabel.ForceMeshUpdate();
    }

    private void OnDestroy()
    {
        _moveTween?.Kill();
        _scaleTween?.Kill();
    }
}
