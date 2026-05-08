using UnityEngine;

namespace Astralum.Astronomy.Stars
{
    public static class StellarMagneticFieldUtil
    {
        public static float GetMagneticField(SpectralClass spectralClass)
        {
            return spectralClass switch
            {
                SpectralClass.O => Random.Range(0.05f, 1.0f),
                SpectralClass.B => Random.Range(0.03f, 0.05f),
                SpectralClass.A => Random.Range(0.01f, 0.03f),
                SpectralClass.F => Random.Range(0.003f, 0.01f),
                SpectralClass.G => Random.Range(0.001f, 0.003f),
                SpectralClass.K => Random.Range(0.0005f, 0.001f),
                SpectralClass.M => Random.Range(0.0001f, 0.0005f),
                _ => Random.Range(0.001f, 0.003f),
            };
        }
        
        public static float GenerateMagneticField(SpectralClass spectralClass)
        {
            return GetMagneticField(spectralClass);
        }
        
        public static string FormatMagneticField(float magneticField)
        {
            return magneticField switch
            {
                >= 1f => $"{magneticField:0.##} T",
                >= 0.001f => $"{magneticField * 1000f:0.##} mT",
                _ => $"{magneticField * 1_000_000f:0.##} µT"
            };
        }
    }
}