using Astralum.Astronomy.Constellations;
using Astralum.World;
using Verse;

namespace Astralum.API
{
  public static class CelestialObjectAvailability
  {
    public static bool HasBlackHoles()
    {
      return Find.World.GetComponent<WorldComponent_CelestialObjectDataCache>()?.BlackHoles?.Count > 0;
    }
    
    public static bool HasConstellations()
    {
      return Find.World.GetComponent<WorldComponent_ConstellationDataCache>()?.Constellations?.Count > 0;
    }
    
    public static bool HasConstellationStars()
    {
      WorldComponent_ConstellationDataCache dataCache = Find.World.GetComponent<WorldComponent_ConstellationDataCache>();
      
      if (dataCache?.Constellations == null) return false;
      
      foreach (SavedConstellation constellation in dataCache.Constellations)
      {
        if (constellation == null) return false;
        if (constellation.stars.Count > 0) return true;
      }
      return false;
    }
    
    public static bool HasNebulae()
    {
      return Find.World.GetComponent<WorldComponent_CelestialObjectDataCache>()?.Nebulas?.Count > 0;
    }
    
    public static bool HasPulsars()
    {
      return Find.World.GetComponent<WorldComponent_CelestialObjectDataCache>()?.Pulsars?.Count > 0;
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