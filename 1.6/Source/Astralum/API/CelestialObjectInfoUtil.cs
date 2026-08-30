using Astralum.Astronomy;
using Astralum.Astronomy.BlackHoles;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.LocalStars;
using Astralum.Astronomy.Nebulae;
using Astralum.Astronomy.Pulsars;

namespace Astralum.API
{
  public static class CelestialObjectInfoUtil
  {
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
    
    public static CelestialObjectInfo FromLocalStar(SavedLocalStar localStar)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.LocalStar,
        localStar.Id,
        localStar.DisplayName,
        LocalStarOrbitUtil.PositionFor(localStar));
    }
    
    public static CelestialObjectInfo FromConstellation(SavedConstellation constellation)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.Constellation,
        constellation.Id,
        constellation.DisplayName,
        constellation.LocalSkyPosition);
    }

    public static CelestialObjectInfo FromConstellationStar(SavedConstellationStar star)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.ConstellationStar,
        star.Id,
        star.DisplayName,
        star.LocalSkyPosition);
    }
    
    public static CelestialObjectInfo FromBlackHole(SavedBlackHole blackHole)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.BlackHole,
        blackHole.Id,
        blackHole.DisplayName,
        blackHole.LocalSkyPosition);
    }
    
    public static CelestialObjectInfo FromPulsar(SavedPulsar pulsar)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.Pulsar,
        pulsar.Id,
        pulsar.DisplayName,
        pulsar.LocalSkyPosition);
    }
    
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