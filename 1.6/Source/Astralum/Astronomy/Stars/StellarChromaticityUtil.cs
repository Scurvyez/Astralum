using UnityEngine;

namespace Astralum.Astronomy.Stars
{
    public static class StellarChromaticityUtil
    {
        public static Color GetChromaticity(SpectralClass spectralClass)
        {
            return spectralClass switch
            {
                SpectralClass.O => new Color(0.57f, 0.71f, 1f, 1f),
                SpectralClass.B => new Color(0.64f, 0.75f, 1f, 1f),
                SpectralClass.A => new Color(0.84f, 0.88f, 1f, 1f),
                SpectralClass.F => new Color(0.98f, 0.96f, 1f, 1f),
                SpectralClass.G => new Color(1f, 0.93f, 0.89f, 1f),
                SpectralClass.K => new Color(1f, 0.85f, 0.71f, 1f),
                SpectralClass.M => new Color(1f, 0.71f, 0.42f, 1f),
                _ => Color.white
            };
        }
        
        public static Color GenerateChromaticity(SpectralClass spectralClass)
        {
            return GetChromaticity(spectralClass);
        }
        
        public static float GenerateChromaticityIntensity()
        {
            return 2f;
        }
        
        public static float GenerateChromaticityFalloffPower()
        {
            return 5f;
        }
        
        public static float GenerateSurfaceNoiseStrength()
        {
            return 0.025f;
        }
        
        public static float GetChromaticityIntensitySpectralFactor(SpectralClass spectralClass)
        {
            return spectralClass switch
            {
                SpectralClass.O => 0.35f,
                SpectralClass.B => 0.3f,
                SpectralClass.A => 0.2f,
                SpectralClass.F => 0.15f,
                SpectralClass.G => 0.2f,
                SpectralClass.K => 0.72f,
                SpectralClass.M => 0.8f,
                _ => 1.00f
            };
        }
        
        public static string FormatChromaticity(Color chromaticity)
        {
            return
                $"R {chromaticity.r:0.00}, " +
                $"G {chromaticity.g:0.00}, " +
                $"B {chromaticity.b:0.00}";
        }
    }
}