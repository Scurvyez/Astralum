using Astralum.Astronomy;
using Astralum.Astronomy.BlackHoles;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.Nebulae;
using Astralum.Astronomy.Pulsars;
using Astralum.Materials;

namespace Astralum.UI
{
  public static class CelestialNamingFocusVisualUtil
  {
    private static IPlayerNameableCelestialObject _focusedObject;

    public static void Focus(IPlayerNameableCelestialObject celestialObject)
    {
      Clear();
      
      _focusedObject = celestialObject;
      
      switch (celestialObject)
      {
        case SavedBlackHole blackHole: BlackHoleMatsUtil.SetFocused(blackHole, true);
          break;
        case SavedPulsar pulsar: PulsarMatsUtil.SetFocused(pulsar, true);
          break;
        case SavedNebula nebula: NebulaeMatsUtil.SetFocused(nebula, true);
          break;
        case SavedConstellation constellation: ConstellationsMatsUtil.SetFocused(constellation, true);
          break;
      }
    }
    
    public static void Clear()
    {
      switch (_focusedObject)
      {
        case SavedBlackHole blackHole: BlackHoleMatsUtil.SetFocused(blackHole, false);
          break;
        case SavedPulsar pulsar: PulsarMatsUtil.SetFocused(pulsar, false);
          break;
        case SavedNebula nebula: NebulaeMatsUtil.SetFocused(nebula, false);
          break;
        case SavedConstellation constellation: ConstellationsMatsUtil.SetFocused(constellation, false);
          break;
      }
      
      _focusedObject = null;
    }
  }
}