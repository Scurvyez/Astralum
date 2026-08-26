using Astralum.Astronomy.BlackHoles;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.Nebulae;
using Astralum.Astronomy.Pulsars;

namespace Astralum.API
{
  public static class CelestialObjectInfoUtil
  {
    public static CelestialObjectInfo FromConstellation(SavedConstellation constellation)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.Constellation,
        constellation.Id,
        constellation.DisplayName,
        constellation.centerDir.normalized * 20f);
    }

    public static CelestialObjectInfo FromConstellationStar(SavedConstellation constellation,
      SavedConstellationStar star)
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