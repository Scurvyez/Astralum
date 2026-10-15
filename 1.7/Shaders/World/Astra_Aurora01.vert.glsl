#version 450

#include "Includes/Astra_AuroraNoise.glsl"

layout(location = 0) in vec3 inPosition;    // @vertex Position required
layout(location = 3) in vec4 inUv0;         // @vertex TexCoord0 optional @default = vec4(0.0, 0.0, 0.0, 0.0)

layout(push_constant) uniform Push
{
    mat4 modelMatrix;
} push;

layout(location = 1) out vec2 vUv;
layout(location = 2) out float vNightFactor;

layout(set = 0, binding = 0) uniform VertexUniforms
{
    mat4 ViewProj;
} uVertex;

layout(set = 0, binding = 1) uniform FragmentUniforms
{
    vec4 SunDir;
    mat4 WorldToCamera;
    mat4 ViewProj;
} uFragment;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _Time;
    float _GameSeconds;

    float _Seed;

    vec4 _AuroraColorBottom;
    vec4 _AuroraColorMiddle;
    vec4 _AuroraColorTop;

    float _Intensity;

    float _RayFrequency;
    float _RayStrength;
    float _FilamentStrength;

    float _AnimationSpeed;
    float _DistortionStrength;

    float _WiggleStrength;
    float _WiggleSpeed;

    float _AuroraActive;
} u;

void main()
{
    vec3 position = inPosition;
    float time = u._GameSeconds * u._WiggleSpeed;
    float seedOffset = u._Seed * 173.731;

    // subtle animated curtain movement
    vec3 radialDirection = vec3(position.x, 0.0, position.z);
    float radialLength = length(radialDirection);

    if (radialLength > 0.0001)
    {
        radialDirection /= radialLength;

        float broadWiggle = aurora_noise(inUv0.x, 2.0, seedOffset + time * 0.12);
        float fineWiggle = aurora_noise(inUv0.x, 5.0, seedOffset * 1.19 - time * 0.18 + 13.7);

        broadWiggle = broadWiggle * 2.0 - 1.0;
        fineWiggle = fineWiggle * 2.0 - 1.0;

        float wiggle = broadWiggle * 0.7 + fineWiggle * 0.3;

        // keep the lower edge comparatively anchored while allowing the upper curtain to move more freely
        float heightMask = smoothstep(0.0, 1.0, inUv0.y);
        float radialDisplacement = wiggle * 7.5 * heightMask;

        position += radialDirection * radialDisplacement;
    }

    // transform
    vec4 world = push.modelMatrix * vec4(position, 1.0);

    // day / night visibility
    vec3 planetNormal = normalize(world.xyz);
    vec3 sunDirection = normalize(uFragment.SunDir.xyz);
    float sunDot = dot(planetNormal, sunDirection);

    vNightFactor = smoothstep(-0.08, 0.08, sunDot);

    // output
    gl_Position = uVertex.ViewProj * world;
    vUv = inUv0.xy;
}