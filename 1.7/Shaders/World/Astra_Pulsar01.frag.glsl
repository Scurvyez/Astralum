#version 450

#include "Includes/Astra_ValueNoise2D.glsl"
#include "Includes/Astra_Rotate2D.glsl"
#include "Includes/Astra_CelestialFocusShimmer.glsl"

layout(location = 1) in vec2 vUv;

layout(location = 0) out vec4 outColor;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _Time;                          // global, MapRenderFeature
    float _GameSeconds;                  // global, set by TickManager
    vec4 _ScreenParams;                  // global, published by MapRenderFeature

    vec4 _ShellDarkColor;                // @color @default = vec4(0.015, 0.03, 0.08, 1.0)
    vec4 _ShellBrightColor;              // @color @default = vec4(0.18, 0.55, 1.0, 1.0)
    vec4 _CoreColor;                     // @color @default = vec4(1.0, 1.0, 1.0, 1.0)
    vec4 _JetColor;                      // @color @default = vec4(0.55, 0.85, 1.0, 1.0)

    float _CanvasScale;                  // @default = 3.0
    float _Intensity;                    // @default = 1.1
    float _Alpha;                        // @default = 0.82

    vec4 _CoreOffset;                    // @default = vec4(0.25, 0.0, 0.0, 0.0)
    float _CoreRadius;                   // @default = 0.01
    float _CoreGlowRadius;               // @default = 0.055
    float _CoreIntensity;                // @default = 1.35
    float _CorePulseSpeed;               // @default = 0.8
    float _CorePulseStrength;            // @default = 0.18

    float _ShellRadius;                  // @default = 0.33
    float _ShellThickness;               // @default = 0.095
    float _ShellSoftness;                // @default = 0.64
    float _ShellPower;                   // @default = 3.75
    float _ShellCoverage;                // @default = 0.91

    float _InnerCrescentRadiusOffset;    // @default = -0.11
    float _InnerCrescentThickness;       // @default = 0.055
    float _InnerCrescentSoftness;        // @default = 0.2
    float _InnerCrescentIntensity;       // @default = 1.4

    float _BandRadiusOffset;             // @default = -0.004
    float _BandThickness;                // @default = 0.003
    float _BandIntensity;                // @default = 0.09
    float _BandSoftness;                 // @default = 0.45

    float _JetLength;                    // @default = 0.80
    float _JetWidth;                     // @default = 0.06
    float _JetSpread;                    // @default = 0.15
    float _JetIntensity;                 // @default = 0.92
    float _JetFalloff;                   // @default = 1.25
    float _JetAngle;                     // @default = 0.0
    float _JetFlicker;                   // @default = 0.15
    float _JetFlickerSpeed;              // @default = 0.5
    float _JetSoftness;                  // @default = 0.035

    float _DustIntensity;                // @default = 0.48
    float _DustAmount;                   // @default = 0.42
    float _DustSpread;                   // @default = 0.28

    float _NoiseScale;                   // @default = 7.0
    float _NoiseStrength;                // @default = 0.38
    float _DetailScale;                  // @default = 214.0
    float _DetailStrength;               // @default = 0.19

    float _FocusShimmer;                 // @default = 0.0
    vec4 _FocusShimmerColor;             // @color @default = vec4(0.65, 0.85, 1.0, 1.0)
    float _FocusShimmerSpeed;            // @default = 0.3
    float _FocusShimmerWidth;            // @default = 0.005
    float _FocusShimmerSoftness;         // @default = 0.15
    float _FocusShimmerIntensity;        // @default = 1.0
} u;

void main()
{
    float time = u._Time.y * u._CorePulseSpeed;
    vec2 inputUv = vec2(vUv.x, 1.0 - vUv.y);
    vec2 screenUv = gl_FragCoord.xy / u._ScreenParams.xy;
    
    // local pulsar coordinates
    vec2 p = (inputUv - 0.5) * u._CanvasScale;

    // rotate the whole effect so +x is always the convex / primary-jet side
    vec2 rp = rotate_2d(p, -u._JetAngle);
    float shellDistFromCenter = length(rp);
    float angle = atan(rp.y, rp.x);
    vec2 coreP = rp - u._CoreOffset.xy;
    float coreDist = length(coreP);

    // general surface noise
    float n1 = value_noise_2d(inputUv * u._NoiseScale + time * 0.05);
    float n2 = value_noise_2d(inputUv * u._DetailScale - time * 0.12);

    float textureNoise = mix(1.0, n1, u._NoiseStrength) * mix(1.0, n2, u._DetailStrength);

    // core
    float pulse = 1.0 + sin(time * u._CorePulseSpeed) * u._CorePulseStrength;
    float core = 1.0 - smoothstep(u._CoreRadius * pulse, u._CoreRadius * pulse * 2.0, coreDist);
    float coreGlow = 1.0 - smoothstep(u._CoreRadius, u._CoreGlowRadius * pulse, coreDist);
    
    // crescent shell, convex side = +x, concave side = -x
    float shellDist = abs(shellDistFromCenter - u._ShellRadius);
    float shell = 1.0 - smoothstep(u._ShellThickness, u._ShellThickness + u._ShellSoftness, shellDist);
    
    // gap centered opposite the primary jet
    const float PI = 3.14159265;
    const float TWO_PI = 6.2831853;

    float concaveAngle = PI;
    float gapAngle = (1.0 - u._ShellCoverage) * TWO_PI;
    float halfGap = gapAngle * 0.5;

    float angleDelta = abs(atan(sin(angle - concaveAngle), cos(angle - concaveAngle)));

    // angular blur around missing-gap edges
    float gapBlur = 0.95;
    float crescentMask = smoothstep(halfGap - gapBlur, halfGap + gapBlur, angleDelta);

    shell *= crescentMask;

    // inner bright crescent
    float innerRadius = u._ShellRadius + u._InnerCrescentRadiusOffset;
    float innerDist = abs(shellDistFromCenter - innerRadius);
    float innerCrescent = 1.0 - smoothstep(u._InnerCrescentThickness, u._InnerCrescentThickness 
        + u._InnerCrescentSoftness, innerDist);
    innerCrescent *= crescentMask;

    float innerConvexBias = smoothstep(-0.35, 0.85, rp.x / max(u._ShellRadius, 0.001));
    
    innerCrescent *= innerConvexBias;
    innerCrescent *= textureNoise;
    
    shell = pow(clamp(shell, 0.0, 1.0), u._ShellPower);
    shell *= textureNoise;
    
    // inner dim band
    float bandRadius = u._ShellRadius + u._BandRadiusOffset;
    float bandDist = abs(shellDistFromCenter - bandRadius);
    float band = 1.0 - smoothstep(u._BandThickness, u._BandThickness + u._BandSoftness, bandDist);
    band *= smoothstep(-0.15, 0.85, rp.x / max(u._ShellRadius, 0.001));
    band *= u._BandIntensity;
    
    shell *= clamp(1.0 - band, 0.0, 1.0);
    
    // primary jet
    float jetAlong = clamp(coreP.x / max(u._JetLength, 0.001), 0.0, 1.0);
    float jetOriginFade = smoothstep(-u._JetWidth * 2.0, u._JetWidth * 3.0, coreP.x);
    float jetEndFade = 1.0 - smoothstep(u._JetLength * 0.72, u._JetLength, coreP.x);

    float jetActive = jetOriginFade * jetEndFade;

    // slow perpendicular undulation
    float centerWave = sin(jetAlong * 7.0 + time * 0.85) * 0.030 + sin(jetAlong * 13.0 - time * 0.45) * 0.014;
    float skew = jetAlong * 0.045;
    float jetCenterY = centerWave + skew;

    // Slight widening toward the tail
    float jetWidth = u._JetWidth + u._JetSpread * 0.35 * smoothstep(0.15, 1.0, jetAlong);
    float jetAcross = abs(coreP.y - jetCenterY);


    // bright inner beam and broader smoky envelope
    float jetInner = 1.0 - smoothstep(jetWidth * 0.25, jetWidth + u._JetSoftness, jetAcross);
    float jetOuter = 1.0 - smoothstep(jetWidth, jetWidth * 3.8 + u._JetSoftness * 4.0, jetAcross);

    // moving jet-axis noise
    float jetNoiseA = value_noise_2d(vec2(jetAlong * 18.0 - time * 0.65, coreP.y * 22.0 + time * 0.18));
    float jetNoiseB = value_noise_2d(vec2(jetAlong * 42.0 + time * 0.35, coreP.y * 35.0 - time * 0.28));

    float jetBreakup = mix(1.0, jetNoiseA, 0.45) * mix(1.0, jetNoiseB, 0.25);
    
    // cyclic brightness bands moving outward
    float flowBands = 0.72 + 0.28 * sin(jetAlong * 22.0 - time * 2.0);
    
    // distance falloff
    float jetFade = pow(clamp(1.0 - jetAlong, 0.0, 1.0), u._JetFalloff);
    
    // combine bright core with smoky envelope
    float jetShape = clamp(jetInner * 0.42 + jetOuter * 0.58, 0.0, 1.0);
    
    // global jet flicker
    float jetFlicker = mix(1.0, 0.88 + 0.12 * 
        value_noise_2d(vec2(jetAlong * 10.0, time * u._JetFlickerSpeed)), u._JetFlicker);
    
    float jet = jetShape * jetActive * jetFade * jetBreakup * flowBands * jetFlicker;


    // small plume where the jet leaves the core
    float originPlume = 1.0 - smoothstep(u._JetWidth * 0.5, u._JetWidth * 5.0 + u._JetSoftness, length(coreP));
    jet += originPlume * 0.18;
    
    // secondary reverse jet
    float reverseLength = u._JetLength * 0.5;
    float reverseAlong = clamp((-coreP.x) / max(reverseLength, 0.001), 0.0, 1.0);
    
    float reverseActive = smoothstep(-u._JetWidth * 2.0, u._JetWidth * 3.0, -coreP.x) * 
        (1.0 - smoothstep(reverseLength * 0.72, reverseLength, -coreP.x));
    
    // reverse-side undulation
    float reverseWave = sin(reverseAlong * 6.5 + time * 0.75) * 0.020 + 
        sin(reverseAlong * 11.0 - time * 0.35) * 0.010;

    // opposite skew direction
    float reverseSkew = -reverseAlong * 0.025;
    float reverseCenterY = reverseWave + reverseSkew;
    float reverseWidth = u._JetWidth * 0.85 +

    u._JetSpread * 0.18 * smoothstep(0.15, 1.0, reverseAlong);

    float reverseAcross = abs(coreP.y - reverseCenterY);
    float reverseInner = 1.0 - smoothstep(reverseWidth * 0.25, reverseWidth + u._JetSoftness, reverseAcross);
    float reverseOuter = 1.0 - smoothstep(reverseWidth, reverseWidth * 3.4 + u._JetSoftness * 4.0, reverseAcross);
    
    float reverseNoiseA = value_noise_2d(vec2(reverseAlong * 16.0 + time * 0.45, coreP.y * 20.0 - time * 0.15));
    float reverseNoiseB = value_noise_2d(vec2(reverseAlong * 32.0 - time * 0.25, coreP.y * 28.0 + time * 0.22));

    float reverseBreakup = mix(1.0, reverseNoiseA, 0.35) * mix(1.0, reverseNoiseB, 0.18);
    float reverseBands = 0.78 + 0.22 * sin(reverseAlong * 18.0 - time * 1.5);
    float reverseFade = pow(clamp(1.0 - reverseAlong, 0.0, 1.0), u._JetFalloff * 1.35);
    float reverseShape = clamp(reverseInner * 0.38 + reverseOuter * 0.62, 0.0, 1.0);

    float reverseJet = reverseShape * reverseActive * reverseFade * reverseBreakup * reverseBands * jetFlicker;
    reverseJet *= 1.5;

    // concave dust / wisps
    float concaveSide = 1.0 - smoothstep(-0.75, 0.15, rp.x / max(u._ShellRadius, 0.001));
    float dustRing = 1.0 - smoothstep(u._DustSpread, u._DustSpread + 0.18, shellDist);
    float dustBreakup = smoothstep(1.0 - u._DustAmount, 1.0, n1);

    float dust = concaveSide * dustRing * dustBreakup * u._DustIntensity;
    
    // composite
    float shellAlpha = shell * 0.75;
    float innerCrescentAlpha = innerCrescent * 0.55 * u._InnerCrescentIntensity;
    float dustAlpha = dust * 0.35;
    float jetAlpha = jet * 0.8;
    float reverseJetAlpha = reverseJet * 0.65;
    float coreAlpha = clamp(core + coreGlow * 0.45, 0.0, 1.0);
    
    vec3 shellColor = mix(u._ShellDarkColor.rgb, u._ShellBrightColor.rgb, clamp(shell + dust, 0.0, 1.0));
    
    vec3 color = shellColor * shellAlpha + u._ShellBrightColor.rgb * innerCrescentAlpha + shellColor * dustAlpha + 
        u._JetColor.rgb * jetAlpha * u._JetIntensity + u._JetColor.rgb * reverseJetAlpha * (u._JetIntensity * 0.75) + 
        u._CoreColor.rgb * coreAlpha * u._CoreIntensity;
    
    float alpha = clamp(shellAlpha + innerCrescentAlpha + dustAlpha + jetAlpha + reverseJetAlpha + coreAlpha, 0.0, 1.0) 
        * u._Alpha;

    // focus shimmer
    float shimmer = celestial_focus_shimmer(
            screenUv,
            time,
            u._FocusShimmerSpeed,
            u._FocusShimmerWidth,
            u._FocusShimmerSoftness);

    shimmer *= alpha;
    shimmer *= u._FocusShimmer;
    
    // final color output
    vec3 finalColor = color * u._Intensity;
    finalColor += u._FocusShimmerColor.rgb * shimmer * u._FocusShimmerIntensity;

    outColor = vec4(finalColor, alpha);
}