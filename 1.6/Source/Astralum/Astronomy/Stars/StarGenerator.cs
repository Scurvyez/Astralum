using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Stars
{
    public static class StarGenerator
    {
        private static readonly SpectralClass[] RandomSpectralClasses =
        [
            SpectralClass.O, 
            SpectralClass.B, 
            SpectralClass.A, 
            SpectralClass.F, 
            SpectralClass.G, 
            SpectralClass.K,
            SpectralClass.M
        ];
        
        private static SpectralClass GenerateRandomSpectralClass()
        {
            return RandomSpectralClasses.RandomElement();
        }
        
        public static GeneratedStar GenerateRandomStar()
        {
            string systemName = StellarNamingUtil.GenerateSystemName();
            string starName = StellarNamingUtil.GenerateStarName(systemName);
            
            SpectralClass spectralClass = GenerateRandomSpectralClass();
            double age = StellarAgeUtil.GenerateAge(spectralClass);
            float temperatureKelvin = StellarTemperatureUtil.GenerateTemperatureKelvin(spectralClass);
            float rotation = StellarRotationUtil.GenerateRotation(spectralClass);
            float magneticField = StellarMagneticFieldUtil.GenerateMagneticField(spectralClass);
            
            var variability = StellarVariabilityUtil.GenerateVariability(spectralClass);
            StellarVariabilityUtil.StellarVariabilityType variabilityType = variability.Type;
            float variabilityAmount = variability.Amount;
            
            float radius = StellarRadiusUtil.GenerateRadius(spectralClass);
            float luminosity = StellarLuminosityUtil.GenerateLuminosity(radius, temperatureKelvin);
            float mass = StellarMassUtil.GenerateMass(radius);
            
            Color chromaticity = StellarChromaticityUtil.GenerateChromaticity(spectralClass);

            float coronaIntensity = StellarCoronaUtil.GenerateCoronaIntensity(
                temperatureKelvin, magneticField, variabilityType, variabilityAmount, age, luminosity);
            Color coronaColor = StellarCoronaUtil.GenerateCoronaColor(
                chromaticity, temperatureKelvin, magneticField, variabilityType, variabilityAmount, age, 
                luminosity, coronaIntensity);
            
            return new GeneratedStar(systemName, starName,
                spectralClass, age, temperatureKelvin, rotation, magneticField,
                variabilityType, variabilityAmount, radius, luminosity, mass, chromaticity, coronaColor,
                chromaticityIntensity: 1f, coronaIntensity: 1f, radiusPower: 1f, glowPower: 5f
            );
        }
    }
}