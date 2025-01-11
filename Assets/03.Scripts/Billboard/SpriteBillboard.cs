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
}