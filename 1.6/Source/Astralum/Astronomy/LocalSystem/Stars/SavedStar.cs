using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.LocalSystem.Stars
{
    public class SavedStar : IExposable
    {
        #region Star/System Informational Properties
        public string systemName;
        public string starName;
        public SpectralClass spectralClass;
        public double age;
        public float temperatureKelvin;
        public float magneticField;
        public float radius;
        public float luminosity;
        public float mass;
        public Dictionary<string, float> composition;
        public float metallicity;
        public StellarVariabilityUtil.StellarVariabilityType variabilityType;
        #endregion
        
        #region Star/System Visual Properties
        public Color chromaticity;
        public Color corona;
        public float rotation;
        public float chromaticityIntensity = 1f;
        public float coronaIntensity = 1f;
        public float outerCoronaIntensity = 0.25f;
        public float chromaticityFalloffPower = 2f;
        public float coronaPower = 5f;
        public float outerCoronaPower = 6f;
        public float surfaceNoiseStrength;
        public float variabilityAmount;
        public float variabilitySpeed;
        #endregion
        
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
            magneticField = generatedStar.MagneticField;
            radius = generatedStar.Radius;
            luminosity = generatedStar.Luminosity;
            mass = generatedStar.Mass;
            composition = generatedStar.Composition;
            metallicity = generatedStar.Metallicity;
            variabilityType = generatedStar.VariabilityType;
            
            chromaticity = generatedStar.Chromaticity;
            corona = generatedStar.Corona;
            rotation = generatedStar.Rotation;
            chromaticityIntensity = generatedStar.ChromaticityIntensity;
            coronaIntensity = generatedStar.CoronaIntensity;
            outerCoronaIntensity = generatedStar.OuterCoronaIntensity;
            chromaticityFalloffPower = generatedStar.ChromaticityFalloffPower;
            coronaPower = generatedStar.CoronaPower;
            outerCoronaPower = generatedStar.OuterCoronaPower;
            surfaceNoiseStrength = generatedStar.SurfaceNoiseStrength;
            variabilityAmount = generatedStar.VariabilityAmount;
            variabilitySpeed = generatedStar.VariabilitySpeed;
        }
        
        public void ExposeData()
        {
            Scribe_Values.Look(ref systemName, "systemName");
            Scribe_Values.Look(ref starName, "starName");
            Scribe_Values.Look(ref spectralClass, "spectralClass", SpectralClass.G);
            Scribe_Values.Look(ref age, "age", 7500000000);
            Scribe_Values.Look(ref temperatureKelvin, "temperatureKelvin", 5800f);
            Scribe_Values.Look(ref magneticField, "magneticField", 0.002f);
            Scribe_Values.Look(ref radius, "radius", 1f);
            Scribe_Values.Look(ref luminosity, "luminosity", 1f);
            Scribe_Values.Look(ref mass, "mass", 1f);
            Scribe_Collections.Look(ref composition, "compositionElements", LookMode.Value, LookMode.Value);
            Scribe_Values.Look(ref metallicity, "metallicity", 2f);
            Scribe_Values.Look(ref variabilityType, "variabilityType");
            
            Scribe_Values.Look(ref chromaticity, "chromaticity", 
                new Color(1f, 0.93f, 0.89f, 1f));
            Scribe_Values.Look(ref corona, "corona", 
                new Color(1f, 0.93f, 0.89f, 1f));
            Scribe_Values.Look(ref rotation, "rotation", 0.5f);
            Scribe_Values.Look(ref chromaticityIntensity, "chromaticityIntensity", 1f);
            Scribe_Values.Look(ref coronaIntensity, "coronaIntensity", 1f);
            Scribe_Values.Look(ref outerCoronaIntensity, "outerCoronaIntensity", 0.25f);
            Scribe_Values.Look(ref chromaticityFalloffPower, "chromaticityFalloffPower", 2f);
            Scribe_Values.Look(ref coronaPower, "coronaPower", 5f);
            Scribe_Values.Look(ref outerCoronaPower, "outerCoronaPower", 6f);
            Scribe_Values.Look(ref surfaceNoiseStrength, "surfaceNoiseStrength", 0.025f);
            Scribe_Values.Look(ref variabilityAmount, "variabilityAmount");
            Scribe_Values.Look(ref variabilitySpeed, "variabilitySpeed");

            if (Scribe.mode == LoadSaveMode.PostLoadInit && composition == null)
            {
                composition = new Dictionary<string, float>
                {
                    { "H", 74f },
                    { "He", 24f },
                    { "O", 1f },
                    { "Fe", 1f }
                };
            }
        }
    }
}