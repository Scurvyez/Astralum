#ifndef ASTRALUM_HASH_UINT_INCLUDED
#define ASTRALUM_HASH_UINT_INCLUDED

uint hash_uint(uint x)
{
    x ^= x >> 16;
    x *= 0x7FEB352Du;
    x ^= x >> 15;
    x *= 0x846CA68Bu;
    x ^= x >> 16;

    return x;
}

#endif