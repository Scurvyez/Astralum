#version 450

#include "Includes/Astra_CelestialFocusShimmer.glsl"

layout(location = 1) in vec2 vUv;

layout(location = 0) out vec4 outColor;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _Time;                      // global, MapRenderFeature
    vec4 _ScreenParams;              // global, published by MapRenderFeature

    vec4 _LineColor;                 // @color @default = vec4(0.45, 0.6, 1.0, 0.45)
    float _Intensity;                // @default = 0.875
    float _BlurStrength;             // @default = 0.45
    
    float _FocusShimmer;             // @default = 0.0
    vec4 _FocusShimmerColor;         // @color @default = vec4(0.65, 0.85, 1.0, 1.0)
    float _FocusShimmerSpeed;        // @default = 0.5
    float _FocusShimmerWidth;        // @default = 0.005
    float _FocusShimmerSoftness;     // @default = 0.5
    float _FocusShimmerIntensity;    // @default = 2.0
} u;

layout(set = 2, binding = 1) uniform sampler2D _MainTex;

float sample_blurred_alpha(vec2 uv)
{
    vec2 texelSize = 1.0 / vec2(textureSize(_MainTex, 0));
    vec2 texel = texelSize * u._BlurStrength;
    
    float a = texture(_MainTex, uv).a * 0.4;
    
    a += texture(_MainTex, uv + vec2(texel.x, 0.0)).a * 0.15;
    a += texture(_MainTex, uv + vec2(-texel.x, 0.0)).a * 0.15;
    a += texture(_MainTex, uv + vec2(0.0, texel.y)).a * 0.15;
    a += texture(_MainTex, uv + vec2(0.0, -texel.y)).a * 0.15;
    
    return clamp(a, 0.0, 1.0);
}

void main()
{
    // flip the texture coords right-side-up
    vec2 uv = vec2(vUv.x, 1.0 - vUv.y);
    
    float lineAlpha = sample_blurred_alpha(uv);
    
    vec3 color = u._LineColor.rgb * lineAlpha * u._Intensity;
    float alpha = lineAlpha * u._LineColor.a;
    
    vec2 screenUv = gl_FragCoord.xy / u._ScreenParams.xy;
    
    float shimmer = celestial_focus_shimmer(
            screenUv,
            u._Time.y, 
            u._FocusShimmerSpeed, 
            u._FocusShimmerWidth, 
            u._FocusShimmerSoftness);
    
    shimmer *= lineAlpha;
    shimmer *= u._FocusShimmer;
    
    color += u._FocusShimmerColor.rgb * shimmer * u._FocusShimmerIntensity;
    
    outColor = vec4(color, alpha);
}