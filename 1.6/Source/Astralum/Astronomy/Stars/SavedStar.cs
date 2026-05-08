using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Stars
{
    public class SavedStar : IExposable
    {
        public string systemName;
        public string starName;
        
        public SpectralClass spectralClass;
        public double age;
        public float temperatureKelvin;
        public float rotation;
        public float magneticField;
        public StellarVariabilityUtil.StellarVariabilityType variabilityType;
        public float variabilityAmount;
        public float radius;
        public float luminosity;
        public float mass;
        public Color chromaticity;
        public Color coronaColor;
        public float coronaIntensity = 1f;
        public float chromaticityIntensity = 1f;
        
        public float radiusPower = 1f;
        public float glowPower = 5f;
        
        public SavedStar()
        {
            
        }
        
        public SavedStar(GeneratedStar generatedStar)
        {
            systemName = generatedStar.SystemName;
            starName = generatedStar.StarName;
            
            spectralClass = generatedStar.SpectralClass;
            age = generatedStar.Age;
            temperatureKelvin = generatedStar.TemperatureKelvin;
            rotation = generatedStar.Rotation;
            magneticField = generatedStar.MagneticField;
            variabilityType = generatedStar.VariabilityType;
            variabilityAmount = generatedStar.VariabilityAmount;
            radius = generatedStar.Radius;
            luminosity = generatedStar.Luminosity;
            mass = generatedStar.Mass;
            chromaticity = generatedStar.Chromaticity;
            coronaColor = generatedStar.CoronaColor;
            chromaticityIntensity = generatedStar.ChromaticityIntensity;
            coronaIntensity = generatedStar.CoronaIntensity;
            
            radiusPower = generatedStar.RadiusPower;
            glowPower = generatedStar.GlowPower;
        }
        
        public void ExposeData()
        {
            Scribe_Values.Look(ref systemName, "systemName");
            Scribe_Values.Look(ref starName, "starName");
            
            Scribe_Values.Look(ref spectralClass, "spectralClass", SpectralClass.G);
            Scribe_Values.Look(ref age, "age", 7500000000);
            Scribe_Values.Look(ref temperatureKelvin, "temperatureKelvin", 5800f);
            Scribe_Values.Look(ref rotation, "rotation", 0.5f);
            Scribe_Values.Look(ref magneticField, "magneticField", 0.002f);
            Scribe_Values.Look(ref variabilityType, "variabilityType");
            Scribe_Values.Look(ref variabilityAmount, "variabilityAmount");
            Scribe_Values.Look(ref radius, "radius", 1f);
            Scribe_Values.Look(ref luminosity, "luminosity", 1f);
            Scribe_Values.Look(ref mass, "mass", 1f);
            Scribe_Values.Look(ref chromaticity, "chromaticity", new Color(1f, 0.93f, 0.89f, 1f));
            Scribe_Values.Look(ref coronaColor, "coronaColor", new Color(1f, 0.93f, 0.89f, 1f));
            Scribe_Values.Look(ref chromaticityIntensity, "chromaticityIntensity", 1f);
            Scribe_Values.Look(ref coronaIntensity, "coronaIntensity", 1f);
            
            Scribe_Values.Look(ref radiusPower, "radiusPower", 1f);
            Scribe_Values.Look(ref glowPower, "glowPower", 5f);
        }
    }
}