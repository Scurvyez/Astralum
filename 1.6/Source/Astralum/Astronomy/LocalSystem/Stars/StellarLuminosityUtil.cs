using UnityEngine;

namespace Astralum.Astronomy.LocalSystem.Stars
{
    public static class StellarLuminosityUtil
    {
        public const float SOL_RADIUS_METERS = 695_700_000.0f;
        public const float STEFAN_BOLTZMANN_CONSTANT = 5.670373E-8f;
        public const float SOL_LUMINOSITY_WATTS = 3.828e+26f;
        
        private static float GetLuminosity(float radius, float temperatureKelvin)
        {
            float starRadiusInMeters = radius * SOL_RADIUS_METERS;
            float luminosity = STEFAN_BOLTZMANN_CONSTANT 
                               * (4 * Mathf.PI * Mathf.Pow(starRadiusInMeters, 2) 
                                  * Mathf.Pow(temperatureKelvin, 4));
            
            luminosity /= SOL_LUMINOSITY_WATTS;
            
            return luminosity;
        }
        
        public static float GenerateLuminosity(float radius, float temperatureKelvin)
        {
            return GetLuminosity(radius, temperatureKelvin);
        }
        
        public static string FormatLuminosity(float luminosity)
        {
            return luminosity switch
            {
                >= 1_000_000f => $"{luminosity:E2} L☉",
                >= 1000f => $"{luminosity:N0} L☉",
                >= 1f => $"{luminosity:0.00} L☉",
                >= 0.001f => $"{luminosity:0.000} L☉",
                _ => $"{luminosity:E2} L☉"
            };
        }
    }
}