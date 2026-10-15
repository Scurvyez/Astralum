#ifndef ASTRALUM_AURORA_NOISE_INCLUDED
#define ASTRALUM_AURORA_NOISE_INCLUDED

const float ASTRA_TWO_PI = 6.28318530717958647692;

float aurora_hash_4d(vec4 p)
{
    return fract(sin(dot(p, vec4(127.1, 311.7, 74.7, 269.5))) * 43758.5453123);
}

float aurora_noise_4d(vec4 p)
{
    vec4 i = floor(p);
    vec4 f = fract(p);

    // quintic interpolation...
    // This should give zero first and second derivatives at
    // lattice boundaries, which should help with the smoothly animated procedural deformation
    f = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);

    float n0000 = aurora_hash_4d(i + vec4(0.0, 0.0, 0.0, 0.0));
    float n1000 = aurora_hash_4d(i + vec4(1.0, 0.0, 0.0, 0.0));
    float n0100 = aurora_hash_4d(i + vec4(0.0, 1.0, 0.0, 0.0));
    float n1100 = aurora_hash_4d(i + vec4(1.0, 1.0, 0.0, 0.0));

    float n0010 = aurora_hash_4d(i + vec4(0.0, 0.0, 1.0, 0.0));
    float n1010 = aurora_hash_4d(i + vec4(1.0, 0.0, 1.0, 0.0));
    float n0110 = aurora_hash_4d(i + vec4(0.0, 1.0, 1.0, 0.0));
    float n1110 = aurora_hash_4d(i + vec4(1.0, 1.0, 1.0, 0.0));

    float n0001 = aurora_hash_4d(i + vec4(0.0, 0.0, 0.0, 1.0));
    float n1001 = aurora_hash_4d(i + vec4(1.0, 0.0, 0.0, 1.0));
    float n0101 = aurora_hash_4d(i + vec4(0.0, 1.0, 0.0, 1.0));
    float n1101 = aurora_hash_4d(i + vec4(1.0, 1.0, 0.0, 1.0));

    float n0011 = aurora_hash_4d(i + vec4(0.0, 0.0, 1.0, 1.0));
    float n1011 = aurora_hash_4d(i + vec4(1.0, 0.0, 1.0, 1.0));
    float n0111 = aurora_hash_4d(i + vec4(0.0, 1.0, 1.0, 1.0));
    float n1111 = aurora_hash_4d(i + vec4(1.0, 1.0, 1.0, 1.0));

    float n000 = mix(n0000, n1000, f.x);
    float n010 = mix(n0100, n1100, f.x);
    float n001 = mix(n0010, n1010, f.x);
    float n011 = mix(n0110, n1110, f.x);

    float n100 = mix(n0001, n1001, f.x);
    float n110 = mix(n0101, n1101, f.x);
    float n101 = mix(n0011, n1011, f.x);
    float n111 = mix(n0111, n1111, f.x);

    float n00 = mix(n000, n010, f.y);
    float n01 = mix(n001, n011, f.y);

    float n10 = mix(n100, n110, f.y);
    float n11 = mix(n101, n111, f.y);

    float n0 = mix(n00, n01, f.z);
    float n1 = mix(n10, n11, f.z);

    return mix(n0, n1, f.w);
}

float aurora_noise(float u, float radius, float offset)
{
    float spatialAngle = u * ASTRA_TWO_PI;
    vec2 spatialCircle = vec2(cos(spatialAngle), sin(spatialAngle)) * radius;
    vec2 temporalCircle = vec2(cos(offset), sin(offset));

    return aurora_noise_4d(vec4(spatialCircle, temporalCircle));
}

float aurora_vertical_noise(float u, float height, float radius, float offset)
{
    float spatialAngle = u * ASTRA_TWO_PI;
    vec2 spatialCircle = vec2(cos(spatialAngle), sin(spatialAngle)) * radius;
    float temporalRadius = 1.0 + height;
    vec2 temporalCircle = vec2(cos(offset), sin(offset)) * temporalRadius;

    return aurora_noise_4d(vec4(spatialCircle, temporalCircle));
}

#endif