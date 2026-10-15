using Astralum.API;
using RimWorld;
using Verse;

namespace Astralum.GameConditions
{
  public class GameCondition_LocalStarEclipse : GameCondition
  {
    public override float SkyGazeChanceFactor(Map map)
    {
      if (!CelestialLightGetter.TryGetEclipse(map, out LocalStarEclipseInfo eclipse))
        return 1f;
      
      return 1f + eclipse.EclipseStrength * 0.5f;
    }

    public override float SkyGazeJoyGainFactor(Map map)
    {
      if (!CelestialLightGetter.TryGetEclipse(map, out LocalStarEclipseInfo eclipse))
        return 1f;
      
      return 1f + eclipse.EclipseStrength * 0.25f;
    }

    public override float TemperatureOffset()
    {
      var map = SingleMap;
      
      if (map == null || !CelestialLightGetter.TryGetEclipse(map, out LocalStarEclipseInfo eclipse))
        return 0f;
      
      return -5f * eclipse.EclipseStrength;
    }
    
    public override bool AllowEnjoyableOutsideNow(Map map) => true;
  }
}