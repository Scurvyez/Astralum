Shader "Astralum/Sun01"
{
    Properties
    {
        _Chromaticity ("Chromaticity", Color) = (1,1,1,1)
        _Corona ("Corona", Color) = (1,1,1,1)
        
        _ChromaticityIntensity ("Chromaticity Intensity", Range(0,2)) = 1
        _CoronaIntensity ("Corona Intensity", Range(0,2)) = 1
        
        _ChromaticityFalloffPower ("Chromaticity Falloff Power", Range(0,10)) = 2
        _RadiusPower ("Radius Power", Range(0,1)) = 0.5
        _CoronaPower ("Corona Power", Range(0,10)) = 5
        
        _VariabilityAmount ("Variability Amount", Range(0,0.5)) = 0
        _VariabilitySpeed ("Variability Speed", Range(0,5)) = 1
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Background+1"
            "RenderType" = "Background"
        }
        
        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            ZClip On
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            fixed4 _Chromaticity;
            fixed4 _Corona;
            
            float _ChromaticityIntensity;
            float _CoronaIntensity;
            
            float _ChromaticityFalloffPower;
            float _RadiusPower;
            float _CoronaPower;
            
            float _VariabilityAmount;
            float _VariabilitySpeed;
            
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
                
                // offset mesh via the camera position
                // should make the sun behave like a distant sky/bg object?
                worldPos.xyz += _WorldSpaceCameraPos;
                
                output.pos = mul(UNITY_MATRIX_VP, worldPos);
                output.uv = input.uv;
                
                return output;
            }
            
            fixed4 frag (vertOutput input) : SV_Target
            {
                float2 centeredUv = input.uv - float2(0.5,0.5);
                float dist = length(centeredUv);
                
                float baseRadial = saturate(1.0 - dist * 2.0);
                
                float chromaticityMask = pow(baseRadial, _ChromaticityFalloffPower);
                float coronaMask = pow(baseRadial, _CoronaPower);
                
                float variabilityPulse = 1.0 + sin(_Time.y * _VariabilitySpeed) * _VariabilityAmount;
                
                float3 chromaticity = _Chromaticity.rgb * chromaticityMask * _ChromaticityIntensity;
                float3 corona = _Corona.rgb * coronaMask * _CoronaIntensity  * variabilityPulse;
                
                float3 finalColor = chromaticity + corona;
                
                return fixed4(saturate(finalColor), 1.0);
            }
            ENDHLSL
        }
    }
}