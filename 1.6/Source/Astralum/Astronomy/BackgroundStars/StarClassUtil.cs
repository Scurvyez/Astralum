using Astralum.Astronomy.LocalSystem.Stars;
using Verse;

namespace Astralum.Astronomy.BackgroundStars
{
    public static class StarClassUtil
    {
        public static SpectralClass RandomBackgroundStarClass()
        {
            float value = Rand.Value;
            
            return value switch
            {
                < 0.005f => SpectralClass.O,
                < 0.025f => SpectralClass.B,
                < 0.080f => SpectralClass.A,
                < 0.180f => SpectralClass.F,
                < 0.360f => SpectralClass.G,
                < 0.620f => SpectralClass.K,
                _ => SpectralClass.M
            };
        }
        
        public static SpectralClass RandomConstellationStarClass()
        {
            float value = Rand.Value;
            
            return value switch
            {
                < 0.001f => SpectralClass.O, // 0.1%
                < 0.041f => SpectralClass.B, // 4.0%
                < 0.221f => SpectralClass.A, // 18.0%
                < 0.461f => SpectralClass.F, // 24.0%
                < 0.681f => SpectralClass.G, // 22.0%
                < 0.921f => SpectralClass.K, // 24.0%
                _ => SpectralClass.M         // 7.9%
            };
        }
    }
}