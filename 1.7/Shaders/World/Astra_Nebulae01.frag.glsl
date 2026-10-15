#version 450

#include "Includes/Astra_ValueNoise2D.glsl"
#include "Includes/Astra_Rotate2D.glsl"
#include "Includes/Astra_CelestialFocusShimmer.glsl"

layout(location = 1) in vec2 vUv;

layout(location = 0) out vec4 outColor;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _Time;                      // global, MapRenderFeature
    vec4 _ScreenParams;              // global, published by MapRenderFeature

    vec4 _ColorA;                    // @color @default = vec4(0.12, 0.18, 0.35, 1.0)
    vec4 _ColorB;                    // @color @default = vec4(0.25, 0.45, 1.0, 1.0)
    vec4 _ColorC;                    // @color @default = vec4(0.85, 0.25, 0.95, 1.0)
    vec4 _ColorD;                    // @color @default = vec4(1.0, 0.75, 0.45, 1.0)

    float _ColorStopB;               // @default = 0.33
    float _ColorStopC;               // @default = 0.66
    float _ColorBandSharpness;       // @default = 1.0

    vec4 _SeedOffset;                // @default = vec4(0.0)
    float _Seed;                     // @default = 0.0

    float _Intensity;                // @default = 1.0
    float _Alpha;                    // @default = 0.35

    float _NoiseScale;               // @default = 5.0
    float _NoiseStrength;            // @default = 1.0
    float _CloudThreshold;           // @default = 0.35
    float _EdgeSoftness;             // @default = 0.35

    float _WarpScale;                // @default = 3.0
    float _WarpStrength;             // @default = 0.35
    float _ShapePower;               // @default = 1.5
    vec4 _CoreOffset;                // @default = vec4(0.0)

    float _StretchX;                 // @default = 1.0
    float _StretchY;                 // @default = 1.0
    float _Rotation;                 // @default = 0.0

    float _FocusShimmer;             // @default = 0.0
    vec4 _FocusShimmerColor;         // @color @default = vec4(0.65, 0.85, 1.0, 1.0)
    float _FocusShimmerSpeed;        // @default = 0.3
    float _FocusShimmerWidth;        // @default = 0.02
    float _FocusShimmerSoftness;     // @default = 0.4
    float _FocusShimmerIntensity;    // @default = 10.0
} u;

float fbm(vec2 uv)
{
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;

    for (int i = 0; i < 5; i++)
    {
        value += value_noise_2d(uv * frequency) * amplitude;
        frequency *= 2.0;
        amplitude *= 0.5;
    }

    return value;
}

float ridged_fbm(vec2 uv)
{
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;

    for (int i = 0; i < 5; i++)
    {
        float n = value_noise_2d(uv * frequency);
        n = 1.0 - abs(n * 2.0 - 1.0);
        
        value += n * n * amplitude;
        frequency *= 2.05;
        amplitude *= 0.52;
    }

    return value;
}

float seeded_signed_noise(float seedOffset)
{
    vec2 p = vec2(u._Seed + seedOffset, u._Seed * 1.371 + seedOffset);
    
    return hash_lattice(ivec2(p)) * 2.0 - 1.0;
}

float edge_fade(vec2 uv)
{
    vec2 edgeDistance = min(uv, 1.0 - uv);
    float nearestEdge = min(edgeDistance.x, edgeDistance.y);

    return smoothstep(0.0, 0.085, nearestEdge);
}

vec2 warp_uv(vec2 uv)
{
    float warpA = fbm(uv * u._WarpScale + u._SeedOffset.xy + u._Seed * 0.411);
    float warpB = fbm(uv * u._WarpScale + u._SeedOffset.zw - u._Seed * 0.719);
    
    vec2 warp = vec2(warpA, warpB) * 2.0 - 1.0;

    vec2 secondaryUv = rotate_2d(uv, u._Seed * 0.173 + 1.917);

    float warpC = ridged_fbm(secondaryUv * (u._WarpScale * 0.73 + 0.35) + u._SeedOffset.yx + u._Seed * 0.263);
    float warpD = fbm(secondaryUv * (u._WarpScale * 1.61 + 0.15) - u._SeedOffset.wz - u._Seed * 0.347);

    vec2 secondaryWarp = vec2(warpC, warpD) * 2.0 - 1.0;

    return uv + (warp * 0.72 + secondaryWarp * 0.28) * u._WarpStrength;
}

vec3 nebula_gradient(float t)
{
    t = clamp(t, 0.0, 1.0);

    float stopB = min(u._ColorStopB, u._ColorStopC - 0.01);
    float stopC = max(u._ColorStopC, stopB + 0.01);

    float inBC = step(stopB, t);
    float inCD = step(stopC, t);

    float bandStart = mix(0.0, stopB, inBC);
    bandStart = mix(bandStart, stopC, inCD);

    float bandEnd = mix(stopB, stopC, inBC);
    bandEnd = mix(bandEnd, 1.0, inCD);

    vec3 colorStart = mix(u._ColorA.rgb, u._ColorB.rgb, inBC);
    colorStart = mix(colorStart, u._ColorC.rgb, inCD);

    vec3 colorEnd = mix(u._ColorB.rgb, u._ColorC.rgb, inBC);
    colorEnd = mix(colorEnd, u._ColorD.rgb, inCD);

    float gradientT = clamp((t - bandStart) / max(bandEnd - bandStart, 0.0001), 0.0, 1.0);
    gradientT = pow(gradientT, u._ColorBandSharpness);

    return mix(colorStart, colorEnd, gradientT);
}

void main()
{
    // flip the texture coords right-side-up
    vec2 inputUv = vec2( vUv.x, 1.0 - vUv.y);
    
    vec2 screenUv = gl_FragCoord.xy / u._ScreenParams.xy;
    
    // basic nebula shape
    vec2 uv = inputUv - vec2(0.5);
    uv = rotate_2d(uv, u._Rotation);
    uv.x *= u._StretchX;
    uv.y *= u._StretchY;
    
    // irregular radial silhouette
    vec2 shapeUv = uv - u._CoreOffset.xy;
    float shapeAngle = atan(shapeUv.y, shapeUv.x);
    float shapeRadius = length(shapeUv);
    
    float lobeA = fbm(vec2(shapeAngle * 0.85 + u._Seed * 0.071, shapeRadius * 2.15 - u._Seed * 0.119) 
        + u._SeedOffset.xy);

    float lobeB = ridged_fbm(vec2(shapeAngle * 1.73 - u._Seed * 0.053, shapeRadius * 3.35 + u._Seed * 0.097) 
        + u._SeedOffset.zw);
    
    vec2 asymmetryDirection = normalize(vec2(seeded_signed_noise(11.7),seeded_signed_noise(29.3)));
    float asymmetry = 1.0;
    asymmetry += (lobeA - 0.5) * 0.42;
    asymmetry += (lobeB - 0.5) * 0.26;
    asymmetry += dot(normalize(shapeUv + vec2(0.0001)), asymmetryDirection) * 0.16;
    asymmetry = clamp(asymmetry, 0.0, 1.0);
    
    float radial = length(shapeUv) * 2.0;
    radial /= mix(0.72, 1.28, asymmetry);

    // distort the outer silhouette itself (prevent the damn thing from looking like a square)
    float boundaryNoise = fbm(rotate_2d(shapeUv, u._Seed * 0.137 + 0.83) * 2.5 + u._SeedOffset.xy + u._Seed * 0.173);
    boundaryNoise = boundaryNoise * 2.0 - 1.0;
    
    radial += boundaryNoise * 0.18;
    
    float softShape = 1.0 - smoothstep(1.0 - u._EdgeSoftness, 1.0, radial);
    softShape = pow(clamp(softShape, 0.0, 1.0), u._ShapePower);

    float uvEdgeFade = edge_fade(inputUv);

    // primary domain warp
    vec2 warpedUv = warp_uv(uv);

    vec2 deepWarp = vec2(
            fbm(warpedUv * (u._NoiseScale * 0.31 + 0.15) + u._SeedOffset.xy + u._Seed * 0.811), 
            ridged_fbm(warpedUv * (u._NoiseScale * 0.27 + 0.21) - u._SeedOffset.zw - u._Seed * 0.647)) * 2.0 - 1.0;

    warpedUv += deepWarp * (u._WarpStrength * 0.18 + 0.035);

    // cloud structure
    float noise = fbm(warpedUv * u._NoiseScale + u._SeedOffset.xy + u._Seed * 0.137);
    float detail = fbm(warpedUv * u._NoiseScale * 2.35 + u._SeedOffset.zw - u._Seed * 0.271);
    float fineDetail = fbm(warpedUv * u._NoiseScale * 5.0 + vec2(8.91) + u._Seed * 0.019);
    
    float ridges = ridged_fbm(
            rotate_2d(warpedUv, u._Seed * 0.113 + 0.61) * u._NoiseScale * 1.42 + u._SeedOffset.yw + u._Seed * 0.389);

    // seed-dependent cloud blending
    float cloudBlend = hash_lattice(ivec2(u._Seed * 0.173 + 4.1, u._SeedOffset.x + 8.7));
    float ridgeBlend = hash_lattice(ivec2(u._Seed * 0.419 + 1.9, u._SeedOffset.y - 2.4));

    float cloud = mix(noise, detail, mix(0.22, 0.52, cloudBlend));
    cloud = mix(cloud, fineDetail, 0.12);
    cloud = mix(cloud, ridges, mix(0.10, 0.34, ridgeBlend));

    // large void structure
    float largeVoids = fbm(warpedUv * (u._NoiseScale * 0.38 + 0.2) - u._SeedOffset.xy + u._Seed * 0.223);

    cloud *= smoothstep(0.12, 0.88, largeVoids + asymmetry * 0.3);
    cloud = clamp((cloud - u._CloudThreshold) / max(0.0001, 1.0 - 0.6), 0.0, 1.0);
    cloud = pow(cloud, 2.3);

    // final mask
    float mask = cloud * softShape * uvEdgeFade * u._NoiseStrength;

    // nebula color
    float colorNoise = fbm(warpedUv * u._NoiseScale * 0.65 + u._SeedOffset.zw + u._Seed * 0.593);
    float colorT = clamp((colorNoise - 0.5) * 1.75 + 0.5, 0.0, 1.0);
    vec3 color = nebula_gradient(colorT);

    float alpha = clamp(mask * u._Alpha, 0.0, 1.0);

    vec3 finalColor = color * mask * u._Intensity;

    // focus shimmer
    float shimmer = celestial_focus_shimmer(
            screenUv,
            u._Time.y,
            u._FocusShimmerSpeed,
            u._FocusShimmerWidth,
            u._FocusShimmerSoftness
    );

    shimmer *= mask;
    shimmer *= u._FocusShimmer;

    finalColor += u._FocusShimmerColor.rgb * shimmer * u._FocusShimmerIntensity;
    
    outColor = vec4(finalColor, alpha);
}