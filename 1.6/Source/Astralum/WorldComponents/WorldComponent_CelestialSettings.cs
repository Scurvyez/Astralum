using Astralum.Astronomy;
using RimWorld.Planet;

namespace Astralum.WorldComponents
{
  public class WorldComponent_CelestialSettings : WorldComponent
  {
    public WorldComponent_CelestialSettings(World world) : base(world)
    {
      
    }
    
    public override void FinalizeInit(bool fromLoad)
    {
      base.FinalizeInit(fromLoad);
      
      CelestialDisplaySettings.Reset();
    }
  }
}