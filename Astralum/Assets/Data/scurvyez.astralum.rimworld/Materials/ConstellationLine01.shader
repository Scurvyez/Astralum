Shader "Astralum/ConstellationLine01"
{
    Properties
    {
        _Color ("Color", Color) = (0.45, 0.6, 1.0, 0.25)
        _Intensity ("Intensity", Range(0, 5)) = 1
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
                
                float3 worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
                float3 worldDir = normalize(worldPos);
                
                float3 skyPos = _WorldSpaceCameraPos + worldDir;
                
                output.pos = mul(UNITY_MATRIX_VP, float4(skyPos, 1.0));
                output.uv = input.uv;
                
                return output;
            }
            
            fixed4 frag (vertOutput input) : SV_Target
            {
                return fixed4(_Color.rgb * _Intensity, _Color.a);
            }
            ENDHLSL
        }
    }
}