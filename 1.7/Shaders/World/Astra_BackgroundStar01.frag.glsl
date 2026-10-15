#version 450

layout(location = 1) in vec2 vUv;

layout(location = 0) out vec4 outColor;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _Color;         // @color @default = vec4(0.75, 0.85, 1.0, 1.0)
    float _Intensity;    // @default = 1.4
    float _CorePower;    // @default = 8.0
    float _GlowPower;    // @default = 2.0
} u;

void main()
{
    vec2 centeredUv = vUv - vec2(0.5);
    float dist = length(centeredUv) * 2.0;
    
    float radial = clamp(1.0 - dist, 0.0, 1.0);
    
    float core = pow(radial, u._CorePower) * 1.1;
    float glow = pow(radial, u._GlowPower) * 0.25;
    
    float brightness = clamp(core + glow, 0.0, 1.0);
    
    vec3 color = u._Color.rgb * brightness * u._Intensity;
    float alpha = brightness * u._Color.a;
    
    outColor = vec4(color, alpha);
}