using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.LocalSystem.Stars
{
    public static class StarGenerator
    {
        private static readonly SpectralClassWeight[] SpectralClassWeights =
        [
            new(SpectralClass.O, 2f),
            new(SpectralClass.B, 4f),
            new(SpectralClass.A, 6f),
            new(SpectralClass.F, 10f),
            new(SpectralClass.G, 18f),
            new(SpectralClass.K, 25f),
            new(SpectralClass.M, 35f)
        ];
        
        // TODO:
        // revert back for release
        private static readonly SpectralClassWeight[] SpectralClassWeights_DEV =
        [
            new(SpectralClass.O, 1f),
            new(SpectralClass.B, 1f),
            new(SpectralClass.A, 1f),
            new(SpectralClass.F, 1f),
            new(SpectralClass.G, 1f),
            new(SpectralClass.K, 1f),
            new(SpectralClass.M, 1f)
        ];
        
        private static SpectralClass GenerateRandomSpectralClass()
        {
            float totalWeight = 0f;
            
            foreach (SpectralClassWeight weight in SpectralClassWeights_DEV)
                totalWeight += weight.Weight;
            
            float random = Rand.Range(0f, totalWeight);
            float cumulativeWeight = 0f;
            
            foreach (SpectralClassWeight weight in SpectralClassWeights_DEV)
            {
                cumulativeWeight += weight.Weight;
                
                if (random <= cumulativeWeight)
                    return weight.SpectralClass;
            }
            
            return SpectralClass.M;
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