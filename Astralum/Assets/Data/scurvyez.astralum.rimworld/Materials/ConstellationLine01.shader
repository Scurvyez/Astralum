Shader "Astralum/ConstellationLine01"
{
    Properties
    {
        _Color ("Color", Color) = (0.45, 0.6, 1.0, 0.25)
        _Intensity ("Intensity", Range(0, 5)) = 1
        _BlurStrength ("Blur Strength", Range(0.001, 1)) = 0.25
        _CoreStrength ("Core Strength", Range(0, 1)) = 0.15
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
            
            fixed4 _Color;
            float _Intensity;
            float _BlurStrength;
            float _CoreStrength;
            
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
            
            vertOutput vert (vertInput input)
            {
                vertOutput output;
                
                float4 worldPos = mul(unity_ObjectToWorld, input.vertex);
                worldPos.xyz += _WorldSpaceCameraPos;
                
                output.pos = mul(UNITY_MATRIX_VP, worldPos);
                output.uv = input.uv;
                
                return output;
            }
            
            fixed4 frag (vertOutput input) : SV_Target
            {
                float dist = abs(input.uv.y - 0.5) * 2.0;
                
                // soft falloff
                float alpha = 1.0 - smoothstep(
                    _CoreStrength,
                    _CoreStrength + _BlurStrength,
                    dist
                );
                
                fixed3 color = _Color.rgb * _Intensity;
                
                return fixed4(color * alpha, _Color.a * alpha);
            }
            ENDHLSL
        }
    }
}