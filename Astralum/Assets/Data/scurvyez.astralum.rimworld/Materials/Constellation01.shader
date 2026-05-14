Shader "Unlit/Constellation01"
{
    Properties
    {
        _MainTex ("Constellation Line Texture", 2D) = "white" {}
        _Color ("Line Color", Color) = (0.45, 0.6, 1.0, 0.45)
        _Intensity ("Line Intensity", Range(0, 5)) = 1
        _BlurStrength ("Blur Strength", Range(0, 2)) = 0.45
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Background+1"
            "RenderType" = "Background"
        }

        Blend SrcAlpha One
        ZWrite Off
        ZTest LEqual
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            fixed4 _Color;
            float _Intensity;
            float _BlurStrength;

            struct vertInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct vertOutput
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            vertOutput vert(vertInput input)
            {
                vertOutput output;

                float4 worldPos = mul(unity_ObjectToWorld, input.vertex);
                worldPos.xyz += _WorldSpaceCameraPos;

                output.pos = mul(UNITY_MATRIX_VP, worldPos);
                output.uv = input.uv;

                return output;
            }

            float SampleBlurredAlpha(float2 uv)
            {
                float2 texel = _MainTex_TexelSize.xy * _BlurStrength;

                float a = tex2D(_MainTex, uv).a * 0.40;
                a += tex2D(_MainTex, uv + float2( texel.x, 0)).a * 0.15;
                a += tex2D(_MainTex, uv + float2(-texel.x, 0)).a * 0.15;
                a += tex2D(_MainTex, uv + float2(0,  texel.y)).a * 0.15;
                a += tex2D(_MainTex, uv + float2(0, -texel.y)).a * 0.15;

                return saturate(a);
            }

            fixed4 frag(vertOutput input) : SV_Target
            {
                float lineAlpha = SampleBlurredAlpha(input.uv);

                float3 color = _Color.rgb * lineAlpha * _Intensity;
                float alpha = lineAlpha * _Color.a;

                return fixed4(color, alpha);
            }
            ENDHLSL
        }
    }
}