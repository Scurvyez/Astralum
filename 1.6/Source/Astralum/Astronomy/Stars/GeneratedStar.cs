using UnityEngine;

namespace Astralum.Astronomy.Stars
{
    public readonly struct GeneratedStar
    {
        public readonly string SystemName;
        public readonly string StarName;
        
        public readonly SpectralClass SpectralClass;
        public readonly double Age;
        public readonly float TemperatureKelvin;
        public readonly float Rotation;
        public readonly float MagneticField;
        public readonly StellarVariabilityUtil.StellarVariabilityType VariabilityType;
        public readonly float VariabilityAmount;
        public readonly float Radius;
        public readonly float Luminosity;
        public readonly float Mass;
        public readonly Color Chromaticity;
        public readonly Color CoronaColor;
        public readonly float ChromaticityIntensity;
        public readonly float CoronaIntensity;
        
        public readonly float RadiusPower;
        public readonly float GlowPower;
        
        public GeneratedStar(string systemName, string starName,
            SpectralClass spectralClass, double age, float temperatureKelvin, float rotation,
            float magneticField, StellarVariabilityUtil.StellarVariabilityType variabilityType, float variabilityAmount,
            float radius, float luminosity, float mass, Color chromaticity, Color coronaColor, 
            float chromaticityIntensity, float coronaIntensity, float radiusPower, float glowPower)
        {
            SystemName = systemName;
            StarName = starName;
            
            SpectralClass = spectralClass;
            Age = age;
            TemperatureKelvin = temperatureKelvin;
            Rotation = rotation;
            MagneticField = magneticField;
            VariabilityType = variabilityType;
            VariabilityAmount = variabilityAmount;
            Radius = radius;
            Luminosity = luminosity;
            Mass = mass;
            Chromaticity = chromaticity;
            CoronaColor = coronaColor;
            ChromaticityIntensity = chromaticityIntensity;
            CoronaIntensity = coronaIntensity;
            
            RadiusPower = radiusPower;
            GlowPower = glowPower;
        }
    }
}