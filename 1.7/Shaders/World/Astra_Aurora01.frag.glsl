#version 450

#include "Includes/Astra_AuroraNoise.glsl"

layout(location = 1) in vec2 vUv;
layout(location = 2) in float vNightFactor;

layout(location = 0) out vec4 outColor;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _Time;                   // global, MapRenderFeature
    float _GameSeconds;           // global, set by TickManager

    float _Seed;                  // @default = 0.0

    vec4 _AuroraColorBottom;      // @color @default = vec4(0.2, 1.0, 0.45, 1.0)
    vec4 _AuroraColorMiddle;      // @color @default = vec4(0.0, 0.7, 1.0, 1.0)
    vec4 _AuroraColorTop;         // @color @default = vec4(0.7, 0.0, 1.0, 1.0)

    float _Intensity;             // @default = 1.5

    float _RayFrequency;          // @default = 24.0
    float _RayStrength;           // @default = 0.65
    float _FilamentStrength;      // @default = 0.45

    float _AnimationSpeed;        // @default = 1.1
    float _DistortionStrength;    // @default = 0.06

    float _WiggleStrength;        // @default = 0.05
    float _WiggleSpeed;           // @default = 0.5

    float _AuroraActive;          // @default = 0.0
} u;

void main()
{
    float time = u._GameSeconds * u._AnimationSpeed;
    float seedOffset = u._Seed * 173.731;

    // horizontal flow
    float flow = aurora_noise(vUv.x, 3.0, seedOffset + time * 0.08);
    float flowOffset = u._DistortionStrength * vUv.y;
    float flowingU = vUv.x + flowOffset;

    // large-scale curtain presence
    float presenceBroad = aurora_noise(flowingU, 2.25, seedOffset * 0.71 + time * 0.012);
    float presenceMedium = aurora_noise(flowingU, 4.75, seedOffset * 1.31 - time * 0.018 + 63.1);
    float presenceNoise = presenceBroad * 0.72 + presenceMedium * 0.28;

    // high threshold deliberately to leave large portions of the underlying spiral visually empty
    // helps break up the spiral :)
    float presenceMask = smoothstep(0.5, 0.95, presenceNoise);

    // oorganic 2D breakup
    float patchShape = aurora_vertical_noise(flowingU, vUv.y * 1.6, 2.75, seedOffset * 1.17 - time * 0.016 + 83.7);
    float patchDetail = aurora_vertical_noise(flowingU, vUv.y * 4.0, 5.0, seedOffset * 0.53 + time * 0.026 + 127.4);
    float patchNoise = patchShape * 0.72 + patchDetail * 0.28;
    float patchMask = smoothstep(0.30, 0.54, patchNoise);

    float visibilityMask = presenceMask * patchMask;

    // irregular vertical extent
    float edgeNoise = aurora_noise(flowingU, 2.5, seedOffset * 0.89 + time * 0.08);
    float topStart = mix(0.48, 0.74, edgeNoise);
    
    float irregularTopFade = 1.0 - smoothstep(topStart, 0.94, vUv.y);
    float bottomEdgeFade = smoothstep(0.0, 0.22, vUv.y);
    float topEdgeFade = 1.0 - smoothstep(0.78, 1.0, vUv.y);
    float edgeFade = bottomEdgeFade * topEdgeFade;
    
    float verticalMask = irregularTopFade * edgeFade * visibilityMask;

    // broad luminosity
    float broadNoise = aurora_noise(flowingU, 3.0, seedOffset * 1.31 + time * 0.12);
    float broadStructure = mix(0.35, 1.0, broadNoise);

    // ray structure
    float fineNoiseA = aurora_noise(flowingU, u._RayFrequency * 0.25, seedOffset * 0.61 + time * 0.35);
    float fineNoiseB = aurora_noise(flowingU, u._RayFrequency * 0.43, seedOffset * 1.43 - time * 0.22 + 31.7);
    float rayNoise = fineNoiseA * 0.65 + fineNoiseB * 0.35;
    float fineRays = smoothstep(0.4, 0.8, rayNoise);
    float rays = broadStructure * mix(1.0, fineRays, u._RayStrength);

    // fine filaments
    float filamentNoise = aurora_noise(flowingU, u._RayFrequency * 0.8, seedOffset * 1.73 - time * 0.30 + 57.1);
    float filaments = smoothstep(0.72, 0.90, filamentNoise);
    rays *= mix(1.0, 1.5, filaments * u._FilamentStrength);

    // vertical luminosity variation
    float verticalVariation = aurora_vertical_noise(flowingU, vUv.y * 3.0, 4.0, seedOffset * 0.47 - time * 0.15 + 17.3);
    rays *= mix(0.35, 1.0, verticalVariation);

    // color morphology
    float colorNoise = aurora_vertical_noise(flowingU, vUv.y * 2.0, 2.0, seedOffset * 1.07 + time * 0.05 + 41.7);
    float colorY = vUv.y + (colorNoise - 0.5) * 0.1;
    float middleBlend = smoothstep(0.55, 0.78, colorY);
    float topBlend = smoothstep(0.78, 1.0, colorY);
   
    vec4 auroraColor = mix(u._AuroraColorBottom, u._AuroraColorMiddle, middleBlend);
    auroraColor = mix(auroraColor, u._AuroraColorTop, topBlend);

    // final output
    float glow = verticalMask * rays * u._Intensity * u._AuroraActive * 10;
    glow *= vNightFactor;
    vec3 color = auroraColor.rgb * glow;
    float alpha = auroraColor.a * verticalMask * rays * u._AuroraActive;
    alpha *= vNightFactor;

    outColor = vec4(color, alpha);
}