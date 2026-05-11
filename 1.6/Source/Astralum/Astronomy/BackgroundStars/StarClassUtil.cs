using Astralum.Astronomy.Stars;
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
                < 0.015f => SpectralClass.O,
                < 0.040f => SpectralClass.B,
                < 0.070f => SpectralClass.A,
                < 0.110f => SpectralClass.F,
                < 0.180f => SpectralClass.G,
                < 0.300f => SpectralClass.K,
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