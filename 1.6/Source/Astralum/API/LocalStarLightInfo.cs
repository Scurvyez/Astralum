using UnityEngine;

namespace Astralum.API
{
  public readonly struct LocalStarLightInfo
  {
    public readonly CelestialObjectInfo Star;
    
    // degrees above the local horizon. 0 = horizon, 90 = directly overhead
    public readonly float Altitude;
    
    // 0 = grazing/horizon, 1 = directly overhead
    public readonly float Incidence;
    
    // accounts for another local star obscuring this one
    public readonly float VisibleFraction;
    
    // raw contribution before the combined-light compression used by TryGetLocalStarLight()
    public readonly float EffectiveLuminosity;
    
    public readonly Color Color;
    
    public LocalStarLightInfo(CelestialObjectInfo star, float altitude, float incidence, float visibleFraction,
      float effectiveLuminosity, Color color)
    {
      Star = star;
      Altitude = altitude;
      Incidence = incidence;
      VisibleFraction = visibleFraction;
      EffectiveLuminosity = effectiveLuminosity;
      Color = color;
    }
  }
}