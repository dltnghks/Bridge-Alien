using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteBillboard billboard;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 필요한 컴포넌트들 가져오기/추가하기
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // SpriteBillboard 컴포넌트 추가
        billboard = gameObject.AddComponent<SpriteBillboard>();
        
        // 선택사항: 빌보드 설정 변경
        // billboard.billboardAxis = BillboardBase.BillboardAxis.VerticalOnly; // Y축 회전만 원할 경우
    }

    void Update()
    {
        // 여기서는 플레이어의 일반적인 업데이트 로직을 처리
        // 예: 이동, 액션 등
        PlayerMovement();
    }

    void PlayerMovement()
    {
        // 예시: 간단한 이동 로직
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical);
        transform.position += movement * Time.deltaTime * 5f;
    }
}