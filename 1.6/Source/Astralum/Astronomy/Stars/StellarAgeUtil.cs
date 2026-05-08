using UnityEngine;

namespace Astralum.Astronomy.Stars
{
    public static class StellarAgeUtil
    {
        private const double MillionYears = 1_000_000d;
        private const double BillionYears = 1_000_000_000d;
        
        private static double GetAge(SpectralClass spectralClass)
        {
            return spectralClass switch
            {
                SpectralClass.O => Random.Range(5_000_000, 10_000_000),
                SpectralClass.B => Random.Range(50_000_000, 100_000_000),
                SpectralClass.A => Random.Range(500_000_000, 1_000_000_000),
                SpectralClass.F => Random.Range(2_500_000_000, 5_000_000_000),
                SpectralClass.G => Random.Range(5_000_000_000, 10_000_000_000),
                SpectralClass.K => Random.Range(25_000_000_000, 50_000_000_000),
                SpectralClass.M => Random.Range(50_000_000_000, 100_000_000_000),
                _ => Random.Range(5_000_000_000, 10_000_000_000),
            };
        }
        
        public static double GenerateAge(SpectralClass spectralClass)
        {
            return GetAge(spectralClass);
        }
        
        public static string FormatAge(double years)
        {
            return years switch
            {
                >= BillionYears => $"{years / BillionYears:0.0} Gyr",
                >= MillionYears => $"{years / MillionYears:0.0} Myr",
                _ => $"{years:0} years"
            };
        }
    }
}