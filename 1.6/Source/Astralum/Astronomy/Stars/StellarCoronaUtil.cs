using UnityEngine;

namespace Astralum.Astronomy.Stars
{
    public static class StellarCoronaUtil
    {
        public static float GenerateCoronaIntensity(float temperatureKelvin, float magneticField,
            StellarVariabilityUtil.StellarVariabilityType variabilityType,
            float variabilityAmount, double age, float luminosity)
        {
            float temperatureFactor = Mathf.InverseLerp(2400f, 50000f, temperatureKelvin);
            float magneticFactor = Mathf.InverseLerp(0.0001f, 1.0f, magneticField);
            float luminosityFactor = Mathf.Clamp01(Mathf.Log10(Mathf.Max(luminosity, 0.0001f)) / 6f + 0.5f);
            
            float variabilityFactor = variabilityType == StellarVariabilityUtil.StellarVariabilityType.Intrinsic
                ? Mathf.Clamp01(variabilityAmount * 10f)
                : 0f;
            
            float ageFactor = GetAgeCoronaFactor(age);
            
            float intensity =
                0.65f +
                temperatureFactor * 0.35f +
                magneticFactor * 0.45f +
                luminosityFactor * 0.35f +
                variabilityFactor * 0.25f +
                ageFactor * 0.15f;
            
            return Mathf.Clamp(intensity, 0.4f, 2.5f);
        }
        
        public static Color GenerateCoronaColor(Color chromaticity, float temperatureKelvin, float magneticField,
            StellarVariabilityUtil.StellarVariabilityType variabilityType, float variabilityAmount,
            double age, float luminosity, float coronaIntensity)
        {
            Color temperatureColor = GetTemperatureCoronaTint(temperatureKelvin);
            Color magneticColor = GetMagneticCoronaTint(magneticField);
            Color variabilityColor = GetVariabilityCoronaTint(variabilityType, variabilityAmount);
            Color ageColor = GetAgeCoronaTint(age);
            
            Color coronaColor = chromaticity;
            
            coronaColor = Color.Lerp(coronaColor, temperatureColor, 0.45f);
            coronaColor = Color.Lerp(coronaColor, magneticColor, Mathf.Clamp01(magneticField * 2f));
            coronaColor = Color.Lerp(coronaColor, variabilityColor, Mathf.Clamp01(variabilityAmount * 4f));
            coronaColor = Color.Lerp(coronaColor, ageColor, GetAgeTintStrength(age));
            
            float luminosityBoost = Mathf.Clamp01(Mathf.Log10(Mathf.Max(luminosity, 0.0001f)) / 6f + 0.5f);
            coronaColor = Color.Lerp(coronaColor, Color.white, luminosityBoost * 0.15f);
            
            coronaColor *= Mathf.Lerp(0.9f, 1.25f, Mathf.Clamp01(coronaIntensity / 2.5f));
            coronaColor.a = 1f;
            
            return ClampColor01(coronaColor);
        }

        public static float GenerateCoronaOuterIntensity()
        {
            return 0.25f;
        }

        public static float GenerateCoronaPower()
        {
            return 5f;
        }
        
        public static float GenerateOuterCoronaPower()
        {
            return 6f;
        }
        
        public static string FormatCoronaIntensity(float coronaIntensity)
        {
            return coronaIntensity switch
            {
                >= 2f => $"{coronaIntensity:0.00} intense",
                >= 1.25f => $"{coronaIntensity:0.00} active",
                >= 0.75f => $"{coronaIntensity:0.00} stable",
                _ => $"{coronaIntensity:0.00} weak"
            };
        }
        
        private static Color GetTemperatureCoronaTint(float temperatureKelvin)
        {
            return temperatureKelvin switch
            {
                >= 30000f => new Color(0.35f, 0.58f, 1f, 1f),
                >= 10000f => new Color(0.48f, 0.68f, 1f, 1f),
                >= 7500f => new Color(0.72f, 0.82f, 1f, 1f),
                >= 6000f => new Color(1f, 0.95f, 0.85f, 1f),
                >= 5200f => new Color(1f, 0.82f, 0.48f, 1f),
                >= 3700f => new Color(1f, 0.55f, 0.28f, 1f),
                _ => new Color(1f, 0.28f, 0.16f, 1f)
            };
        }
        
        private static Color GetMagneticCoronaTint(float magneticField)
        {
            return magneticField switch
            {
                >= 0.1f => new Color(0.35f, 0.75f, 1f, 1f),
                >= 0.01f => new Color(0.55f, 0.85f, 1f, 1f),
                >= 0.001f => new Color(0.85f, 0.95f, 1f, 1f),
                _ => Color.white
            };
        }
        
        private static Color GetVariabilityCoronaTint(StellarVariabilityUtil.StellarVariabilityType variabilityType,
            float variabilityAmount)
        {
            if (variabilityType == StellarVariabilityUtil.StellarVariabilityType.None || variabilityAmount <= 0f)
                return Color.white;
            
            return variabilityType == StellarVariabilityUtil.StellarVariabilityType.Intrinsic
                ? new Color(0.75f, 0.55f, 1f, 1f)
                : new Color(1f, 0.85f, 0.65f, 1f);
        }
        
        private static float GetAgeCoronaFactor(double age)
        {
            const double billionYears = 1_000_000_000d;
            double ageGyr = age / billionYears;
            
            return ageGyr switch
            {
                < 0.1d => 0.35f,
                < 1d => 0.2f,
                > 20d => 0.15f,
                _ => 0f
            };
        }
        
        private static Color GetAgeCoronaTint(double age)
        {
            const double billionYears = 1_000_000_000d;
            double ageGyr = age / billionYears;
            
            return ageGyr switch
            {
                < 0.1d => new Color(0.65f, 0.8f, 1f, 1f),
                > 20d => new Color(1f, 0.55f, 0.35f, 1f),
                _ => Color.white
            };
        }
        
        private static float GetAgeTintStrength(double age)
        {
            const double billionYears = 1_000_000_000d;
            double ageGyr = age / billionYears;
            
            return ageGyr switch
            {
                < 0.1d => 0.2f,
                > 20d => 0.15f,
                _ => 0f
            };
        }
        
        private static Color ClampColor01(Color color)
        {
            return new Color(
                Mathf.Clamp01(color.r),
                Mathf.Clamp01(color.g),
                Mathf.Clamp01(color.b),
                Mathf.Clamp01(color.a)
            );
        }
    }
}