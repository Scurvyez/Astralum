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
                < 0.015f => SpectralClass.O,
                < 0.085f => SpectralClass.B,
                < 0.285f => SpectralClass.A,
                _ => SpectralClass.F
            };
        }
    }
}