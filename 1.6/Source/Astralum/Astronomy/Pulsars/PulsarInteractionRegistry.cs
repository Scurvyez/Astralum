using System.Collections.Generic;
using UnityEngine;

namespace Astralum.Astronomy.Pulsars
{
  public static class PulsarInteractionRegistry
  {
    private static readonly List<HoverPulsar> Pulsars = [];
    
    public static IReadOnlyList<HoverPulsar> HoverPulsars => Pulsars;
    
    public static void Clear()
    {
      Pulsars.Clear();
    }
    
    public static void Register(string name, Vector3 localSkyPos, float size, string hemisphere, string rightAscension,
      string declination)
    {
      Pulsars.Add(new HoverPulsar(name, localSkyPos, size, hemisphere, rightAscension, declination));
    }
    
    public readonly struct HoverPulsar
    {
      public readonly string name;
      public readonly Vector3 localSkyPos;
      public readonly float size;
      public readonly string hemisphere;
      public readonly string rightAscension;
      public readonly string declination;

      public HoverPulsar(string name, Vector3 localSkyPos, float size, string hemisphere, string rightAscension, 
        string declination)
      {
        this.name = name;
        this.localSkyPos = localSkyPos;
        this.size = size;
        this.hemisphere = hemisphere;
        this.rightAscension = rightAscension;
        this.declination = declination;
      }
    }
  }
}