#version 450

layout(location = 0) out vec4 outColor;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _LineColor;    // @color @default = vec4(0.35, 0.65, 1.0, 0.28)
} u;

void main()
{
    outColor = u._LineColor;
}