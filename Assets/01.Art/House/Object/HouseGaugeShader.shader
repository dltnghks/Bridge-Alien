    Shader "Unlit/ContinuousGaugeShader"
    {
        Properties
        {
            _FilledTex("Filled Texture", 2D) = "white" {}      // 채워진 칸에 사용할 텍스처
            _EmptyTex("Empty Texture", 2D) = "blank" {}       // 비워진 칸에 사용할 텍스처
            _Color("Tint Color", Color) = (1,1,1,1)           // 텍스처에 적용할 틴트 색상 (옵션)
            _FillAmount("Fill Amount", Range(0.0, 1.0)) = 0.5
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

                v2f vert (appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {

                    fixed4 finalColor;

                    // UV의 x좌표를 기준으로 _FillAmount와 비교
                    if(i.uv.x < _FillAmount){
                        // 채워진 부분 : _FilledText 샘플링
                        finalColor = tex2D(_FilledTex, i.uv);
                    }
                    else{
                        // 비워진 텍스처에서 색상을 샘플링
                        finalColor = tex2D(_EmptyTex, i.uv);
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