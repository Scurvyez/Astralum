using Astralum.Astronomy.Constellations;
using Astralum.World;
using Verse;

namespace Astralum.API
{
  public static class CelestialObjectAvailability
  {
    public static bool HasBlackHoles()
    {
      return Find.World.GetComponent<WorldComponent_BlackHoleData>()?.BlackHoles?.Count > 0;
    }
    
    public static bool HasConstellations()
    {
      return Find.World.GetComponent<WorldComponent_ConstellationData>()?.Constellations?.Count > 0;
    }
    
    public static bool HasConstellationStars()
    {
      WorldComponent_ConstellationData data = Find.World.GetComponent<WorldComponent_ConstellationData>();
      
      if (data?.Constellations == null) return false;
      
      foreach (SavedConstellation constellation in data.Constellations)
      {
        if (constellation == null) return false;
        if (constellation.stars.Count > 0) return true;
      }
      return false;
    }
    
    public static bool HasNebulae()
    {
      return Find.World.GetComponent<WorldComponent_NebulaeData>()?.Nebulae?.Count > 0;
    }
    
    public static bool HasPulsars()
    {
      return Find.World.GetComponent<WorldComponent_PulsarData>()?.Pulsars?.Count > 0;
    }

    public static bool HasAny(CelestialObjectType type)
    {
      return type switch
      {
        CelestialObjectType.BlackHole => HasBlackHoles(),
        CelestialObjectType.Constellation => HasConstellations(),
        CelestialObjectType.ConstellationStar => HasConstellationStars(),
        CelestialObjectType.Nebulae => HasNebulae(),
        CelestialObjectType.Pulsar => HasPulsars(),
        _ => false
      };
    }
  }
}