Shader "Unlit/Pulsar01"
{
    Properties
    {
        _ShellDarkColor ("Shell Dark Color", Color) = (0.015, 0.03, 0.08, 1)
        _ShellBrightColor ("Shell Bright Color", Color) = (0.18, 0.55, 1.0, 1)
        _CoreColor ("Core Color", Color) = (1, 1, 1, 1)
        _JetColor ("Jet Color", Color) = (0.55, 0.85, 1.0, 1)
        
        _CanvasScale ("Canvas Scale", Range(1, 10)) = 1
        _Intensity ("Intensity", Range(0, 10)) = 1.25
        _Alpha ("Alpha", Range(0, 1)) = 0.85
        
        _CoreOffset ("Core Offset", Vector) = (0.06, -0.015, 0, 0)
        _CoreRadius ("Core Radius", Range(0.001, 0.15)) = 0.018
        _CoreGlowRadius ("Core Glow Radius", Range(0.01, 0.5)) = 0.12
        _CoreIntensity ("Core Intensity", Range(0, 20)) = 4
        _CorePulseSpeed ("Core Pulse Speed", Range(0, 20)) = 7
        _CorePulseStrength ("Core Pulse Strength", Range(0, 1)) = 0.2
        
        _ShellRadius ("Shell Radius", Range(0.05, 0.75)) = 0.32
        _ShellThickness ("Shell Thickness", Range(0.01, 0.35)) = 0.12
        _ShellSoftness ("Shell Softness", Range(0.01, 0.35)) = 0.10
        _ShellPower ("Shell Power", Range(0.5, 8)) = 2.25
        _ShellCoverage ("Shell Coverage", Range(0.05, 1)) = 0.75
        
        _InnerCrescentRadiusOffset ("Inner Crescent Radius Offset", Range(-0.4, 0.1)) = -0.11
        _InnerCrescentThickness ("Inner Crescent Thickness", Range(0.005, 0.25)) = 0.055
        _InnerCrescentSoftness ("Inner Crescent Softness", Range(0.005, 0.25)) = 0.09
        _InnerCrescentIntensity ("Inner Crescent Intensity", Range(0, 5)) = 1.25
        
        _BandRadiusOffset ("Inner Band Radius Offset", Range(-0.3, 0.3)) = -0.06
        _BandThickness ("Inner Band Thickness", Range(0.005, 0.2)) = 0.035
        _BandIntensity ("Inner Band Intensity", Range(0, 1)) = 0.35
        _BandSoftness ("Inner Band Softness", Range(0.01, 0.25)) = 0.08
        
        _JetLength ("Jet Length", Range(0.05, 1.5)) = 0.80
        _JetWidth ("Jet Width", Range(0.001, 0.25)) = 0.035
        _JetSpread ("Jet Spread", Range(0, 0.5)) = 0.18
        _JetIntensity ("Jet Intensity", Range(0, 10)) = 2.0
        _JetFalloff ("Jet Falloff", Range(0.2, 8)) = 2.8
        _JetAngle ("Jet Angle", Range(-3.14159, 3.14159)) = 0
        _JetFlicker ("Jet Flicker", Range(0, 1)) = 0.15
        _JetFlickerSpeed ("Jet Flicker Speed", Range(0, 20)) = 3
        _JetSoftness ("Jet Softness", Range(0.001, 0.5)) = 0.08
        
        _DustIntensity ("Concave Dust Intensity", Range(0, 2)) = 0.35
        _DustAmount ("Concave Dust Amount", Range(0, 1)) = 0.45
        _DustSpread ("Concave Dust Spread", Range(0.01, 0.6)) = 0.32
        
        _NoiseScale ("Noise Scale", Range(1, 40)) = 10
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.45
        _DetailScale ("Detail Scale", Range(1, 80)) = 24
        _DetailStrength ("Detail Strength", Range(0, 1)) = 0.18
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Background+3"
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
            
            fixed4 _ShellDarkColor;
            fixed4 _ShellBrightColor;
            fixed4 _CoreColor;
            fixed4 _JetColor;
            
            float _CanvasScale;
            float _Intensity;
            float _Alpha;
            
            float4 _CoreOffset;
            float _CoreRadius;
            float _CoreGlowRadius;
            float _CoreIntensity;
            float _CorePulseSpeed;
            float _CorePulseStrength;
            
            float _ShellRadius;
            float _ShellThickness;
            float _ShellSoftness;
            float _ShellPower;
            float _ShellCoverage;
            float _InnerCrescentRadiusOffset;
            float _InnerCrescentThickness;
            float _InnerCrescentSoftness;
            float _InnerCrescentIntensity;
            
            float _BandRadiusOffset;
            float _BandThickness;
            float _BandIntensity;
            float _BandSoftness;
            
            float _JetLength;
            float _JetWidth;
            float _JetSpread;
            float _JetIntensity;
            float _JetFalloff;
            float _JetAngle;
            float _JetFlicker;
            float _JetFlickerSpeed;
            float _JetSoftness;
            
            float _DustIntensity;
            float _DustAmount;
            float _DustSpread;
            
            float _NoiseScale;
            float _NoiseStrength;
            float _DetailScale;
            float _DetailStrength;
            
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
                
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }
            
            float2 rotate2D(float2 p, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);
                
                return float2(p.x * c - p.y * s, p.x * s + p.y * c);
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
                float2 p = (input.uv - 0.5) * _CanvasScale;
                
                // rotate the whole effect so +X is always the convex/jet side
                float2 rp = rotate2D(p, -_JetAngle);
                
                float shellDistFromCenter = length(rp);
                float angle = atan2(rp.y, rp.x);
                
                float2 coreP = rp - _CoreOffset.xy;
                float coreDist = length(coreP);
                
                float n1 = noise(input.uv * _NoiseScale + _Time.y * 0.05);
                float n2 = noise(input.uv * _DetailScale - _Time.y * 0.12);
                
                float textureNoise = lerp(1.0, n1, _NoiseStrength) * lerp(1.0, n2, _DetailStrength);
                
                // ------------------------------------------------------------
                // CORE: fixed at true disk center
                // ------------------------------------------------------------
                float pulse = 1.0 + sin(_Time.y * _CorePulseSpeed) * _CorePulseStrength;
                float core = 1.0 - smoothstep(_CoreRadius * pulse, _CoreRadius * pulse * 2.0, coreDist);
                float coreGlow = 1.0 - smoothstep(_CoreRadius, _CoreGlowRadius * pulse, coreDist);
                
                // ------------------------------------------------------------
                // CRESCENT SHELL: partial ring around same center as core
                // Convex side is +X. Concave side is -X
                // ------------------------------------------------------------
                float shellDist = abs(shellDistFromCenter - _ShellRadius);
                float shell = 1.0 - smoothstep(_ShellThickness, _ShellThickness + _ShellSoftness, shellDist);
                
                // gap is centered on the concave side, opposite the jet
                // jet/convex side is +X, concave gap is -X
                float concaveAngle = 3.14159265;
                
                // 1.0 coverage = full circle
                // 0.75 coverage = 25% missing gap
                float gapAngle = (1.0 - _ShellCoverage) * 6.2831853;
                float halfGap = gapAngle * 0.5;
                
                // angular distance from the concave/gap direction
                float angleDelta = abs(atan2(
                    sin(angle - concaveAngle),
                    cos(angle - concaveAngle)
                ));
                
                // hard-coded angular blur around the missing gap edges
                float gapBlur = 0.95;
                
                // 0 inside gap, softly fades to 1 outside gap
                float crescentMask = smoothstep(halfGap - gapBlur, halfGap + gapBlur, angleDelta);
                
                shell *= crescentMask;
                
                // ------------------------------------------------------------
                // INNER BRIGHT CRESCENT: extra matter/energy inside the shell,
                // strongest around the core side and curving with the shell.
                // ------------------------------------------------------------
                float innerRadius = _ShellRadius + _InnerCrescentRadiusOffset;
                float innerDist = abs(shellDistFromCenter - innerRadius);
                
                float innerCrescent = 1.0 - smoothstep(
                    _InnerCrescentThickness,
                    _InnerCrescentThickness + _InnerCrescentSoftness,
                    innerDist
                );
                
                // Use same angular crescent coverage as the main shell.
                innerCrescent *= crescentMask;
                
                // Bias brightness toward the convex/core side.
                float innerConvexBias = smoothstep(-0.35, 0.85, rp.x / max(_ShellRadius, 0.001));
                
                innerCrescent *= innerConvexBias;
                innerCrescent *= textureNoise;
                
                shell = pow(saturate(shell), _ShellPower);
                shell *= textureNoise;
                
                // ------------------------------------------------------------
                // INNER DIM BAND: faint darker partial band inside convex shell
                // this subtracts light from the convex side of the shell
                // ------------------------------------------------------------
                float bandRadius = _ShellRadius + _BandRadiusOffset;
                float bandDist = abs(shellDistFromCenter - bandRadius);
                
                float band = 1.0 - smoothstep(_BandThickness, _BandThickness + _BandSoftness, bandDist);
                
                band *= smoothstep(-0.15, 0.85, rp.x / max(_ShellRadius, 0.001));
                band *= _BandIntensity;
                shell *= saturate(1.0 - band);
                
                // ------------------------------------------------------------
                // JET: soft, smoky, cyclic beam from the core toward convex side
                // perpendicular to the direction of travel from the core
                // ------------------------------------------------------------
                float jetAlong = saturate(coreP.x / max(_JetLength, 0.001));
                
                // starts at core, fades out near end.
                float jetOriginFade = smoothstep(-_JetWidth * 2.0, _JetWidth * 3.0,coreP.x);
                float jetEndFade = 1.0 - smoothstep(_JetLength * 0.72, _JetLength, coreP.x);
                
                float jetActive = jetOriginFade * jetEndFade;
                
                // slow perpendicular undulation
                // this fakes the side-view “rotating hose” effect without becoming a harsh squiggle
                float centerWave =
                    sin(jetAlong * 7.0 + _Time.y * 0.85) * 0.030 +
                    sin(jetAlong * 13.0 - _Time.y * 0.45) * 0.014;
                
                // slight skew as it travels outward
                float skew = jetAlong * 0.045;
                
                // curved centerline of the jet
                float jetCenterY = centerWave + skew;
                
                // keep width mostly controlled, not a strong V
                // it widens slightly, then softens into a smoky tail
                float jetWidth = _JetWidth + _JetSpread * 0.35 * smoothstep(0.15, 1.0, jetAlong);
                float jetAcross = abs(coreP.y - jetCenterY);
                
                // soft core + broader smoky envelope
                float jetInner = 1.0 - smoothstep(jetWidth * 0.25, jetWidth + _JetSoftness, jetAcross);
                float jetOuter = 1.0 - smoothstep(jetWidth, jetWidth * 3.8 + _JetSoftness * 4.0, jetAcross);
                
                // moving noise along the jet axis
                float jetNoiseA = noise(float2(
                    jetAlong * 18.0 - _Time.y * 0.65,
                    coreP.y * 22.0 + _Time.y * 0.18
                ));
                
                float jetNoiseB = noise(float2(
                    jetAlong * 42.0 + _Time.y * 0.35,
                    coreP.y * 35.0 - _Time.y * 0.28
                ));
                
                float jetBreakup = lerp(1.0, jetNoiseA, 0.45) * lerp(1.0, jetNoiseB, 0.25);
                
                // subtle cyclic brightness bands moving outward
                float flowBands = 0.72 + 0.28 * sin(jetAlong * 22.0 - _Time.y * 2.0);
                
                // fade with distance, but less aggressively than before
                float jetFade = pow(saturate(1.0 - jetAlong), _JetFalloff);
                
                // combine inner brightness with smoky envelope
                float jetShape = saturate(jetInner * 0.42 + jetOuter * 0.58);
                
                float jetFlicker = lerp(1.0, 
                    0.88 + 0.12 * noise(float2(jetAlong * 10.0, _Time.y * _JetFlickerSpeed)), _JetFlicker);
                
                float jet = jetShape * jetActive * jetFade * jetBreakup * flowBands * jetFlicker;
                float originPlume = 1.0 - smoothstep(_JetWidth * 0.5, _JetWidth * 5.0 + _JetSoftness, length(coreP));
                jet += originPlume * 0.18;
                
                // ------------------------------------------------------------
                // SECONDARY REVERSE JET:
                // shorter, dimmer jet emitted in the opposite direction
                // ------------------------------------------------------------
                
                // mirror the jet axis
                float reverseAlong = saturate((-coreP.x) / max(_JetLength * 0.5, 0.001));
                
                float reverseActive = smoothstep(-_JetWidth * 2.0, _JetWidth * 3.0, -coreP.x) *
                    (1.0 - smoothstep(
                            (_JetLength * 0.5) * 0.72,
                            (_JetLength * 0.5),
                            -coreP.x));
                
                // reverse-side undulation
                float reverseWave =
                    sin(reverseAlong * 6.5 + _Time.y * 0.75) * 0.020 +
                    sin(reverseAlong * 11.0 - _Time.y * 0.35) * 0.010;
                
                // opposite skew direction
                float reverseSkew = -reverseAlong * 0.025;
                float reverseCenterY = reverseWave + reverseSkew;
                
                float reverseWidth =
                    (_JetWidth * 0.85) +
                    (_JetSpread * 0.18) *
                    smoothstep(0.15, 1.0, reverseAlong);
                
                float reverseAcross = abs(coreP.y - reverseCenterY);
                
                float reverseInner = 1.0 - smoothstep(reverseWidth * 0.25, reverseWidth + _JetSoftness, reverseAcross);
                float reverseOuter = 1.0 - smoothstep(reverseWidth, reverseWidth * 3.4 + _JetSoftness * 4.0, 
                    reverseAcross);
                
                float reverseNoiseA = noise(float2(reverseAlong * 16.0 + _Time.y * 0.45,
                    coreP.y * 20.0 - _Time.y * 0.15));
                float reverseNoiseB = noise(float2(reverseAlong * 32.0 - _Time.y * 0.25,
                    coreP.y * 28.0 + _Time.y * 0.22));
                
                float reverseBreakup =
                    lerp(1.0, reverseNoiseA, 0.35) *
                    lerp(1.0, reverseNoiseB, 0.18);
                
                float reverseBands = 0.78 + 0.22 * sin(reverseAlong * 18.0 - _Time.y * 1.5);
                float reverseFade = pow(saturate(1.0 - reverseAlong), _JetFalloff * 1.35);
                float reverseShape = saturate(reverseInner * 0.38 + reverseOuter * 0.62);
                
                float reverseJet =reverseShape * reverseActive * reverseFade * reverseBreakup *
                    reverseBands * jetFlicker;
                
                // brighter than the primary jet
                reverseJet *= 1.5;
                
                // ------------------------------------------------------------
                // CONCAVE DUST/WISPS: subtle particles on -X side(?) of shell
                // ------------------------------------------------------------
                float concaveSide = smoothstep(0.15, -0.75, rp.x / max(_ShellRadius, 0.001));
                
                float dustRing = 1.0 - smoothstep(_DustSpread, _DustSpread + 0.18, shellDist);
                float dustBreakup = smoothstep(1.0 - _DustAmount, 1.0, n1);
                float dust = concaveSide * dustRing * dustBreakup * _DustIntensity;
                
                // ------------------------------------------------------------
                // let's tie it all together...
                // ------------------------------------------------------------
                float shellAlpha = shell * 0.75;
                float innerCrescentAlpha = innerCrescent * 0.55 * _InnerCrescentIntensity;
                float dustAlpha = dust * 0.35;
                float jetAlpha = jet * 0.8;
                float reverseJetAlpha = reverseJet * 0.65;
                float coreAlpha = saturate(core + coreGlow * 0.45);
                
                float3 shellColor = lerp(_ShellDarkColor.rgb, _ShellBrightColor.rgb, saturate(shell + dust));
                
                float3 color =
                    shellColor * shellAlpha +
                    _ShellBrightColor.rgb * innerCrescentAlpha +
                    shellColor * dustAlpha +
                    _JetColor.rgb * jetAlpha * _JetIntensity +
                    _JetColor.rgb * reverseJetAlpha * (_JetIntensity * 0.75) +
                    _CoreColor.rgb * coreAlpha * _CoreIntensity;
                
                float alpha = saturate(
                    shellAlpha +
                    innerCrescentAlpha +
                    dustAlpha +
                    jetAlpha +
                    reverseJetAlpha +
                    coreAlpha
                ) * _Alpha;
                
                return fixed4(color * _Intensity, alpha);
            }
            ENDHLSL
        }
    }
}