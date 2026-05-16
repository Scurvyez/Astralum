using System.Collections.Generic;
using Astralum.Astronomy.LocalSystem.Stars;
using UnityEngine;

namespace Astralum.Astronomy.Constellations
{
  public static class ConstellationInteractionRegistry
  {
    private static readonly List<HoverStar> Stars = [];

    public static IReadOnlyList<HoverStar> HoverStars => Stars;

    public static void Clear()
    {
      Stars.Clear();
    }

    public static void RegisterStar(string name, Vector3 localSkyPos, SpectralClass spectralClass, float radius,
      string constellationName, string hemisphere, string rightAscension, string declination)
    {
      Stars.Add(new HoverStar(name, localSkyPos, spectralClass, radius, constellationName, hemisphere,
        rightAscension, declination));
    }

    public readonly struct HoverStar
    {
      public readonly string name;
      public readonly Vector3 localSkyPos;
      public readonly SpectralClass spectralClass;
      public readonly float radius;
      public readonly string constellationName;
      public readonly string hemisphere;
      public readonly string rightAscension;
      public readonly string declination;

      public HoverStar(string name, Vector3 localSkyPos, SpectralClass spectralClass, float radius,
        string constellationName, string hemisphere, string rightAscension, string declination)
      {
        this.name = name;
        this.localSkyPos = localSkyPos;
        this.spectralClass = spectralClass;
        this.radius = radius;
        this.constellationName = constellationName;
        this.hemisphere = hemisphere;
        this.rightAscension = rightAscension;
        this.declination = declination;
      }
    }
  }
}