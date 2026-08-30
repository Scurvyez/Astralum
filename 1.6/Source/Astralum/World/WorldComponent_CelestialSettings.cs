using Astralum.Astronomy;
using RimWorld.Planet;

namespace Astralum.World
{
  public class WorldComponent_CelestialSettings : WorldComponent
  {
    public WorldComponent_CelestialSettings(RimWorld.Planet.World world) : base(world)
    {
      
    }
    
    public override void FinalizeInit(bool fromLoad)
    {
      base.FinalizeInit(fromLoad);
      
      CelestialDisplaySettings.Reset();
    }
  }
}