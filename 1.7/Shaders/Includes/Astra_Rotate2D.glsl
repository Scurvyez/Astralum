#ifndef ASTRALUM_ROTATE_2D_INCLUDED
#define ASTRALUM_ROTATE_2D_INCLUDED

vec2 rotate_2d(vec2 p, float angle)
{
    float s = sin(angle);
    float c = cos(angle);

    return vec2(p.x * c - p.y * s, p.x * s + p.y * c);
}

#endif