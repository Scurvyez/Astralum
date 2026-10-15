#ifndef ASTRALUM_CELESTIAL_FOCUS_SHIMMER_INCLUDED
#define ASTRALUM_CELESTIAL_FOCUS_SHIMMER_INCLUDED

float celestial_focus_shimmer(vec2 uv, float time, float speed, float width, float softness)
{
    float diaganol = (uv.x - uv.y) * 0.5;
    
    float padding = width + softness + 0.05;
    float sweepStart = -0.5 - padding;
    float sweepEnd = 0.5 + padding;
    
    float phase = fract(time * speed);
    float sweep = mix(sweepStart, sweepEnd, phase);
    float distanceFromBand = abs(diaganol - sweep);
    
    return 1.0 - smoothstep(width, width + softness, distanceFromBand);
}

#endif