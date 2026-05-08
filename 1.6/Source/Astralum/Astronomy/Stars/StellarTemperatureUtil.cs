using UnityEngine;

namespace Astralum.Astronomy.Stars
{
    public static class StellarTemperatureUtil
    {
        private static float GetTemperatureRange(SpectralClass spectralClass)
        {
            return spectralClass switch
            {
                SpectralClass.O => Random.Range(30000f, 50000f),
                SpectralClass.B => Random.Range(10000f, 30000f),
                SpectralClass.A => Random.Range(7500f, 10000f),
                SpectralClass.F => Random.Range(6000f, 7500f),
                SpectralClass.G => Random.Range(5200f, 6000f),
                SpectralClass.K => Random.Range(3700f, 5200f),
                SpectralClass.M => Random.Range(2400f, 3700f),
                _ => Random.Range(5200f, 6000f)
            };
        }
        
        public static float GenerateTemperatureKelvin(SpectralClass spectralClass)
        {
            return GetTemperatureRange(spectralClass);
        }
        
        public static string FormatTemperature(float kelvin)
        {
            return $"{kelvin:N0} K";
        }
    }
}