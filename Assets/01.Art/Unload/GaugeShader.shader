    Shader "Unlit/TexturedGaugeShader"
    {
        Properties
        {
            _FilledTex("Filled Texture", 2D) = "white" {}      // 채워진 칸에 사용할 텍스처
            _EmptyTex("Empty Texture", 2D) = "white" {}       // 비워진 칸에 사용할 텍스처
            _Color("Tint Color", Color) = (1,1,1,1)           // 텍스처에 적용할 틴트 색상 (옵션)
            _FillAmount("Fill Amount", Range(0.0, 1.0)) = 0.5
            _SegmentCount("Segment Count", Float) = 10
            _GapWidth("Gap Width", Range(0.0, 0.5)) = 0.05
        }
        SubShader
        {
            Tags { "RenderType"="Transparent" "Queue"="Transparent" }
            LOD 100
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _FilledTex;
                sampler2D _EmptyTex;
                fixed4 _Color;
                float _FillAmount;
                float _SegmentCount;
                float _GapWidth;

                v2f vert (appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    float segmentIndex = floor(i.uv.x * _SegmentCount);
                    float filledSegments = floor(_FillAmount * _SegmentCount);
                    if (_FillAmount >= 1.0) {
                        filledSegments = _SegmentCount;
                    }

                    // 각 칸 안에서 텍스처가 반복되도록 로컬 UV 좌표를 계산합니다.
                    float localUvX = frac(i.uv.x * _SegmentCount);
                    float2 localUV = float2(localUvX, i.uv.y);

                    if (localUvX < _GapWidth && segmentIndex > 0)
                    {
                        return fixed4(0,0,0,0); // 간격은 투명하게 처리
                    }

                    fixed4 finalColor;
                    // 현재 칸이 채워져야 하는지 확인
                    if (segmentIndex < filledSegments)
                    {
                        // 채워진 텍스처에서 색상을 샘플링
                        finalColor = tex2D(_FilledTex, localUV);
                    }
                    else
                    {
                        // 비워진 텍스처에서 색상을 샘플링
                        finalColor = tex2D(_EmptyTex, localUV);
                    }

                    // 틴트 색상을 곱하고 알파값을 적용하여 반환
                    finalColor.rgb *= _Color.rgb;
                    finalColor.a *= _Color.a;

                    return finalColor;
                }
                ENDCG
            }
        }
    }