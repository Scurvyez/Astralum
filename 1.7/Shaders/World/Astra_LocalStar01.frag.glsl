#version 450

#include "Includes/Astra_ValueNoise2D.glsl"
#include "Includes/Astra_CelestialFocusShimmer.glsl"

layout(location = 1) in vec2 vUv;

layout(location = 0) out vec4 outColor;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _Time;                         // global, MapRenderFeature
    vec4 _ScreenParams;                 // global, published by MapRenderFeature

    vec4 _Chromaticity;                 // @color @default = vec4(1.0)
    vec4 _Corona;                       // @color @default = vec4(1.0)

    float _SurfaceNoiseStrength;        // @default = 0.025
    float _CoronaRotationSpeed;         // @default = 0.0

    float _ChromaticityIntensity;       // @default = 1.0
    float _CoronaIntensity;             // @default = 1.0
    float _OuterCoronaIntensity;        // @default = 0.25

    float _ChromaticityFalloffPower;    // @default = 2.0
    float _CoronaPower;                 // @default = 5.0
    float _OuterCoronaPower;            // @default = 6.0

    float _VariabilityAmount;           // @default = 0.0
    float _VariabilitySpeed;            // @default = 1.0

    float _ScalingFactor;               // @default = 0.65

    float _FocusShimmer;                // @default = 0.0
    vec4 _FocusShimmerColor;            // @color @default = vec4(0.65, 0.85, 1.0, 1.0)
    float _FocusShimmerSpeed;           // @default = 0.3
    float _FocusShimmerWidth;           // @default = 0.02
    float _FocusShimmerSoftness;        // @default = 0.4
    float _FocusShimmerIntensity;       // @default = 10.0
} u;


void main()
{
    // flip the texture coordinates right-side-up
    vec2 uv = vec2(vUv.x, 1.0 - vUv.y);
    
    vec2 centeredUv = uv - vec2(0.5);
    centeredUv /= max(u._ScalingFactor, 0.0001);
    
    // basic radial star shape
    float dist = length(centeredUv);
    float baseRadial = clamp(1.0 - dist * 2.0, 0.0, 1.0);
    
    // chromaticity / stellar core
    float chromaticityMask = pow(baseRadial, u._ChromaticityFalloffPower);
    vec3 chromaticity = u._Chromaticity.rgb * chromaticityMask * u._ChromaticityIntensity;
    
    // surface noise
    // preserve the original high-frequency appearance while using our shared value-noise implementation
    float surfaceNoise = value_noise_2d(uv * 128.0);
    surfaceNoise = mix(1.0 - u._SurfaceNoiseStrength, 1.0 + u._SurfaceNoiseStrength, surfaceNoise);

    chromaticity *= surfaceNoise;

    float coreAlpha = clamp(chromaticityMask * u._Chromaticity.a, 0.0, 1.0);

    // stellar variability
    float variabilityPulse = 1.0 + sin(u._Time.y * u._VariabilitySpeed) * u._VariabilityAmount;

    // corona masks
    float coronaMask = pow(baseRadial, u._CoronaPower);
    float outerHalo = clamp(1.0 - dist * 1.15, 0.0, 1.0);
    outerHalo = pow(outerHalo, u._OuterCoronaPower);

    // spherical surface projection
    float sphereMask = baseRadial;
    float projectedDepth = sqrt(clamp(1.0 - dot(centeredUv * 2.0, centeredUv * 2.0), 0.0, 1.0));

    // corona rotation
    const float visualDayLengthSeconds = 240.0;
    const float visualRotationScale = 0.1;

    float rotationsPerSecond = u._CoronaRotationSpeed / visualDayLengthSeconds;
    float flow = u._Time.y * rotationsPerSecond * visualRotationScale;
    
    float surfaceX = centeredUv.x;
    float surfaceY = centeredUv.y / max(projectedDepth, 0.15);
    
    vec2 coronaFlowUv = vec2(surfaceX * 2.0, surfaceY + flow);

    // corona surface variation
    float coronaNoiseA = value_noise_2d(coronaFlowUv * 5.0);
    float coronaNoiseB = value_noise_2d(coronaFlowUv * 11.0 + vec2(17.3));
    
    float coronaVariation = mix(coronaNoiseA, coronaNoiseB, 0.35);
    coronaVariation = mix(0.85, 1.15, coronaVariation);
    coronaVariation = mix(1.0, coronaVariation, sphereMask);

    // corona color
    vec3 corona = u._Corona.rgb * coronaMask * u._CoronaIntensity * variabilityPulse * coronaVariation;
    vec3 outerCorona = u._Corona.rgb * outerHalo * u._OuterCoronaIntensity * variabilityPulse * coronaVariation;
    float visibleCoronaMask = clamp(coronaMask * u._CoronaIntensity + outerHalo * u._OuterCoronaIntensity, 0.0, 1.0);

    // combine core + corona
    float exposedCorona = 1.0 - coreAlpha;
    vec3 coronaColor = (corona + outerCorona) * exposedCorona;
    vec3 finalColor = chromaticity + coronaColor - chromaticity * coronaColor;

    // focus shimmer
    vec2 screenUv = gl_FragCoord.xy / u._ScreenParams.xy;

    float shimmer = celestial_focus_shimmer(
            screenUv,
            u._Time.y,
            u._FocusShimmerSpeed,
            u._FocusShimmerWidth,
            u._FocusShimmerSoftness
    );
    
    shimmer *= u._FocusShimmer;
    shimmer *= max(chromaticityMask, visibleCoronaMask);

    float shimmerStrength = clamp(shimmer * u._FocusShimmerIntensity, 0.0, 1.0);
    vec3 shimmerTarget = max(finalColor, u._FocusShimmerColor.rgb);

    finalColor = mix(finalColor, shimmerTarget, shimmerStrength);

    // final coverage
    float coronaAlpha = visibleCoronaMask * u._Corona.a;
    float alpha = max(coreAlpha, coronaAlpha);
    
    vec3 outputColor = finalColor * alpha;
    outColor = vec4(clamp(outputColor * 1.25, 0.0, 1.0), alpha);
}