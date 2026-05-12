Shader "Astralum/Nebulae01"
{
    Properties
    {
        _ColorA ("Color A / Outer Dust", Color) = (0.12, 0.18, 0.35, 1)
        _ColorB ("Color B / Main Cloud", Color) = (0.25, 0.45, 1.0, 1)
        _ColorC ("Color C / Hot Knots", Color) = (0.85, 0.25, 0.95, 1)
        _ColorD ("Color D / Bright Core", Color) = (1.0, 0.75, 0.45, 1)
        
        _ColorStopB ("Color Stop B", Range(0.01, 0.99)) = 0.33
        _ColorStopC ("Color Stop C", Range(0.01, 0.99)) = 0.66
        _ColorBandSharpness ("Color Band Sharpness", Range(0.25, 8)) = 1
        
        _SeedOffset ("Seed Offset", Vector) = (0,0,0,0)
        _Seed ("Seed", Float) = 0
        
        _Intensity ("Intensity", Range(0, 5)) = 1
        _Alpha ("Alpha", Range(0, 1)) = 0.35
        
        _NoiseScale ("Noise Scale", Range(0.5, 20)) = 5
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 1
        _CloudThreshold ("Cloud Threshold", Range(0, 1)) = 0.35
        _EdgeSoftness ("Edge Softness", Range(0.01, 1)) = 0.35
        
        _WarpScale ("Warp Scale", Range(0.1, 20)) = 3
        _WarpStrength ("Warp Strength", Range(0, 10)) = 0.35
        _ShapePower ("Shape Power", Range(0.25, 6)) = 1.5
        _CoreOffset ("Core Offset", Vector) = (0, 0, 0, 0)
        
        _StretchX ("Stretch X", Range(0.25, 4)) = 1.8
        _StretchY ("Stretch Y", Range(0.25, 4)) = 0.85
        _Rotation ("Rotation", Range(0, 6.28318)) = 0
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Background+2"
            "RenderType" = "Background"
        }
        
        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            ZTest LEqual
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            fixed4 _ColorA;
            fixed4 _ColorB;
            fixed4 _ColorC;
            fixed4 _ColorD;
            
            float _ColorStopB;
            float _ColorStopC;
            float _ColorBandSharpness;
            
            float4 _SeedOffset;
            float _Seed;
            
            float _Intensity;
            float _Alpha;
            
            float _NoiseScale;
            float _NoiseStrength;
            float _CloudThreshold;
            float _EdgeSoftness;
            
            float _WarpScale;
            float _WarpStrength;
            float _ShapePower;
            float4 _CoreOffset;
            
            float _StretchX;
            float _StretchY;
            float _Rotation;
            
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
            
            float Fbm(float2 uv)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;
                
                for (int i = 0; i < 5; i++)
                {
                    value += ValueNoise(uv * frequency) * amplitude;
                    frequency *= 2.0;
                    amplitude *= 0.5;
                }
                
                return value;
            }
            
            float2 Rotate2D(float2 p, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);
                
                return float2(
                    p.x * c - p.y * s,
                    p.x * s + p.y * c
                );
            }
            
            float2 WarpUv(float2 uv)
            {
                float warpA = Fbm(
                    uv * _WarpScale
                    + _SeedOffset.xy + _Seed * 0.411
                );
                
                float warpB = Fbm(
                    uv * _WarpScale
                    + _SeedOffset.zw - _Seed * 0.719
                );
                
                float2 warp = float2(warpA, warpB) * 2.0 - 1.0;

                return uv + warp * _WarpStrength;
            }
            
            float3 NebulaGradient(float t)
            {
                t = saturate(t);
                
                float stopB = min(_ColorStopB, _ColorStopC - 0.01);
                float stopC = max(_ColorStopC, stopB + 0.01);
                
                if (t < stopB)
                {
                    float u = saturate(t / stopB);
                    u = pow(u, _ColorBandSharpness);
                    return lerp(_ColorA.rgb, _ColorB.rgb, u);
                }
                
                if (t < stopC)
                {
                    float u = saturate((t - stopB) / (stopC - stopB));
                    u = pow(u, _ColorBandSharpness);
                    return lerp(_ColorB.rgb, _ColorC.rgb, u);
                }
                
                float u = saturate((t - stopC) / (1.0 - stopC));
                u = pow(u, _ColorBandSharpness);
                return lerp(_ColorC.rgb, _ColorD.rgb, u);
            }
            
            fixed4 frag (vertOutput input) : SV_Target
            {
                float2 uv = input.uv - float2(0.5, 0.5);
                
                uv = Rotate2D(uv, _Rotation);
                uv.x *= _StretchX;
                uv.y *= _StretchY;

                float2 shapeUv = uv - _CoreOffset.xy;
                
                float radial = length(shapeUv) * 2.0;
                float softShape = 1.0 - smoothstep(1.0 - _EdgeSoftness, 1.0, radial);
                softShape = pow(saturate(softShape), _ShapePower);

                float2 warpedUv = WarpUv(uv);
                
                float noise = Fbm(
                    warpedUv * _NoiseScale
                    + _SeedOffset.xy + _Seed * 0.137);
                
                float detail = Fbm(
                    warpedUv * _NoiseScale * 2.35
                    + _SeedOffset.zw - _Seed * 0.271);
                
                float fineDetail = Fbm(warpedUv * _NoiseScale * 5.0 + 8.91);
                
                float cloud = lerp(noise, detail, 0.35);
                cloud = lerp(cloud, fineDetail, 0.15);

                cloud = saturate((cloud - _CloudThreshold) / max(0.0001, 1.0 - _CloudThreshold));
                cloud = pow(cloud, 1.35);
                
                float mask = cloud * softShape * _NoiseStrength;

                float colorT = saturate(detail * 0.65 + cloud * 0.35);
                colorT = pow(colorT, 0.5);
                float3 color = NebulaGradient(colorT);
                
                float alpha = saturate(mask * _Alpha);
                float3 finalColor = color * mask * _Intensity;
                
                return fixed4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
}