using Verse;

namespace Astralum.Astronomy.LocalSystem.Stars
{
    public static class StellarRotationUtil
    {
        private static float GetRotation(SpectralClass spectralClass)
        {
            return spectralClass switch
            {
                SpectralClass.O => Rand.Range(20f, 30f),
                SpectralClass.B => Rand.Range(5f, 15f),
                SpectralClass.A => Rand.Range(1f, 10f),
                SpectralClass.F => Rand.Range(0.5f, 5f),
                SpectralClass.G => Rand.Range(0.1f, 1f),
                SpectralClass.K => Rand.Range(0.05f, 0.5f),
                SpectralClass.M => Rand.Range(0.01f, 0.05f),
                _ => Rand.Range(0.1f, 1f),
            };
        }
        
        public static float GenerateRotation(SpectralClass spectralClass)
        {
            return GetRotation(spectralClass);
        }
        
        public static string FormatRotation(float rotationsPerDay)
        {
            return rotationsPerDay switch
            {
                >= 1f => $"{rotationsPerDay:0.##} rotations/day",
                >= 0.01f => $"{1f / rotationsPerDay:0.#} day rotation period",
                _ => $"{1f / rotationsPerDay:0} day rotation period"
            };
        }
    }
}