Shader "Astralum/ShootingStar01"
{
    Properties
    {
        _Color ("Color", Color) = (0.75, 0.9, 1.0, 1)
        _Intensity ("Intensity", Range(0, 10)) = 2
        _CorePower ("Core Power", Range(0.5, 16)) = 4
        _TailPower ("Tail Power", Range(0.5, 16)) = 2
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Background+8"
            "RenderType" = "Transparent"
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
            float _TailPower;
            
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
            
            fixed4 frag(vertOutput input) : SV_Target
            {
                // uv.x: 0 = tail, 1 = head
                // uv.y: 0/1 = vertical edge, 0.5 = center
                float along = saturate(input.uv.x);
                float across = abs(input.uv.y - 0.5) * 2.0;
                
                float tail = pow(along, _TailPower);
                float core = pow(saturate(1.0 - across), _CorePower);
                
                float alpha = tail * core * _Color.a;
                float3 color = _Color.rgb * alpha * _Intensity;
                
                return fixed4(color, alpha);
            }
            ENDHLSL
        }
    }
}