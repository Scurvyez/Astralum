#version 450

layout(location = 0) in vec3 inPosition;    // @vertex Position required
layout(location = 1) in vec4 inColor;       // @vertex Color optional @default = vec4(1.0)
layout(location = 3) in vec4 inUv0;         // @vertex TexCoord0 optional @default = vec4(0.0)

layout(push_constant) uniform Push
{
    mat4 modelMatrix;
} push;

layout(location = 0) out vec2 vUv;
layout(location = 1) out vec4 vColor;

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

    float _FocusShimmer;
    vec4 _FocusShimmerColor;
    float _FocusShimmerSpeed;
    float _FocusShimmerWidth;
    float _FocusShimmerSoftness;
    float _FocusShimmerIntensity;
} u;

void main()
{
    mat3 viewRotation = mat3(uFragment.WorldToCamera);
    vec3 viewTranslation = uFragment.WorldToCamera[3].xyz;
    vec3 cameraWorldPos = -(viewTranslation * viewRotation);

    vec4 world = push.modelMatrix * vec4(inPosition, 1.0);
    world.xyz += cameraWorldPos;

    gl_Position = uVertex.ViewProj * world;

    vUv = inUv0.xy;
    vColor = inColor;
}