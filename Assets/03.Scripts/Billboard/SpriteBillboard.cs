using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! 스프라이트 빌보드
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteBillboard : BillboardBase
{
    protected SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void ApplyBillboard()
    {
        // 카메라의 rotation을 가져와서 스프라이트에 적용
        Quaternion cameraRotation = mainCamera.transform.rotation;
        
        switch (billboardAxis)
        {
            case BillboardAxis.All:
                var rotateVector = cameraRotation.eulerAngles;
                rotateVector.x -= 15f;
                transform.rotation = Quaternion.Euler(rotateVector);
                
                //transform.rotation = cameraRotation;
                break;
                
            case BillboardAxis.VerticalOnly:
                transform.rotation = Quaternion.Euler(0f, cameraRotation.eulerAngles.y, 0f);
                break;
                
            case BillboardAxis.HorizontalOnly:
                transform.rotation = Quaternion.Euler(cameraRotation.eulerAngles.x, 0f, 0f);
                break;
        }

        if (freezeXZAxis)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x = 0f;
            eulerAngles.z = 0f;
            transform.eulerAngles = eulerAngles;
        }
    }
}