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
        constellation.name,
        constellation.name,
        constellation.centerDir.normalized * 20f
      );
    }
    
    public static CelestialObjectInfo FromBlackHole(SavedBlackHole blackHole)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.BlackHole,
        blackHole.id.ToString(),
        blackHole.generatedName,
        blackHole.localSkyPos
      );
    }
    
    public static CelestialObjectInfo FromPulsar(SavedPulsar pulsar)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.Pulsar,
        pulsar.pulsarId.ToString(),
        pulsar.name,
        pulsar.localSkyPos
      );
    }
    
    public static CelestialObjectInfo FromNebula(SavedNebula nebula)
    {
      return new CelestialObjectInfo(
        CelestialObjectType.Nebula,
        nebula.nebulaId.ToString(),
        nebula.name,
        nebula.localSkyPos
      );
    }
  }
}