Shader "Astralum/GalacticDustLane01"
{
    Properties
    {
        _ColorA ("Dust Color A", Color) = (0.035, 0.030, 0.045, 1)
        _ColorB ("Dust Color B", Color) = (0.10, 0.075, 0.055, 1)
        
        _Alpha ("Alpha", Range(0, 1)) = 0.16
        _Intensity ("Intensity", Range(0, 2)) = 0.65
        
        _CanvasScale ("Canvas Scale", Range(1, 10)) = 1
        
        _NoiseScale ("Noise Scale", Range(0.1, 20)) = 4
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.75
        
        _DetailScale ("Detail Scale", Range(0.1, 60)) = 18
        _DetailStrength ("Detail Strength", Range(0, 1)) = 0.35
        
        _CloudThreshold ("Cloud Threshold", Range(0, 1)) = 0.42
        _EdgeSoftness ("Edge Softness", Range(0.001, 1)) = 0.32
        _EdgeFadeStart ("Quad Edge Fade Start", Range(0, 0.5)) = 0.08
        _EdgeFadeEnd ("Quad Edge Fade End", Range(0, 0.5)) = 0.22
        
        _StretchX ("Stretch X", Range(0.1, 5)) = 1.8
        _StretchY ("Stretch Y", Range(0.1, 5)) = 0.55
        
        _Rotation ("Rotation", Range(0, 6.28318)) = 0
        _SeedOffset ("Seed Offset", Vector) = (0, 0, 0, 0)
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Background+2"
            "RenderType" = "Transparent"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual
        Cull Off
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            fixed4 _ColorA;
            fixed4 _ColorB;
            
            float _Alpha;
            float _Intensity;
            float _CanvasScale;
            
            float _NoiseScale;
            float _NoiseStrength;
            float _DetailScale;
            float _DetailStrength;
            
            float _CloudThreshold;
            float _EdgeSoftness;
            float _EdgeFadeStart;
            float _EdgeFadeEnd;
            
            float _StretchX;
            float _StretchY;
            float _Rotation;
            float4 _SeedOffset;
            
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
            
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }
            
            float noise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                
                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));
                
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(
                    lerp(a, b, u.x),
                    lerp(c, d, u.x),
                    u.y
                );
            }
            
            float2 rotate2D(float2 p, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);
                
                return float2(
                    p.x * c - p.y * s,
                    p.x * s + p.y * c
                );
            }
            
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
                float2 uv = input.uv;
                
                float edgeDistance = min(
                    min(uv.x, 1.0 - uv.x),
                    min(uv.y, 1.0 - uv.y)
                );
                
                float quadEdgeFade = smoothstep(
                    _EdgeFadeStart,
                    _EdgeFadeEnd,
                    edgeDistance
                );
                
                float2 p = (uv - 0.5) * _CanvasScale;
                p = rotate2D(p, _Rotation);
                
                p.x /= max(_StretchX, 0.001);
                p.y /= max(_StretchY, 0.001);
                
                float radial = length(p);
                float edgeMask = 1.0 - smoothstep(0.35, 0.75, radial );
                
                float2 noiseUv = p + _SeedOffset.xy;
                
                float baseNoise = noise(noiseUv * _NoiseScale);
                float detailNoise = noise(noiseUv * _DetailScale + _SeedOffset.zw);
                float cloud = lerp(baseNoise, baseNoise * detailNoise, _DetailStrength);
                
                cloud = lerp(1.0, cloud, _NoiseStrength);
                
                float cloudMask = smoothstep(
                    _CloudThreshold,
                    _CloudThreshold + _EdgeSoftness,
                    cloud
                );
                
                float alpha = cloudMask * edgeMask * quadEdgeFade * _Alpha;
                
                float3 color = lerp(_ColorA.rgb, _ColorB.rgb, baseNoise);
                color *= _Intensity;
                
                return fixed4(color, alpha);
            }
            ENDHLSL
        }
    }
}
