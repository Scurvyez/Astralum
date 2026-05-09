using System.Collections.Generic;
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
            float magneticField = StellarMagneticFieldUtil.GenerateMagneticField(spectralClass);
            float radius = StellarRadiusUtil.GenerateRadius(spectralClass);
            float luminosity = StellarLuminosityUtil.GenerateLuminosity(radius, temperatureKelvin);
            float mass = StellarMassUtil.GenerateMass(luminosity);
            GeneratedStellarComposition compositionRaw = StellarCompositionUtil.GenerateComposition(spectralClass);
            Dictionary<string, float> composition = compositionRaw.Elements;
            float metallicity = compositionRaw.Metallicity;
            var variability = StellarVariabilityUtil.GenerateVariability(spectralClass);
            StellarVariabilityUtil.StellarVariabilityType variabilityType = variability.Type;
            float variabilityAmount = variability.Amount;
            float variabilitySpeed = StellarVariabilityUtil.GenerateVariabilitySpeed();
            float coronaIntensity = StellarCoronaUtil.GenerateCoronaIntensity(
                temperatureKelvin, magneticField, variabilityType, variabilityAmount, age, luminosity);
            
            Color chromaticity = StellarChromaticityUtil.GenerateChromaticity(spectralClass);
            Color corona = StellarCoronaUtil.GenerateCoronaColor(
                chromaticity, temperatureKelvin, magneticField, variabilityType, variabilityAmount, age, 
                luminosity, coronaIntensity);
            float rotation = StellarRotationUtil.GenerateRotation(spectralClass);
            float chromaticityIntensity = StellarChromaticityUtil.GenerateChromaticityIntensity();
            float outerCoronaIntensity = StellarCoronaUtil.GenerateCoronaOuterIntensity();
            float chromaticityFalloffPower = StellarChromaticityUtil.GenerateChromaticityFalloffPower();
            float coronaPower = StellarCoronaUtil.GenerateCoronaPower();
            float outerCoronaPower = StellarCoronaUtil.GenerateOuterCoronaPower();
            float surfaceNoiseStrength = StellarChromaticityUtil.GenerateSurfaceNoiseStrength();
            
            return new GeneratedStar(systemName, starName, spectralClass, age, temperatureKelvin, magneticField,
                radius, luminosity, mass, composition, metallicity, variabilityType, chromaticity, corona, rotation, 
                chromaticityIntensity, coronaIntensity, outerCoronaIntensity, chromaticityFalloffPower, 
                coronaPower, outerCoronaPower, surfaceNoiseStrength, variabilityAmount, variabilitySpeed);
        }
    }
}