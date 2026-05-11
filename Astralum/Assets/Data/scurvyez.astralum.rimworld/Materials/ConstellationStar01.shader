Shader "Astralum/ConstellationStar01"
{
    Properties
    {
        _Color ("Color", Color) = (0.75, 0.85, 1.0, 1)
        _Intensity ("Intensity", Range(0, 5)) = 1
        _CorePower ("Core Power", Range(0.5, 20)) = 8
        _GlowPower ("Glow Power", Range(0.5, 20)) = 2
        
        _SkyProjectionDistance ("Sky Projection Distance", Range(1,10000)) = 5000
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
            float _CorePower;
            float _GlowPower;
            
            float _SkyProjectionDistance;
            
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
                
                float3 skyPos = _WorldSpaceCameraPos + worldDir * _SkyProjectionDistance;
                
                output.pos = mul(UNITY_MATRIX_VP, float4(skyPos, 1.0));
                output.uv = input.uv;
                
                return output;
                
            }
            
            fixed4 frag (vertOutput input) : SV_Target
            {
                float2 centeredUv = input.uv - float2(0.5, 0.5);
                float dist = length(centeredUv) * 2.0;
                
                float radial = saturate(1.0 - dist);
                
                float core = pow(radial, _CorePower);
                float glow = pow(radial, _GlowPower) * 0.35;
                
                float brightness = saturate(core + glow);
                
                float3 color = _Color.rgb * brightness * _Intensity;
                float alpha = brightness * _Color.a;
                
                return fixed4(color, alpha);
            }
            ENDHLSL
        }
    }
}