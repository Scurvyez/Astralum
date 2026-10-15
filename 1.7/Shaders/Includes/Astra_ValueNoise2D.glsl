#ifndef ASTRALUM_VALUE_NOISE_2D_INCLUDED
#define ASTRALUM_VALUE_NOISE_2D_INCLUDED

#include "Includes/Astra_HashLattice.glsl"

float value_noise_2d(vec2 uv)
{
    vec2 cell = floor(uv);
    vec2 f = uv - cell;

    ivec2 i = ivec2(cell);

    float a = hash_lattice(i);
    float b = hash_lattice(i + ivec2(1, 0));
    float c = hash_lattice(i + ivec2(0, 1));
    float d = hash_lattice(i + ivec2(1, 1));

    vec2 u = f * f * (3.0 - 2.0 * f);

    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

#endif