#version 450

layout(location = 1) in vec2 vUv;

layout(location = 0) out vec4 outColor;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _CoreColor;     // @color @default = vec4(0.75, 0.9, 1.0, 1.0)
    float _Intensity;    // @default = 2.25
    float _CorePower;    // @default = 4.0
    float _TailPower;    // @default = 2.2
} u;

void main()
{
    float along = clamp(vUv.x, 0.0, 1.0);
    float across = abs(vUv.y - 0.5) * 2.0;

    float tail = pow(along, u._TailPower);
    float core = pow(clamp(1.0 - across, 0.0, 1.0), u._CorePower);

    float alpha = tail * core * u._CoreColor.a;
    vec3 color = u._CoreColor.rgb * u._Intensity;

    outColor = vec4(color, alpha);
}