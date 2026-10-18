#version 450

layout(location = 0) in vec4 vColor;
layout(location = 1) in vec2 vUv;
layout(location = 2) in vec3 vWorldPos;

layout(location = 0) out vec4 outColor;

layout(set = 0, binding = 1) uniform FragmentUniforms
{
    vec4 SunDir;
    mat4 WorldToCamera;
    mat4 ViewProj;
} uFragment;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _PlanetSunLightDirection; // global
} uMaterial;

const float PlanetRadius = 100.0;
const float OuterGlowRadius = 18.0;
const float InnerGlowRadius = 7.0;
const float OuterGlowPower = 2.2;
const float InnerGlowPower = 1.5;
const float Intensity = 2.0;
const float NightLight = 0.018;
const float DayLight = 0.20;
const float TerminatorSoftness = 0.25;
const float SunwardPower = 1.7;
const float SunwardBoost = 0.12;
const vec3 GlowColor = vec3(0.706, 0.839, 1.0);

void main()
{
    mat3 viewRotation = mat3(uFragment.WorldToCamera);
    vec3 cameraWorldPos = -(uFragment.WorldToCamera[3].xyz * viewRotation);

    vec3 rayDir = normalize(vWorldPos - cameraWorldPos);
    float rayClosestT = -dot(cameraWorldPos, rayDir);
    vec3 closestPoint = cameraWorldPos + rayDir * rayClosestT;
    float dist = length(closestPoint);

    // distance from planetary limb
    //
    // < 0 = projected planet disk
    // = 0 = silhouette
    // > 0 = outside planet
    float limbDistance = dist - PlanetRadius;

    // inner glow
    float innerDistance = max(-limbDistance, 0.0);

    float innerGlow = 1.0 - smoothstep(0.0, InnerGlowRadius, innerDistance);
    innerGlow = pow(innerGlow, InnerGlowPower);

    // outer atmosphere
    float outerDistance = max(limbDistance, 0.0);
    float outerGlow = 1.0 - smoothstep(0.0, OuterGlowRadius, outerDistance);
    outerGlow = pow(outerGlow, OuterGlowPower);
    
    // select inside/outside profile
    float outsidePlanet = step(PlanetRadius, dist);
    float limbGlow = mix(innerGlow, outerGlow, outsidePlanet);

    // direction around planetary limb
    vec3 limbDirection = closestPoint / max(dist, 0.0001);
    vec3 lightDirection = normalize(-uMaterial._PlanetSunLightDirection.xyz);

    // -1 = opposite the sun
    //  0 = terminator
    // +1 = toward the sun
    float lightAlignment = dot(limbDirection, lightDirection);

    // terminator
    // creates a controlled transition around lightAlignment=0,
    // instead of remapping the entire [-1,1] range
    float dayFactor = smoothstep(-TerminatorSoftness, TerminatorSoftness, lightAlignment);

    // sunward concentration: 0 at the terminator/night side, 1 directly toward the sun
    float sunward = pow(max(lightAlignment, 0.0), SunwardPower);

    // base atmospheric illumination
    // with additional concentration toward the sun-facing limb
    float atmosphereLight = mix(NightLight, DayLight, dayFactor);
    atmosphereLight += sunward * SunwardBoost;
    
    // let the extended outer halo respond more strongly to sunlight
    float outerSunBoost = mix(1.0, 1.35, sunward);
    float profileLightBoost = mix(1.0, outerSunBoost, outsidePlanet);

    // thin bright rim at exact silhouette
    float rim = 1.0 - smoothstep(0.0, 3.0, abs(limbDistance));
    rim *= 0.10;
    
    // final glow
    float glow = (limbGlow + rim) * atmosphereLight * profileLightBoost * Intensity;
    
    outColor = vec4(GlowColor * glow, 1.0);
}