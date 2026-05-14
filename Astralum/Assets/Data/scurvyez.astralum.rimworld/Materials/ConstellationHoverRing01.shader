Shader "Astralum/ConstellationHoverRing01"
{
    Properties
    {
        _Color ("Color", Color) = (0.65, 0.85, 1.0, 1)
        _Intensity ("Intensity", Range(0, 5)) = 1.25
        _RingRadius ("Ring Radius", Range(0, 1)) = 0.45
        _RingThickness ("Ring Thickness", Range(0.001, 0.25)) = 0.055
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 1.5
        _PulseStrength ("Pulse Strength", Range(0, 1)) = 0.35
        _AlphaPulseMin ("Alpha Pulse Min", Range(0, 1)) = 0.65
        _PulseTime ("Pulse Time", Float) = 0
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Background+5"
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
            float _RingRadius;
            float _RingThickness;
            float _PulseSpeed;
            float _PulseStrength;
            float _AlphaPulseMin;
            float _PulseTime;

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

            fixed4 frag(vertOutput input) : SV_Target
            {
                float2 centeredUv = input.uv - 0.5;
                float dist = length(centeredUv);
                
                // start bright = cos()
                float pulse = cos(_PulseTime * _PulseSpeed) * 0.5 + 0.5;
                float ringRadius = _RingRadius + pulse * _PulseStrength * 0.08;
                float alphaPulse = lerp(_AlphaPulseMin, 1.0, pulse);
                
                float delta = abs(dist - ringRadius);
                
                float ring = 1.0 - smoothstep(
                    _RingThickness,
                    _RingThickness * 2.0,
                    delta
                );
                
                float alpha  = ring * _Color.a * alphaPulse;
                float3 color = _Color.rgb * alpha * _Intensity;
                
                return fixed4(color, alpha);
            }
            ENDHLSL
        }
    }
}