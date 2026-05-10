Shader "Astralum/Sun01"
{
    Properties
    {
        _Chromaticity ("Chromaticity", Color) = (1,1,1,1)
        _Corona ("Corona", Color) = (1,1,1,1)
        
        _SurfaceNoiseStrength ("Surface Noise Strength", Range(0,5)) = 0.025
        _CoronaRotationSpeed ("Corona Rotation Speed", Range(-5,5)) = 0
        
        _ChromaticityIntensity ("Chromaticity Intensity", Range(0,2)) = 1
        _CoronaIntensity ("Corona Intensity", Range(0,2)) = 1
        _OuterCoronaIntensity ("Outer Corona Intensity", Range(0,2)) = 0.25
        
        _ChromaticityFalloffPower ("Chromaticity Falloff Power", Range(0,10)) = 2
        _CoronaPower ("Corona Power", Range(0,10)) = 5
        _OuterCoronaPower ("Outer Corona Power", Range(0,10)) = 6
        
        _VariabilityAmount ("Variability Amount", Range(0,0.5)) = 0
        _VariabilitySpeed ("Variability Speed", Range(0,5)) = 1
        
        _ScalingFactor ("Scaling Factor", Range(0,2)) = 0.65
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Background+1"
            "RenderType" = "Background"
        }
        
        Cull Off
        ZWrite Off
        ZClip On
        
        // ------------------------------------------------------------
        // PASS 1: STAR CORE / CHROMATICITY
        // Normal alpha blending so the core does not additively inflate
        // against bright sky/background colors.
        // ------------------------------------------------------------
        Pass
        {
            Name "Core"
            Blend SrcAlpha OneMinusSrcAlpha
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragCore
            #include "UnityCG.cginc"
            
            fixed4 _Chromaticity;
            float _SurfaceNoiseStrength;
            float _ChromaticityIntensity;
            float _ChromaticityFalloffPower;
            float _ScalingFactor;
            
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
            
            fixed4 fragCore (vertOutput input) : SV_Target
            {
                float2 centeredUv = input.uv - float2(0.5, 0.5);
                centeredUv /= max(_ScalingFactor, 0.0001);

                float dist = length(centeredUv);
                float baseRadial = saturate(1.0 - dist * 2.0);

                float chromaticityMask = pow(baseRadial, _ChromaticityFalloffPower);

                float3 chromaticity = _Chromaticity.rgb * chromaticityMask * _ChromaticityIntensity;

                float noise = frac(sin(dot(input.uv * 128.0, float2(12.9898, 78.233))) * 43758.5453);
                noise = lerp(1.0 - _SurfaceNoiseStrength, 1.0 + _SurfaceNoiseStrength, noise);

                chromaticity *= noise;

                float alpha = saturate(chromaticityMask * _Chromaticity.a);

                return fixed4(saturate(chromaticity), alpha);
            }
            ENDHLSL
        }
        
        // ------------------------------------------------------------
        // PASS 2: CORONA / OUTER GLOW
        // Screen-ish blending. Less explosive than pure additive on bright
        // backgrounds, while still feeling like glow.
        // ------------------------------------------------------------
        Pass
        {
            Name "Corona"
            Blend One OneMinusSrcColor

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragCorona
            #include "UnityCG.cginc"
            
            fixed4 _Corona;
            
            float _CoronaRotationSpeed;
            float _CoronaIntensity;
            float _OuterCoronaIntensity;
            float _CoronaPower;
            float _OuterCoronaPower;
            float _VariabilityAmount;
            float _VariabilitySpeed;
            float _ScalingFactor;
            
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

            float Hash21(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }
            
            float ValueNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                
                float a = Hash21(i);
                float b = Hash21(i + float2(1.0, 0.0));
                float c = Hash21(i + float2(0.0, 1.0));
                float d = Hash21(i + float2(1.0, 1.0));
                
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }
            
            fixed4 fragCorona (vertOutput input) : SV_Target
            {
                float2 centeredUv = input.uv - float2(0.5, 0.5);
                centeredUv /= max(_ScalingFactor, 0.0001);

                float dist = length(centeredUv);
                float baseRadial = saturate(1.0 - dist * 2.0);

                float coronaMask = pow(baseRadial, _CoronaPower);

                float outerHalo = saturate(1.0 - dist * 1.15);
                outerHalo = pow(outerHalo, _OuterCoronaPower);

                float variabilityPulse = 1.0 + sin(_Time.y * _VariabilitySpeed) * _VariabilityAmount;

                float sphereMask = saturate(1.0 - dist * 2.0);

                float projectedDepth = sqrt(saturate(1.0 - dot(centeredUv * 2.0, centeredUv * 2.0)));

                float visualDayLengthSeconds = 240.0;
                float visualRotationScale = 0.1;

                float rotationsPerSecond = _CoronaRotationSpeed / visualDayLengthSeconds;
                float flow = _Time.y * rotationsPerSecond * visualRotationScale;

                float surfaceX = centeredUv.x;
                float surfaceY = centeredUv.y / max(projectedDepth, 0.15);

                float2 coronaFlowUv = float2(
                    surfaceX * 2.0,
                    surfaceY + flow
                );

                float coronaNoiseA = ValueNoise(coronaFlowUv * 5.0);
                float coronaNoiseB = ValueNoise(coronaFlowUv * 11.0 + 17.3);

                float coronaVariation = lerp(coronaNoiseA, coronaNoiseB, 0.35);
                coronaVariation = lerp(0.85, 1.15, coronaVariation);
                coronaVariation = lerp(1.0, coronaVariation, sphereMask);

                float3 corona = _Corona.rgb
                    * coronaMask
                    * _CoronaIntensity
                    * variabilityPulse
                    * coronaVariation;

                float3 outerCorona = _Corona.rgb
                    * outerHalo
                    * _OuterCoronaIntensity
                    * variabilityPulse
                    * coronaVariation;

                float3 finalColor = corona + outerCorona;

                // Even though Blend One OneMinusSrcColor does not use alpha
                // the same way normal alpha blending does, this is still useful
                // if you switch this pass to Blend SrcAlpha One for testing.
                float alpha = saturate(
                    coronaMask * _CoronaIntensity +
                    outerHalo * _OuterCoronaIntensity
                ) * _Corona.a;

                return fixed4(saturate(finalColor), alpha);
            }
            ENDHLSL
        }
    }
}