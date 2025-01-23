using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    [Header("Shake Settings")]
    public float positionShakeAmount = 0.1f; // 위치 흔들림 크기
    public float rotationShakeAmount = 1f;  // 회전 흔들림 크기
    public float shakeSpeed = 2f;           // 흔들림 속도

    private Vector3 originalPosition;       // 초기 위치 저장
    private Quaternion originalRotation;    // 초기 회전 저장

    private void Start()
    {
        // 초기 위치와 회전 저장
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        ApplyShakeEffect();
    }

    private void ApplyShakeEffect()
    {
        // 흔들림을 Perlin Noise를 기반으로 계산
        float shakeOffsetX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * 2 - 1; // -1 ~ 1 범위
        float shakeOffsetY = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * 2 - 1;

        // 위치 흔들림
        Vector3 positionOffset = new Vector3(shakeOffsetX, shakeOffsetY, 0) * positionShakeAmount;
        transform.localPosition = originalPosition + positionOffset;

        // 회전 흔들림
        float rotationOffsetZ = Mathf.PerlinNoise(Time.time * shakeSpeed, Time.time * shakeSpeed) * 2 - 1;
        Vector3 rotationOffset = new Vector3(0, 0, rotationOffsetZ) * rotationShakeAmount;
        transform.localRotation = Quaternion.Euler(originalRotation.eulerAngles + rotationOffset);
    }

}
