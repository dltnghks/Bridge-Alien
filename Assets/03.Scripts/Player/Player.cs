using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private InputManager inputManager;                 // Input Manager 스크립트 (../Managers/InputManager.cs)

    [Header("Movement")]
    [SerializeField] private float moveSpeed;                           // 이동 속도
    private Vector2 movement;                                           // 이동 방향

    [Header("Components")]
    [SerializeField] private Animator animator;                         // 애니메이션 컴포넌트
    [SerializeField] private Rigidbody2D rb;                            // 리지드 바디 컴포넌트

    //~ Start시 컴포넌트 초기화
    private void Start()
    {
        if (animator == null)   animator = GetComponent<Animator>();
        if (rb == null)         rb = GetComponent<Rigidbody2D>();
        if (inputManager == null) inputManager = FindObjectOfType<InputManager>();
    }

    //~ Update시 매 프레임 마다 입력 매니저에서 이동 입력 받아와 이동 처리
    void Update()
    {
        movement = inputManager.GetMovementInput();
        Move();
    }

    //~ 플레이어 이동 함수
    void Move()
    {
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}
