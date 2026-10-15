#ifndef ASTRALUM_HASH_LATTICE_INCLUDED
#define ASTRALUM_HASH_LATTICE_INCLUDED

#include "Includes/Astra_HashUint.glsl"

float hash_lattice(ivec2 p)
{
    uint x = uint(p.x);
    uint y = uint(p.y);

    uint h =
    x * 0x8DA6B343u ^
    y * 0xD8163841u;

    h = hash_uint(h);

    return float(h & 0x00FFFFFFu) * (1.0 / 16777216.0);
}

#endif