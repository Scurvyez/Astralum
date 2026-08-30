using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.LocalStars
{
  public static class LocalStarDataUtil
  {
    public static WorldComponent_CelestialObjectDataCache Data => Find.World?.GetComponent<WorldComponent_CelestialObjectDataCache>();
    
    public static SavedLocalStar Create(string id, Vector3 dir, float size, float rotation, string systemName,
      int systemIndex)
    {
      string starName = StellarNamingUtil.GenerateStarName(systemName, systemIndex);
      
      return CelestialObjectDataUtil.CreateNameable<SavedLocalStar>(id, dir, size, starName, rotation,
        star =>
        {
          star.systemName = systemName;
          star.systemIndex = systemIndex;
          
          star.spectralClass = LocalStarGenerationUtil.GenerateRandomSpectralClass();
          star.age = LocalStarGenerationUtil.GenerateAge(star.spectralClass);
          star.temperatureKelvin = LocalStarGenerationUtil.GenerateTemperatureKelvin(star.spectralClass);
          star.magneticField = LocalStarGenerationUtil.GenerateMagneticField(star.spectralClass);
          star.radius = LocalStarGenerationUtil.GenerateRadius(star.spectralClass);
          star.luminosity = LocalStarGenerationUtil.GenerateLuminosity(star.radius, star.temperatureKelvin);
          star.mass = LocalStarGenerationUtil.GenerateMass(star.luminosity);
          GeneratedStellarComposition compositionRaw = LocalStarGenerationUtil.GenerateComposition(star.spectralClass);
          star.composition = compositionRaw.Elements;
          star.metallicity = compositionRaw.Metallicity;
          LocalStarGenerationUtil.GeneratedStellarVariability variability = LocalStarGenerationUtil.GenerateVariability(
            star.spectralClass);
          
          star.variabilityType = variability.Type;
          star.variabilityAmount = variability.Amount;
          star.variabilitySpeed = LocalStarGenerationUtil.GenerateVariabilitySpeed();
          star.coronaIntensity = LocalStarGenerationUtil.GenerateCoronaIntensity(star.temperatureKelvin, star.magneticField,
            star.variabilityType, star.variabilityAmount, star.age, star.luminosity);
          
          star.chromaticity = LocalStarGenerationUtil.GenerateChromaticity(star.spectralClass);
          star.corona = LocalStarGenerationUtil.GenerateCoronaColor(star.chromaticity, star.temperatureKelvin,
            star.magneticField, star.variabilityType, star.variabilityAmount, star.age,
            star.luminosity, star.coronaIntensity);
          
          star.rotationsPerDay = LocalStarGenerationUtil.GenerateRotationsPerDay(star.spectralClass);
          star.chromaticityIntensity = LocalStarGenerationUtil.GenerateChromaticityIntensity();
          star.outerCoronaIntensity = LocalStarGenerationUtil.GenerateCoronaOuterIntensity();
          star.chromaticityFalloffPower = LocalStarGenerationUtil.GenerateChromaticityFalloffPower();
          star.coronaPower = LocalStarGenerationUtil.GenerateCoronaPower();
          star.outerCoronaPower = LocalStarGenerationUtil.GenerateOuterCoronaPower();
          star.surfaceNoiseStrength = LocalStarGenerationUtil.GenerateSurfaceNoiseStrength();
        });
    }
    
    public static SavedLocalStar GetById(string id)
    {
      return Data?.LocalStars.GetById(id);
    }
  }
}