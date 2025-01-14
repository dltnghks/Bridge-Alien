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

    private void Update()
    {
        mChildTransform.LookAt(Camera.main.transform);
    }

    public void Init(Vector3 pos, float amount, Color color, float size)
    {
        /*
        // 만약 호출된 값이 0이라면 리턴
        if(Mathf.RoundToInt(amount) == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        // 재사용시 기존의 상태를 초기화
        mRigidBody.angularVelocity = Vector3.zero;
        mRigidBody.velocity = Vector3.zero;
        transform.localScale = Vector3.one * size;

        // 위치 설정
        transform.position = pos;
    
        
        // 텍스트 설정
        mDamageLabel.text = Mathf.RoundToInt(amount).ToString();
        mDamageLabel.color = color;

        // 연출을 위한 힘 추가
        mRigidBody.AddForce(new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)), ForceMode.Impulse);

        transform.DOScale(transform.localScale, 0);
        transform.DOLocalMoveY(3, 1).OnComplete(() =>
            // UI 변경
            Managers.Resource.Destroy(gameObject)
        );*/

    }
}