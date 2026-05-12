using System;
using UnityEngine;

namespace Astralum.Astronomy.LocalSystem.Stars
{
    public static class StellarMassUtil
    {
        private static float GetMass(float luminosity)
        {
            return luminosity <= 0 
                ? throw new ArgumentException("Luminosity must be a positive value.") 
                : Mathf.Pow(luminosity, 3f / 4f);
        }
        
        public static float GenerateMass(float luminosity)
        {
            return GetMass(luminosity);
        }
        
        public static string FormatMass(float solarMasses)
        {
            return solarMasses switch
            {
                >= 1000f => $"{solarMasses:N0} M☉",
                >= 10f => $"{solarMasses:N1} M☉",
                >= 1f => $"{solarMasses:0.00} M☉",
                _ => $"{solarMasses:0.000} M☉"
            };
        }
    }
}