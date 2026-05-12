using Verse;

namespace Astralum.Astronomy.LocalSystem.Stars
{
    public static class StellarTemperatureUtil
    {
        private static float GetTemperatureRange(SpectralClass spectralClass)
        {
            return spectralClass switch
            {
                SpectralClass.O => Rand.Range(30000f, 50000f),
                SpectralClass.B => Rand.Range(10000f, 30000f),
                SpectralClass.A => Rand.Range(7500f, 10000f),
                SpectralClass.F => Rand.Range(6000f, 7500f),
                SpectralClass.G => Rand.Range(5200f, 6000f),
                SpectralClass.K => Rand.Range(3700f, 5200f),
                SpectralClass.M => Rand.Range(2400f, 3700f),
                _ => Rand.Range(5200f, 6000f)
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