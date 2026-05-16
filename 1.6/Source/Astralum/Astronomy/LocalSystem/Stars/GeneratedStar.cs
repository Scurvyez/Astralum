using System.Collections.Generic;
using UnityEngine;

namespace Astralum.Astronomy.LocalSystem.Stars
{
  public readonly struct GeneratedStar
  {
    #region Star/System Informational Properties

    public readonly string SystemName;
    public readonly string StarName;
    public readonly SpectralClass SpectralClass;
    public readonly double Age;
    public readonly float TemperatureKelvin;
    public readonly float MagneticField;
    public readonly float Radius;
    public readonly float Luminosity;
    public readonly float Mass;
    public readonly Dictionary<string, float> Composition;
    public readonly float Metallicity;
    public readonly StellarVariabilityUtil.StellarVariabilityType VariabilityType;

    #endregion

    #region Star/System Visual Properties

    public readonly Color Chromaticity;
    public readonly Color Corona;
    public readonly float Rotation;
    public readonly float ChromaticityIntensity;
    public readonly float CoronaIntensity;
    public readonly float OuterCoronaIntensity;
    public readonly float ChromaticityFalloffPower;
    public readonly float CoronaPower;
    public readonly float OuterCoronaPower;
    public readonly float SurfaceNoiseStrength;
    public readonly float VariabilityAmount;
    public readonly float VariabilitySpeed;

    #endregion

    public GeneratedStar(string systemName, string starName, SpectralClass spectralClass, double age,
      float temperatureKelvin, float magneticField, float radius, float luminosity, float mass,
      Dictionary<string, float> composition, float metallicity,
      StellarVariabilityUtil.StellarVariabilityType variabilityType, Color chromaticity, Color corona,
      float rotation, float chromaticityIntensity, float coronaIntensity, float outerCoronaIntensity,
      float chromaticityFalloffPower, float coronaPower, float outerCoronaPower, float surfaceNoiseStrength,
      float variabilityAmount, float variabilitySpeed)
    {
      SystemName = systemName;
      StarName = starName;
      SpectralClass = spectralClass;
      Age = age;
      TemperatureKelvin = temperatureKelvin;
      MagneticField = magneticField;
      Radius = radius;
      Luminosity = luminosity;
      Mass = mass;
      Composition = composition;
      Metallicity = metallicity;
      VariabilityType = variabilityType;

      Chromaticity = chromaticity;
      Corona = corona;
      Rotation = rotation;
      ChromaticityIntensity = chromaticityIntensity;
      CoronaIntensity = coronaIntensity;
      OuterCoronaIntensity = outerCoronaIntensity;
      ChromaticityFalloffPower = chromaticityFalloffPower;
      CoronaPower = coronaPower;
      OuterCoronaPower = outerCoronaPower;
      SurfaceNoiseStrength = surfaceNoiseStrength;
      VariabilityAmount = variabilityAmount;
      VariabilitySpeed = variabilitySpeed;
    }
  }
}