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
                < 0.01f => SpectralClass.O, // 1%
                < 0.08f => SpectralClass.B, // 7%
                < 0.38f => SpectralClass.A, // 30%
                < 0.78f => SpectralClass.F, // 40%
                _ => SpectralClass.G  // 22%
            };
        }
    }
}