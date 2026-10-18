using Astralum.Astronomy.Constellations;
using Astralum.WorldComponents;
using Verse;

namespace Astralum.API
{
  /// <summary>
  /// Provides utility methods to determine the existence and availability of specific types of celestial objects
  /// generated in the current world session.
  /// </summary>
  public static class CelestialObjectAvailability
  {
    /// <summary>
    /// Determines whether any black holes exist in the current world data cache.
    /// </summary>
    /// <returns><c>true</c> if at least one black hole exists; otherwise, <c>false</c>.</returns>
    public static bool HasBlackHoles()
    {
      return Find.World.GetComponent<WorldComponent_CelestialObjectDataCache>()?.BlackHoles?.Count > 0;
    }
    
    /// <summary>
    /// Determines whether any constellations exist in the current world constellation cache.
    /// </summary>
    /// <returns><c>true</c> if at least one constellation is defined; otherwise, <c>false</c>.</returns>
    public static bool HasConstellations()
    {
      return Find.World.GetComponent<WorldComponent_ConstellationDataCache>()?.Constellations?.Count > 0;
    }
    
    /// <summary>
    /// Determines whether any constellation stars exist within the defined constellations in the current world.
    /// </summary>
    /// <returns><c>true</c> if at least one constellation contains one or more stars; otherwise, <c>false</c>.</returns>
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
    
    /// <summary>
    /// Determines whether any nebulae exist in the current world data cache.
    /// </summary>
    /// <returns><c>true</c> if at least one nebula exists; otherwise, <c>false</c>.</returns>
    public static bool HasNebulae()
    {
      return Find.World.GetComponent<WorldComponent_CelestialObjectDataCache>()?.Nebulae?.Count > 0;
    }
    
    /// <summary>
    /// Determines whether any pulsars exist in the current world data cache.
    /// </summary>
    /// <returns><c>true</c> if at least one pulsar exists; otherwise, <c>false</c>.</returns>
    public static bool HasPulsars()
    {
      return Find.World.GetComponent<WorldComponent_CelestialObjectDataCache>()?.Pulsars?.Count > 0;
    }

    /// <summary>
    /// Evaluates whether any celestial objects of the specified <see cref="CelestialObjectType"/> exist in the current world.
    /// </summary>
    /// <param name="type">The type of celestial object to query.</param>
    /// <returns><c>true</c> if at least one object matching the specified type exists; otherwise, <c>false</c>.</returns>
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