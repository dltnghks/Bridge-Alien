using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Joystick joystick;                                           // 조이스틱 오브젝트

    public Vector2 GetMovementInput()                                   // 조이스틱 입력 받아오는 함수
    {
        return new Vector2(joystick.Horizontal, joystick.Vertical);     // 조이스틱 입력 받아오는 함수
    }
}