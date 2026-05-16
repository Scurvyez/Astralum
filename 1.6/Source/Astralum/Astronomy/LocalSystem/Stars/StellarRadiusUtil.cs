using UnityEngine;
using Verse;

namespace Astralum.Astronomy.LocalSystem.Stars
{
  public static class StellarRadiusUtil
  {
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
  }
}