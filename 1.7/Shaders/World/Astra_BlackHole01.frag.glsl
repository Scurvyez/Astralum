#version 450

#include "Includes/Astra_CelestialFocusShimmer.glsl"

layout(location = 0) in vec2 vUv;
layout(location = 1) in vec4 vColor;

layout(location = 0) out vec4 outColor;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _Time;                      // global, MapRenderFeature

    float _CanvasScale;              // @default = 2.4
    float _ScreenEdgeFadeStart;      // @default = 0.01
    float _ScreenEdgeFadeEnd;        // @default = 0.08

    float _Radius;                   // @default = 0.16
    float _DistortionRadius;         // @default = 1.0
    float _DistortionStrength;       // @default = 0.35
    float _Darkness;                 // @default = 1.0

    float _HorizonFeather;           // @default = 0.025
    float _DistortionFeather;        // @default = 0.16
    float _RingFeather;              // @default = 0.08

    float _FocusShimmer;             // @default = 0.0
    vec4 _FocusShimmerColor;         // @color @default = vec4(0.65, 0.85, 1.0, 1.0)
    float _FocusShimmerSpeed;        // @default = 0.3
    float _FocusShimmerWidth;        // @default = 0.005
    float _FocusShimmerSoftness;     // @default = 0.15
    float _FocusShimmerIntensity;    // @default = 1.0
} u;

layout(set = 2, binding = 1) uniform sampler2D _GrabBlackHoleScreen;

vec2 local_uv_delta_to_screen_uv(vec2 localUvDelta, vec2 screenUv)
{
    vec2 localDx = dFdx(vUv);
    vec2 localDy = dFdy(vUv);
    vec2 screenDx = dFdx(screenUv);
    vec2 screenDy = dFdy(screenUv);
    
    mat2 localDerivative = mat2(localDx, localDy);

    float determinant = localDerivative[0].x * localDerivative[1].y - localDerivative[1].x * localDerivative[0].y;

    if (abs(determinant) < 0.000001)
    return vec2(0.0);
    
    mat2 inverseLocalDerivative = mat2(localDerivative[1].y, -localDerivative[0].y, -localDerivative[1].x, localDerivative[0].x) 
        / determinant;

    vec2 derivativeCoordinates = inverseLocalDerivative * localUvDelta;

    return screenDx * derivativeCoordinates.x + screenDy * derivativeCoordinates.y;
}

void main()
{
    // grab-screen coordinates
    vec2 screenUv = gl_FragCoord.xy / vec2(textureSize(_GrabBlackHoleScreen, 0));
    
    // Screen-edge protection
    float edgeDistance = min(min(screenUv.x, 1.0 - screenUv.x), min(screenUv.y, 1.0 - screenUv.y));
    float screenEdgeFade = smoothstep(u._ScreenEdgeFadeStart, u._ScreenEdgeFadeEnd, edgeDistance);

    // black-hole-local coordinates
    vec2 localOffset = (vUv - 0.5) * u._CanvasScale;
    float dist = length(localOffset);
    vec2 radialDir = localOffset / max(dist, 0.0001);
    
    // distortion region
    float distortionMask = 1.0 - smoothstep(u._DistortionRadius, u._DistortionRadius + u._DistortionFeather, dist);

    // gravitational bending
    float safeDist = max(dist - u._Radius, 0.0001);
    float bend = u._DistortionStrength / safeDist;
    bend = min(bend, u._DistortionStrength * 8.0);

    // fade distortion toward the outer radius
    float outerFade = 1.0 - smoothstep(u._Radius, u._DistortionRadius, dist);
    bend *= outerFade * distortionMask;

    // event-horizon transition
    float horizonFade = smoothstep(u._Radius, u._Radius + u._HorizonFeather, dist);
    bend *= horizonFade;
    bend *= screenEdgeFade;

    // normal gravitational lens
    vec2 normalLensOffset = localOffset - radialDir * bend;

    // einstein ring / image inversion (best part imo)
    float ringCenter = u._Radius + u._RingFeather * 0.65;
    float ringDistance = abs(dist - ringCenter);

    float ringMask = 1.0 - smoothstep(u._RingFeather * 0.75, u._RingFeather * 1.75, ringDistance);
    ringMask *= horizonFade;
    ringMask *= distortionMask;
    ringMask *= screenEdgeFade;
    
    // sample toward the opposite side of the black hole within the strong-lensing ring
    vec2 invertedLensOffset = -localOffset * 0.85;
    float inversionStrength = ringMask * 0.65;

    vec2 distortedLocalOffset = mix(normalLensOffset, invertedLensOffset, inversionStrength);

    // local displacement -> billboard UV displacement
    vec2 distortedUv = 0.5 + distortedLocalOffset / u._CanvasScale;
    vec2 localUvDelta = distortedUv - vUv;

    // billboard UV displacement -> grab-screen displacement
    vec2 screenUvDelta = local_uv_delta_to_screen_uv(localUvDelta, screenUv);
    screenUvDelta *= screenEdgeFade;

    // scene sampling
    vec3 originalColor = texture(_GrabBlackHoleScreen, screenUv).rgb;

    vec2 distortedScreenUv = screenUv + screenUvDelta;
    vec3 distortedColor = texture(_GrabBlackHoleScreen, distortedScreenUv).rgb;

    // gravitational lens composite
    //
    // fade between the untouched and displaced scene inside the shader rather than relying on the framebuffer alpha
    vec3 color = mix(originalColor, distortedColor, distortionMask);

    // event horizon
    float holeMask = 1.0 - smoothstep(u._Radius, u._Radius + u._HorizonFeather, dist);
    color = mix(color, vec3(0.0), holeMask * u._Darkness);
    
    float shimmer = celestial_focus_shimmer(
            screenUv,
            u._Time.y,
            u._FocusShimmerSpeed,
            u._FocusShimmerWidth,
            u._FocusShimmerSoftness
    );

    shimmer *= distortionMask;
    shimmer *= 1.0 - holeMask;

    float centerFalloff = 1.0 - smoothstep(u._Radius, u._DistortionRadius, dist);

    shimmer *= centerFalloff;
    shimmer *= u._FocusShimmer;

    color += u._FocusShimmerColor.rgb * shimmer * u._FocusShimmerIntensity;
    
    // final output
    //
    // the scene-grab alpha channel is zero / unusable...
    // therefore, the shader constructs the complete RGB result itself and writes it opaquely
    outColor = vec4(color, 1.0);
}