#ifndef ASTRALUM_FBM_VALUE_NOISE_2D_INCLUDED
#define ASTRALUM_FBM_VALUE_NOISE_2D_INCLUDED

#include "Includes/Astra_ValueNoise2D.glsl"
#include "Includes/Astra_Rotate2D.glsl"

float fbm_value_noise_2d(vec2 p)
{
    float value = 0.0;
    float amplitude = 0.5;

    value += value_noise_2d(p) * amplitude;

    p = rotate_2d(p * 2.0, 0.5) + vec2(17.13, 9.71);
    amplitude *= 0.5;
    value += value_noise_2d(p) * amplitude;

    p = rotate_2d(p * 2.0, 0.5) + vec2(11.37, 23.91);
    amplitude *= 0.5;
    value += value_noise_2d(p) * amplitude;

    p = rotate_2d(p * 2.0, 0.5) + vec2(31.73, 4.17);
    amplitude *= 0.5;
    value += value_noise_2d(p) * amplitude;

    return value;
}

#endif