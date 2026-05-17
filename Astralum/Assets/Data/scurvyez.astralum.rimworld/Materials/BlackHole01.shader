Shader "Astralum/BlackHole01"
{
    Properties
    {
        _EffectActive ("Effect Active", Range(0,1)) = 0
        _CanvasScale ("Canvas Scale", Range(1,10)) = 2
        _ScreenEdgeFadeStart ("Screen Edge Fade Start", Range(0,0.5)) = 0.035
        _ScreenEdgeFadeEnd ("Screen Edge Fade End", Range(0,0.5)) = 0.16
        
        _Radius ("Event Horizon Radius", Range(0,1)) = 0.18
        _DistortionRadius ("Distortion Radius", Range(0,5)) = 1
        _DistortionStrength ("Distortion Strength", Range(0,0.2)) = 0.04
        _Darkness ("Center Darkness", Range(0,1)) = 1
        
        _HorizonFeather ("Event Horizon Feather", Range(0.001,0.2)) = 0.025
        _DistortionFeather ("Distortion Edge Feather", Range(0.001,0.5)) = 0.12
        _RingFeather ("Einstein Ring Feather", Range(0.001,0.3)) = 0.08
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Background+6"
            "RenderType" = "Transparent"
        }
        
        GrabPass { }
        
        Pass
        {
            Blend One Zero
            ZWrite Off
            ZTest LEqual
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _GrabTexture;
            
            float _EffectActive;
            float _CanvasScale;
            float _ScreenEdgeFadeStart;
            float _ScreenEdgeFadeEnd;
            
            float _Radius;
            float _DistortionRadius;
            float _DistortionStrength;
            float _Darkness;
            
            float _HorizonFeather;
            float _DistortionFeather;
            float _RingFeather;
            
            struct vertInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            
            struct vertOutput
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 grabPos : TEXCOORD1;
            };
            
            vertOutput vert (vertInput input)
            {
                vertOutput output;
                
                float4 worldPos = mul(unity_ObjectToWorld, input.vertex);
                worldPos.xyz += _WorldSpaceCameraPos;
                
                output.vertex = mul(UNITY_MATRIX_VP, worldPos);
                output.uv = input.uv;
                output.grabPos = ComputeGrabScreenPos(output.vertex);
                output.color = input.color;
                
                return output;
            }
            
            fixed4 frag (vertOutput input) : SV_Target
            {
                float active = saturate(_EffectActive);
                
                float2 screenUV = input.grabPos.xy / input.grabPos.w;
                
                float edgeDistance = min(
                    min(screenUV.x, 1.0 - screenUV.x),
                    min(screenUV.y, 1.0 - screenUV.y));
                
                float screenEdgeFade = smoothstep(_ScreenEdgeFadeStart, _ScreenEdgeFadeEnd, edgeDistance);
                
                float2 p = (input.uv - 0.5) * _CanvasScale;
                float2 localOffset = p;
                
                float dist = length(localOffset);
                
                float distortionMask = 1.0 - smoothstep(
                    _DistortionRadius,
                    _DistortionRadius + _DistortionFeather,
                    dist
                );
                
                distortionMask *= active;
                
                float2 radialDir = localOffset / max(dist, 0.0001);
                
                // distance from event horizon
                float safeDist = max(dist - _Radius, 0.0001);
                
                // strongest bending near the horizon, fading outward
                float bend = _DistortionStrength / safeDist;
                
                // keep it controlled
                bend = min(bend, _DistortionStrength * 8.0);
                
                // fade out at outer distortion boundary.
                float outerFade = 1.0 - smoothstep(_Radius, _DistortionRadius, dist);
                bend *= outerFade * distortionMask * active;
                
                // horizon fade prevents unstable smear inside the black center.
                float horizonFade = smoothstep(_Radius, _Radius + _HorizonFeather, dist);
                bend *= horizonFade;
                bend *= screenEdgeFade;
                
                // Einstein-ring
                float ringCenter = _Radius + _RingFeather * 0.65;
                float ringMask = 1.0 - smoothstep(_RingFeather * 0.75, _RingFeather * 1.75, abs(dist - ringCenter));
                ringMask *= active;
                ringMask *= screenEdgeFade;
                
                // near the ring, sample from opposite side of the ring...
                // inverted sky effect on either side
                float inversionStrength = ringMask * 0.65;
                
                float2 normalLensOffset = localOffset - radialDir * bend;
                float2 invertedLensOffset = -localOffset * 0.85;
                
                // main gravitational lensing:
                // sample farther/closer along the radial line.
                // this makes background appear bent around the black hole.
                float2 distortedLocalOffset = lerp(normalLensOffset, invertedLensOffset, inversionStrength);
                
                float2 distortedUV = 0.5 + distortedLocalOffset / _CanvasScale;
                float2 uvDelta = distortedUV - input.uv;
                
                uvDelta *= screenEdgeFade;
                
                float4 grabUV = input.grabPos;
                grabUV.xy += uvDelta * grabUV.w;
                
                fixed4 color = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(grabUV));
                
                // DEBUGGING: shows the distortion mask
                //float2 debugUV = distortedUV;
                //fixed4 color = fixed4(frac(debugUV.x), frac(debugUV.y), 0, 1 );
                
                float holeMask = 1.0 - smoothstep(_Radius, _Radius + _HorizonFeather, dist);
                holeMask *= active;
                
                color.rgb = lerp(color.rgb, 0.0, holeMask * _Darkness);
                
                float alphaMask = 1.0 - smoothstep(_DistortionRadius, _DistortionRadius + _DistortionFeather, dist);
                
                color.a = alphaMask * active * input.color.a;
                
                return color;
            }
            ENDHLSL
        }
    }
}