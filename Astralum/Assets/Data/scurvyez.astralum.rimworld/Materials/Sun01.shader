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
            
            float _SurfaceNoiseStrength;
            float _CoronaRotationSpeed;
            
            float _ChromaticityIntensity;
            float _CoronaIntensity;
            float _OuterCoronaIntensity;
            
            float _ChromaticityFalloffPower;
            float _CoronaPower;
            float _OuterCoronaPower;
            
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
            
            fixed4 frag (vertOutput input) : SV_Target
            {
                float2 centeredUv = input.uv - float2(0.5,0.5);
                float dist = length(centeredUv);
                
                float baseRadial = saturate(1.0 - dist * 2.0);
                
                float chromaticityMask = pow(baseRadial, _ChromaticityFalloffPower);
                float coronaMask = pow(baseRadial, _CoronaPower);
                
                float outerHalo = saturate(1.0 - dist * 1.15);
                outerHalo = pow(outerHalo, _OuterCoronaPower);
                
                float variabilityPulse = 1.0 + sin(_Time.y * _VariabilitySpeed) * _VariabilityAmount;
                
                float3 chromaticity = _Chromaticity.rgb * chromaticityMask * _ChromaticityIntensity;
                
                float noise = frac(sin(dot(input.uv * 128.0, float2(12.9898,78.233))) * 43758.5453);
                noise = lerp(1.0 - _SurfaceNoiseStrength, 1.0 + _SurfaceNoiseStrength, noise);
                
                chromaticity *= noise;
                
                float sphereMask = saturate(1.0 - dist * 2.0);
                
                // 0 at visible edge, 1 at center.
                // should makes motion strongest in the middle and weaker at the edges...
                // edit: it does :poggers:
                float projectedDepth = sqrt(saturate(1.0 - dot(centeredUv * 2.0, centeredUv * 2.0)));
                
                // horizontal left/right flow, like...
                // if you're looking head-on at a spinning basketball
                float rotationSpeedScale = 0.15;
                float flow = _Time.y * _CoronaRotationSpeed * rotationSpeedScale;
                
                // compress motion near the edges so it feels like it's wrapped around a sphere
                float surfaceX = centeredUv.x;
                float surfaceY = centeredUv.y / max(projectedDepth, 0.15);
                
                // move the "surface" horizontally
                float2 coronaFlowUv = float2(
                    surfaceX * 2.0,
                    surfaceY + flow
                );
                
                // layered soft noise/banding for visible rotating features, I think I like this...
                float coronaNoiseA = ValueNoise(coronaFlowUv * 5.0);
                float coronaNoiseB = ValueNoise(coronaFlowUv * 11.0 + 17.3);
                
                float coronaVariation = lerp(coronaNoiseA, coronaNoiseB, 0.35);
                coronaVariation = lerp(0.85, 1.15, coronaVariation);
                
                // fade the rotating detail toward the outer silhouette
                coronaVariation = lerp(1.0, coronaVariation, sphereMask);
                
                float3 corona = _Corona.rgb * coronaMask * _CoronaIntensity * 
                    variabilityPulse * coronaVariation;
                float3 outerCorona = _Corona.rgb * outerHalo * _OuterCoronaIntensity * 
                    variabilityPulse * coronaVariation;
                
                float3 finalColor = chromaticity + corona + outerCorona;
                
                return fixed4(saturate(finalColor), 1.0);
            }
            ENDHLSL
        }
    }
}