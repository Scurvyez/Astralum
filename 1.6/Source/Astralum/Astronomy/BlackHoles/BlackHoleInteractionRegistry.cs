using System.Collections.Generic;
using UnityEngine;

namespace Astralum.Astronomy.BlackHoles
{
  public static class BlackHoleInteractionRegistry
  {
    private static readonly List<HoverBlackHole> BlackHoles = [];
    
    public static IReadOnlyList<HoverBlackHole> HoverBlackHoles => BlackHoles;
    
    public static void Clear()
    {
      BlackHoles.Clear();
    }
    
    public static void Register(string name, Vector3 localSkyPos, float size, string hemisphere,
      string rightAscension, string declination)
    {
      BlackHoles.Add(new HoverBlackHole(name, localSkyPos, size, hemisphere, rightAscension, declination));
    }
    
    public readonly struct HoverBlackHole
    {
      public readonly string name;
      public readonly Vector3 localSkyPos;
      public readonly float size;
      public readonly string hemisphere;
      public readonly string rightAscension;
      public readonly string declination;
      
      public HoverBlackHole(string name, Vector3 localSkyPos, float size, string hemisphere,
        string rightAscension, string declination)
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