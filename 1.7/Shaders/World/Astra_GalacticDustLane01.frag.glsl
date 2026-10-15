#version 450

#include "Includes/Astra_FbmValueNoise2D.glsl"
#include "Includes/Astra_Rotate2D.glsl"

layout(location = 1) in vec2 vUv;

layout(location = 0) out vec4 outColor;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _ColorA;             // @color @default = vec4(0.035, 0.030, 0.045, 1.0)
    vec4 _ColorB;             // @color @default = vec4(0.10, 0.075, 0.055, 1.0)

    float _Alpha;             // @default = 0.1
    float _CanvasScale;       // @default = 1.0

    float _NoiseScale;        // @default = 4.0
    float _NoiseStrength;     // @default = 0.75

    float _DetailScale;       // @default = 18.0
    float _DetailStrength;    // @default = 0.35

    float _CloudThreshold;    // @default = 0.42
    float _EdgeSoftness;      // @default = 0.32
    float _EdgeFadeStart;     // @default = 0.02
    float _EdgeFadeEnd;       // @default = 0.18

    float _StretchX;          // @default = 1.5
    float _StretchY;          // @default = 0.15

    float _Rotation;          // @default = 0.0
    vec4 _SeedOffset;         // @default = vec4(0.0)
} u;

void main()
{
    vec2 uv = vec2(vUv.x, 1.0 - vUv.y);
    
    float edgeDistance = min(min(uv.x, 1.0 - uv.x), min(uv.y, 1.0 - uv.y));
    
    float quadEdgeFade = smoothstep(0.2, 0.35, edgeDistance);
    
    vec2 p = (uv - 0.5) * u._CanvasScale;
    p = rotate_2d(p, u._Rotation);
    
    p.x /= max(u._StretchX, 0.001);
    p.y /= max(u._StretchY, 0.001);
    
    float radial = length(p);
    
    float edgeMask = 1.0 - smoothstep(0.35, 0.75, radial);
    
    vec2 noiseUv = p + u._SeedOffset.xy;

    vec2 warp = vec2(
            fbm_value_noise_2d(noiseUv + vec2(5.2, 1.3)),
            fbm_value_noise_2d(noiseUv + vec2(1.7, 9.2))
    );

    warp = warp * 2.0 - 1.0;

    float baseNoise = fbm_value_noise_2d(noiseUv * u._NoiseScale + warp * 0.35);
    float detailNoise = fbm_value_noise_2d(noiseUv * u._DetailScale + u._SeedOffset.zw);
    
    float cloud = mix(baseNoise,baseNoise * detailNoise, u._DetailStrength);
    cloud = mix(1.0, cloud, u._NoiseStrength);
    
    float cloudMask = smoothstep(u._CloudThreshold, u._CloudThreshold + u._EdgeSoftness, cloud);
    
    float alpha = cloudMask * edgeMask * quadEdgeFade * u._Alpha;
    vec3 color = mix(u._ColorA.rgb, u._ColorB.rgb, baseNoise);
    
    outColor = vec4(color, alpha);
}