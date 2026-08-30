using System;
using System.Collections.Generic;
using System.Linq;
using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.LocalStars
{
  public static class LocalStarGenerationUtil
  {
    private const double MillionYears = 1_000_000d;
    private const double BillionYears = 1_000_000_000d;
    private const float SolRadiusMeters = 695_700_000.0f;
    private const float StefanBoltzmannConstant = 5.670373E-8f;
    private const float SolLuminosityWatts = 3.828e+26f;
    private const float BaseLocalStarRenderSize = 7.5f;
    
    public const float DistanceToLocalStars = 20f;
    
    public enum StellarVariabilityType
    {
      None,
      Intrinsic,
      Extrinsic
    }
    
    public static void EnsureGenerated()
    {
      WorldComponent_CelestialObjectDataCache data = LocalStarDataUtil.Data;
      
      if (data == null)
        return;
      
      if (data.HasGeneratedLocalStars)
        return;
      
      Rand.PushState();
      
      try
      {
        Rand.Seed = Find.World.info.Seed ^ 0x57A410CA;
        GenerateAndSave(data);
      }
      finally
      {
        Rand.PopState();
      }
    }
    
    public static void GenerateAndSave(WorldComponent_CelestialObjectDataCache data)
    {
      data.ClearLocalStars();
      
      string systemName = StellarNamingUtil.GenerateSystemName();
      int starCount = Rand.RangeInclusive(1, 3);

      Vector3 systemCenter = Vector3.forward * DistanceToLocalStars;
      
      for (int i = 0; i < starCount; i++)
      {
        string id = $"localstar_{Find.World.info.seedString}_{i}";
        float rotation = GenerateRenderRotation(i);

        SavedLocalStar star = LocalStarDataUtil.Create(id, systemCenter, BaseLocalStarRenderSize, 
          rotation, systemName, i);
        data.LocalStars.Add(star);
      }
      
      data.LocalStarSystem = GenerateStarSystem(systemName, starCount);
    }
    
    private static SavedLocalStarSystem GenerateStarSystem(string systemName, int starCount)
    {
      SavedLocalStarSystem system = new()
      {
        systemName = systemName
      };
      
      // random isotropic orientation...
      float cosInclination = Rand.Range(0f, 1f);
      
      system.inclinationRadians = Mathf.Acos(cosInclination);
      system.positionAngleRadians = Rand.Range(0f, Mathf.PI * 2f);
      system.innerInitialPhaseRadians = Rand.Range(0f, Mathf.PI * 2f);
      system.outerInitialPhaseRadians = Rand.Range(0f, Mathf.PI * 2f);

      if (starCount >= 2)
      {
        system.innerOrbitalPeriodTicks = Rand.RangeInclusive(300_000, 1_800_000);
        system.innerSeparation = Rand.Range(1f, 1.35f); // tweak if stars too far apart
      }
      
      if (starCount < 3) 
        return system;
      
      system.outerOrbitalPeriodTicks = Rand.RangeInclusive(3_000_000, 12_000_000);
      system.outerSeparation = Rand.Range(1.6f, 2.5f); // tweak if stars too far apart
      
      return system;
    }
    
    private static float GenerateRenderRotation(int index)
    {
      return 0f;
    }
    
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
    
    public static SpectralClass GenerateRandomSpectralClass()
    {
      float totalWeight = 0f;
      
      foreach (SpectralClassWeight weight in SpectralClassWeights)
        totalWeight += weight.Weight;
      
      float random = Rand.Range(0f, totalWeight);
      float cumulativeWeight = 0f;
      
      foreach (SpectralClassWeight weight in SpectralClassWeights)
      {
        cumulativeWeight += weight.Weight;
        
        if (random <= cumulativeWeight)
          return weight.SpectralClass;
      }
      
      return SpectralClass.M;
    }
    
    private static double GetAge(SpectralClass spectralClass)
    {
      return spectralClass switch
      {
        SpectralClass.O => Rand.Range(5_000_000, 10_000_000),
        SpectralClass.B => Rand.Range(50_000_000, 100_000_000),
        SpectralClass.A => Rand.Range(500_000_000, 1_000_000_000),
        SpectralClass.F => Rand.Range(2_500_000_000, 5_000_000_000),
        SpectralClass.G => Rand.Range(5_000_000_000, 10_000_000_000),
        SpectralClass.K => Rand.Range(25_000_000_000, 50_000_000_000),
        SpectralClass.M => Rand.Range(50_000_000_000, 100_000_000_000),
        _ => Rand.Range(5_000_000_000, 10_000_000_000)
      };
    }
    
    public static double GenerateAge(SpectralClass spectralClass)
    {
      return GetAge(spectralClass);
    }
    
    public static string FormatAge(double years)
    {
      return years switch
      {
        >= BillionYears => $"{years / BillionYears:0.0} Gyr",
        >= MillionYears => $"{years / MillionYears:0.0} Myr",
        _ => $"{years:0} " + "Astra_Stars_Years".Translate()
      };
    }
    
    private static float GetTemperatureRange(SpectralClass spectralClass)
    {
      return spectralClass switch
      {
        SpectralClass.O => Rand.Range(30000f, 50000f),
        SpectralClass.B => Rand.Range(10000f, 30000f),
        SpectralClass.A => Rand.Range(7500f, 10000f),
        SpectralClass.F => Rand.Range(6000f, 7500f),
        SpectralClass.G => Rand.Range(5200f, 6000f),
        SpectralClass.K => Rand.Range(3700f, 5200f),
        SpectralClass.M => Rand.Range(2400f, 3700f),
        _ => Rand.Range(5200f, 6000f)
      };
    }
    
    public static float GenerateTemperatureKelvin(SpectralClass spectralClass)
    {
      return GetTemperatureRange(spectralClass);
    }
    
    public static string FormatTemperature(float kelvin)
    {
      return $"{kelvin:N0} K";
    }
    
    public static float GetMagneticField(SpectralClass spectralClass)
    {
      return spectralClass switch
      {
        SpectralClass.O => Rand.Range(0.05f, 1.0f),
        SpectralClass.B => Rand.Range(0.03f, 0.05f),
        SpectralClass.A => Rand.Range(0.01f, 0.03f),
        SpectralClass.F => Rand.Range(0.003f, 0.01f),
        SpectralClass.G => Rand.Range(0.001f, 0.003f),
        SpectralClass.K => Rand.Range(0.0005f, 0.001f),
        SpectralClass.M => Rand.Range(0.0001f, 0.0005f),
        _ => Rand.Range(0.001f, 0.003f)
      };
    }
    
    public static float GenerateMagneticField(SpectralClass spectralClass)
    {
      return GetMagneticField(spectralClass);
    }
    
    public static string FormatMagneticField(float magneticField)
    {
      return magneticField switch
      {
        >= 1f => $"{magneticField:0.##} T",
        >= 0.001f => $"{magneticField * 1000f:0.##} mT",
        _ => $"{magneticField * 1_000_000f:0.##} µT"
      };
    }
    
    private static float GetRadius(SpectralClass spectralClass)
    {
      float randomNumber = Rand.Range(0f, 250f);
      
      return spectralClass switch
      {
        SpectralClass.O when Mathf.Approximately(randomNumber, 250) => Rand.Range(1250f, 1500f),
        SpectralClass.O when randomNumber is <= 249 and >= 240 => Rand.Range(1000f, 1250f),
        SpectralClass.O when randomNumber is <= 239 and >= 230 => Rand.Range(800f, 1000f),
        SpectralClass.O when randomNumber is <= 229 and >= 220 => Rand.Range(500f, 800f),
        SpectralClass.O when randomNumber is <= 219 and >= 200 => Rand.Range(100f, 500f),
        SpectralClass.O when randomNumber is <= 199 and >= 175 => Rand.Range(30f, 100f),
        SpectralClass.O when randomNumber is <= 174 and >= 125 => Rand.Range(10f, 30f),
        SpectralClass.O => Rand.Range(6.6f, 10f),
        SpectralClass.B => Rand.Range(1.8f, 6.6f),
        SpectralClass.A => Rand.Range(1.4f, 1.8f),
        SpectralClass.F => Rand.Range(1.15f, 1.4f),
        SpectralClass.G => Rand.Range(0.96f, 1.15f),
        SpectralClass.K => Rand.Range(0.7f, 0.96f),
        SpectralClass.M => Rand.Range(0.08f, 0.7f),
        _ => Rand.Range(0.96f, 1.15f)
      };
    }
    
    public static float GenerateRadius(SpectralClass spectralClass)
    {
      return GetRadius(spectralClass);
    }
    
    public static string FormatRadius(float solarRadii)
    {
      return solarRadii switch
      {
        >= 1000f => $"{solarRadii:N0} R☉",
        >= 10f => $"{solarRadii:N1} R☉",
        >= 1f => $"{solarRadii:0.00} R☉",
        _ => $"{solarRadii:0.000} R☉"
      };
    }
    
    private static float GetLuminosity(float radius, float temperatureKelvin)
    {
      float starRadiusInMeters = radius * SolRadiusMeters;
      float luminosity = StefanBoltzmannConstant
                         * (4 * Mathf.PI * Mathf.Pow(starRadiusInMeters, 2)
                            * Mathf.Pow(temperatureKelvin, 4));
      
      luminosity /= SolLuminosityWatts;
      
      return luminosity;
    }
    
    public static float GenerateLuminosity(float radius, float temperatureKelvin)
    {
      return GetLuminosity(radius, temperatureKelvin);
    }
    
    public static string FormatLuminosity(float luminosity)
    {
      return luminosity switch
      {
        >= 1_000_000f => $"{luminosity:E2} L☉",
        >= 1000f => $"{luminosity:N0} L☉",
        >= 1f => $"{luminosity:0.00} L☉",
        >= 0.001f => $"{luminosity:0.000} L☉",
        _ => $"{luminosity:E2} L☉"
      };
    }
    
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
    
    public static GeneratedStellarComposition GenerateComposition(SpectralClass spectralClass)
    {
      Dictionary<string, float> elements = spectralClass switch
      {
        SpectralClass.O => new Dictionary<string, float>
        {
          { "H", Rand.Range(74f, 76f) },
          { "He", Rand.Range(24f, 26f) }
        },
        
        SpectralClass.B => new Dictionary<string, float>
        {
          { "H", Rand.Range(58f, 70f) },
          { "He", Rand.Range(28f, 42f) },
          { "C", Rand.Range(0.1f, 2f) },
          { "N", Rand.Range(0.1f, 2f) },
          { "O", Rand.Range(0.1f, 2f) }
        },
        
        SpectralClass.A => new Dictionary<string, float>
        {
          { "H", Rand.Range(71f, 74f) },
          { "He", Rand.Range(25f, 28f) },
          { "C", Rand.Range(0.1f, 2f) },
          { "N", Rand.Range(0.1f, 2f) },
          { "O", Rand.Range(0.1f, 2f) },
          { "Ne", Rand.Range(0.1f, 2f) }
        },
        
        SpectralClass.F => new Dictionary<string, float>
        {
          { "H", Rand.Range(54f, 64f) },
          { "He", Rand.Range(35f, 45f) },
          { "C", Rand.Range(0.1f, 2f) },
          { "N", Rand.Range(0.1f, 2f) },
          { "O", Rand.Range(0.1f, 2f) },
          { "Ne", Rand.Range(0.1f, 2f) },
          { "Fe", Rand.Range(0.1f, 2f) }
        },
        
        SpectralClass.G => new Dictionary<string, float>
        {
          { "H", Rand.Range(74f, 84f) },
          { "He", Rand.Range(14f, 24f) },
          { "C", Rand.Range(0.1f, 2f) },
          { "N", Rand.Range(0.1f, 2f) },
          { "O", Rand.Range(0.1f, 2f) },
          { "Ne", Rand.Range(0.1f, 2f) },
          { "Fe", Rand.Range(0.1f, 2f) }
        },
        
        SpectralClass.K => new Dictionary<string, float>
        {
          { "H", Rand.Range(56f, 64f) },
          { "He", Rand.Range(36f, 44f) },
          { "C", Rand.Range(0.1f, 2f) },
          { "N", Rand.Range(0.1f, 2f) },
          { "O", Rand.Range(0.1f, 2f) },
          { "Ne", Rand.Range(0.1f, 2f) },
          { "Fe", Rand.Range(0.1f, 2f) },
          { "Si", Rand.Range(0.1f, 2f) },
          { "Mg", Rand.Range(0.1f, 2f) }
        },
        
        SpectralClass.M => new Dictionary<string, float>
        {
          { "H", Rand.Range(36f, 56f) },
          { "He", Rand.Range(44f, 64f) },
          { "C", Rand.Range(0.1f, 2f) },
          { "N", Rand.Range(0.1f, 2f) },
          { "O", Rand.Range(0.1f, 2f) },
          { "Ne", Rand.Range(0.1f, 2f) },
          { "Fe", Rand.Range(0.1f, 2f) },
          { "Si", Rand.Range(0.1f, 2f) },
          { "Mg", Rand.Range(0.1f, 2f) },
          { "S", Rand.Range(0.1f, 2f) },
          { "Cl", Rand.Range(0.1f, 2f) },
          { "K", Rand.Range(0.1f, 2f) }
        },
        
        _ => new Dictionary<string, float>
        {
          { "H", 74f },
          { "He", 24f },
          { "O", 1f },
          { "Fe", 1f }
        }
      };
      
      NormalizeToPercent(elements);
      
      return new GeneratedStellarComposition(elements);
    }
    
    public static string FormatMetallicity(float metallicity)
    {
      return $"{metallicity:0.##}%";
    }
    
    public static List<string> FormatCompositionLines(Dictionary<string, float> elements, int elementsPerLine = 4)
    {
      if (elements == null || elements.Count == 0)
        return ["Astra_NameGenerator_Unknown".Translate()];
      
      List<string> parts = elements
        .OrderByDescending(kvp => kvp.Value)
        .Select(kvp => $"{kvp.Key} {kvp.Value:0.#}%")
        .ToList();
      
      List<string> lines = [];
      
      for (int i = 0; i < parts.Count; i += elementsPerLine)
        lines.Add(string.Join(", ", parts.Skip(i).Take(elementsPerLine)));
      
      return lines;
    }

    private static void NormalizeToPercent(Dictionary<string, float> elements)
    {
      float total = 0f;
      
      foreach (float value in elements.Values)
        total += value;
      
      if (total <= 0f)
        return;
      
      List<string> keys = new(elements.Keys);
      
      foreach (string key in keys)
        elements[key] = elements[key] / total * 100f;
    }
    
    private static float GetVariabilityAmount(SpectralClass spectralClass)
    {
      return spectralClass switch
      {
        SpectralClass.O => Rand.Range(0.05f, 0.15f),
        SpectralClass.B => Rand.Range(0.03f, 0.10f),
        SpectralClass.A => Rand.Range(0.02f, 0.08f),
        SpectralClass.F => Rand.Range(0.01f, 0.06f),
        SpectralClass.G => Rand.Range(0.01f, 0.05f),
        SpectralClass.K => Rand.Range(0.01f, 0.03f),
        SpectralClass.M => Rand.Range(0.01f, 0.02f),
        _ => Rand.Range(0.01f, 0.05f)
      };
    }
    
    public static float GenerateVariabilitySpeed()
    {
      return 0f;
    }
    
    public static GeneratedStellarVariability GenerateVariability(SpectralClass spectralClass)
    {
      // about 40% have no visible variability
      if (Rand.Range(0f, 1f) < 0.4f)
        return new GeneratedStellarVariability(StellarVariabilityType.None, 0f);
      
      StellarVariabilityType type = Rand.Range(0, 2) == 0
        ? StellarVariabilityType.Extrinsic
        : StellarVariabilityType.Intrinsic;
      
      float amount = GetVariabilityAmount(spectralClass);
      
      return new GeneratedStellarVariability(type, amount);
    }
    
    public static string FormatVariability(StellarVariabilityType type, float amount)
    {
      if (type == StellarVariabilityType.None || amount <= 0f)
        return "Astra_Stars_Variability_None".Translate();

      return $"{type} ({amount * 100f:0.#}%)";
    }
    
    public readonly struct GeneratedStellarVariability
    {
      public readonly StellarVariabilityType Type;
      public readonly float Amount;
      
      public bool HasVariability => Type != StellarVariabilityType.None && Amount > 0f;
      public bool IsIntrinsic => Type == StellarVariabilityType.Intrinsic && Amount > 0f;
      public bool IsExtrinsic => Type == StellarVariabilityType.Extrinsic && Amount > 0f;
      
      public GeneratedStellarVariability(StellarVariabilityType type, float amount)
      {
        Type = type;
        Amount = amount;
      }
    }
    
    public static float GenerateCoronaIntensity(float temperatureKelvin, float magneticField,
      StellarVariabilityType variabilityType,
      float variabilityAmount, double age, float luminosity)
    {
      float temperatureFactor = Mathf.InverseLerp(2400f, 50000f, temperatureKelvin);
      float magneticFactor = Mathf.InverseLerp(0.0001f, 1.0f, magneticField);
      float luminosityFactor = Mathf.Clamp01(Mathf.Log10(Mathf.Max(luminosity, 0.0001f)) / 6f + 0.5f);
      
      float variabilityFactor = variabilityType == StellarVariabilityType.Intrinsic
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
      
      return Mathf.Clamp(intensity, 0.1f, 1.5f);
    }
    
    public static Color GenerateCoronaColor(Color chromaticity, float temperatureKelvin, float magneticField,
      StellarVariabilityType variabilityType, float variabilityAmount,
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
      return 1.5f;
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
        >= 2f => $"{coronaIntensity:0.00} " + "Astra_Stars_CoronaIntensity_Intense".Translate(),
        >= 1.25f => $"{coronaIntensity:0.00} " + "Astra_Stars_CoronaIntensity_Active".Translate(),
        >= 0.75f => $"{coronaIntensity:0.00} " + "Astra_Stars_CoronaIntensity_Stable".Translate(),
        _ => $"{coronaIntensity:0.00} " + "Astra_Stars_CoronaIntensity_Weak".Translate()
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
    
    private static Color GetVariabilityCoronaTint(StellarVariabilityType variabilityType,
      float variabilityAmount)
    {
      if (variabilityType == StellarVariabilityType.None || variabilityAmount <= 0f)
        return Color.white;
      
      return variabilityType == StellarVariabilityType.Intrinsic
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
    
    private static Color GetChromaticity(SpectralClass spectralClass)
    {
      return spectralClass switch
      {
        SpectralClass.O => new Color(0.57f, 0.71f, 1f, 1f),
        SpectralClass.B => new Color(0.64f, 0.75f, 1f, 1f),
        SpectralClass.A => new Color(0.84f, 0.88f, 1f, 1f),
        SpectralClass.F => new Color(0.98f, 0.96f, 1f, 1f),
        SpectralClass.G => new Color(1f, 0.93f, 0.89f, 1f),
        SpectralClass.K => new Color(1f, 0.85f, 0.71f, 1f),
        SpectralClass.M => new Color(1f, 0.71f, 0.42f, 1f),
        _ => Color.white
      };
    }
    
    public static Color GenerateChromaticity(SpectralClass spectralClass)
    {
      return GetChromaticity(spectralClass);
    }
    
    public static float GenerateChromaticityIntensity()
    {
      return 2f;
    }
    
    public static float GenerateChromaticityFalloffPower()
    {
      return 5f;
    }
    
    public static float GenerateSurfaceNoiseStrength()
    {
      return 0.025f;
    }
    
    public static float GetChromaticityIntensitySpectralFactor(SpectralClass spectralClass)
    {
      return spectralClass switch
      {
        SpectralClass.O => 0.35f,
        SpectralClass.B => 0.3f,
        SpectralClass.A => 0.2f,
        SpectralClass.F => 0.15f,
        SpectralClass.G => 0.2f,
        SpectralClass.K => 0.72f,
        SpectralClass.M => 0.8f,
        _ => 1.00f
      };
    }
    
    public static string FormatChromaticity(Color chromaticity)
    {
      return
        "Astra_Stars_RedChannel".Translate() + $" {chromaticity.r:0.00}, " +
        "Astra_Stars_GreenChannel".Translate() + $" {chromaticity.g:0.00}, " +
        "Astra_Stars_BlueChannel".Translate() + $" {chromaticity.b:0.00}";
    }
    
    private static float GetRotation(SpectralClass spectralClass)
    {
      return spectralClass switch
      {
        SpectralClass.O => Rand.Range(20f, 30f),
        SpectralClass.B => Rand.Range(5f, 15f),
        SpectralClass.A => Rand.Range(1f, 10f),
        SpectralClass.F => Rand.Range(0.5f, 5f),
        SpectralClass.G => Rand.Range(0.1f, 1f),
        SpectralClass.K => Rand.Range(0.05f, 0.5f),
        SpectralClass.M => Rand.Range(0.01f, 0.05f),
        _ => Rand.Range(0.1f, 1f)
      };
    }
    
    public static float GenerateRotationsPerDay(SpectralClass spectralClass)
    {
      return GetRotation(spectralClass);
    }
    
    public static string FormatRotation(float rotationsPerDay)
    {
      return rotationsPerDay switch
      {
        >= 1f => $"{rotationsPerDay:0.##} " + "Astra_Stars_RotationsPerDay_One".Translate(),
        >= 0.01f => $"{1f / rotationsPerDay:0.#} " + "Astra_Stars_RotationsPerDay_Two".Translate(),
        _ => $"{1f / rotationsPerDay:0} " + "Astra_Stars_RotationsPerDay_Two".Translate()
      };
    }
  }
}