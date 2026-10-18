using Astralum.Astronomy;
using Astralum.Astronomy.BlackHoles;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.LocalStars;
using Astralum.Astronomy.Nebulae;
using Astralum.Astronomy.Pulsars;

namespace Astralum.API
{
  /// <summary>
  /// Provides utility methods for converting polymorphic <see cref="ICelestialObject"/> instances
  /// and concrete celestial domain models into normalized <see cref="CelestialObjectInfo"/> descriptors.
  /// </summary>
  public static class CelestialObjectInfoUtil
  {
    /// <summary>
    /// Converts a generic <see cref="ICelestialObject"/> into its corresponding <see cref="CelestialObjectInfo"/> representation.
    /// Dynamic coordinates are resolved where applicable (such as orbiting local stars).
    /// </summary>
    /// <param name="celestialObject">The celestial object instance to convert.</param>
    /// <returns>A populated <see cref="CelestialObjectInfo"/> struct matching the object's type, or an unknown info descriptor if the object is unhandled.</returns>
    public static CelestialObjectInfo From(ICelestialObject celestialObject)
    {
      return celestialObject switch
      {
        SavedLocalStar localStar => FromLocalStar(localStar),
        SavedConstellation constellation => FromConstellation(constellation),
        SavedConstellationStar constellationStar => FromConstellationStar(constellationStar),
        SavedBlackHole blackHole => FromBlackHole(blackHole),
        SavedPulsar pulsar => FromPulsar(pulsar),
        SavedNebula nebula => FromNebula(nebula),
        _ => new CelestialObjectInfo(CelestialObjectType.Unknown, null, null, default)
      };
    }
    
    /// <summary>
    /// Creates a <see cref="CelestialObjectInfo"/> descriptor from a <see cref="SavedLocalStar"/>,
    /// calculating its dynamic orbital position in the local sky.
    /// </summary>
    /// <param name="localStar">The saved local star model.</param>
    /// <returns>A <see cref="CelestialObjectInfo"/> containing the star's metadata and evaluated sky position.</returns>
    public static CelestialObjectInfo FromLocalStar(SavedLocalStar localStar)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.LocalStar,
        localStar.Id,
        localStar.DisplayName,
        LocalStarOrbitUtil.PositionFor(localStar));
    }
    
    /// <summary>
    /// Creates a <see cref="CelestialObjectInfo"/> descriptor from a <see cref="SavedConstellation"/>.
    /// </summary>
    /// <param name="constellation">The saved constellation model.</param>
    /// <returns>A <see cref="CelestialObjectInfo"/> containing the constellation's metadata and local sky position.</returns>
    public static CelestialObjectInfo FromConstellation(SavedConstellation constellation)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.Constellation,
        constellation.Id,
        constellation.DisplayName,
        constellation.LocalSkyPosition);
    }

    /// <summary>
    /// Creates a <see cref="CelestialObjectInfo"/> descriptor from a <see cref="SavedConstellationStar"/>.
    /// </summary>
    /// <param name="star">The saved constellation star model.</param>
    /// <returns>A <see cref="CelestialObjectInfo"/> containing the star's metadata and local sky position.</returns>
    public static CelestialObjectInfo FromConstellationStar(SavedConstellationStar star)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.ConstellationStar,
        star.Id,
        star.DisplayName,
        star.LocalSkyPosition);
    }
    
    /// <summary>
    /// Creates a <see cref="CelestialObjectInfo"/> descriptor from a <see cref="SavedBlackHole"/>.
    /// </summary>
    /// <param name="blackHole">The saved black hole model.</param>
    /// <returns>A <see cref="CelestialObjectInfo"/> containing the black hole's metadata and local sky position.</returns>
    public static CelestialObjectInfo FromBlackHole(SavedBlackHole blackHole)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.BlackHole,
        blackHole.Id,
        blackHole.DisplayName,
        blackHole.LocalSkyPosition);
    }
    
    /// <summary>
    /// Creates a <see cref="CelestialObjectInfo"/> descriptor from a <see cref="SavedPulsar"/>.
    /// </summary>
    /// <param name="pulsar">The saved pulsar model.</param>
    /// <returns>A <see cref="CelestialObjectInfo"/> containing the pulsar's metadata and local sky position.</returns>
    public static CelestialObjectInfo FromPulsar(SavedPulsar pulsar)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.Pulsar,
        pulsar.Id,
        pulsar.DisplayName,
        pulsar.LocalSkyPosition);
    }
    
    /// <summary>
    /// Creates a <see cref="CelestialObjectInfo"/> descriptor from a <see cref="SavedNebula"/>.
    /// </summary>
    /// <param name="nebula">The saved nebula model.</param>
    /// <returns>A <see cref="CelestialObjectInfo"/> containing the nebula's metadata and local sky position.</returns>
    public static CelestialObjectInfo FromNebula(SavedNebula nebula)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.Nebulae,
        nebula.Id,
        nebula.DisplayName,
        nebula.LocalSkyPosition);
    }
  }
}