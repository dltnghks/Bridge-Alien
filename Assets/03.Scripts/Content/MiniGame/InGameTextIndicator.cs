using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class InGameTextIndicator : MonoBehaviour
{
    [Header("프리팹의 리지드바디")]
    [SerializeField] Rigidbody mRigidBody;

    [Header("프리팹의 데미지 라벨")]
    [SerializeField] TextMeshPro mDamageLabel;

    [Header("프리팹의 자식 객체 (Look Camera)")]
    [SerializeField] Transform mChildTransform;

    [Header("세팅")]
    [SerializeField] private float _size = 0.2f;

    private void Update()
    {
        mChildTransform.LookAt(Camera.main.transform);
    }

    public void Init(Vector3 pos, string text)
    {
        mDamageLabel.SetText(text);
        // 위치 설정
        transform.position = pos;

        OnText();
    }

    public void OnText()
    {
        // 재사용시 기존의 상태를 초기화
        mRigidBody.angularVelocity = Vector3.zero;
        mRigidBody.velocity = Vector3.zero;
        transform.localScale = Vector3.one * _size;

        // 연출을 위한 힘 추가
        mRigidBody.AddForce(new Vector3(Random.Range(-2f, 2f), 3f, Random.Range(-1f, 1f)), ForceMode.Impulse);

        transform.DOScale(transform.localScale, 0);
        transform.DOLocalMoveY(3, 1).OnComplete(() =>
            // UI 변경
            Managers.Resource.Destroy(gameObject)
        );
    }

}