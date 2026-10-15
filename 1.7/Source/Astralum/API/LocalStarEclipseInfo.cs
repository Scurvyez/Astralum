using System.Collections.Generic;

namespace Astralum.API
{
  public readonly struct LocalStarEclipseInfo
  {
    // light that would be reaching this location if none of the local stars were obscured by another local star
    public readonly float UnobscuredLuminosity;
    
    // light actually reaching the location after occlusion
    public readonly float VisibleLuminosity;
    
    // 0 = no eclipse, 1 = all direct local-star light is blocked
    public readonly float EclipseStrength;
    
    // 1 = no light lost, 0 = all local-star light lost
    public readonly float RemainingLightFraction;
    
    // stars currently at least partially obscured
    public readonly IReadOnlyList<CelestialObjectInfo> EclipsedStars;
    
    public bool IsEclipse => EclipseStrength > 0.001f;
    public bool IsTotalEclipse => RemainingLightFraction <= 0.001f;
    public int EclipsedStarCount => EclipsedStars?.Count ?? 0;
    
    public LocalStarEclipseInfo(float unobscuredLuminosity, float visibleLuminosity, float eclipseStrength,
      float remainingLightFraction, IReadOnlyList<CelestialObjectInfo> eclipsedStars)
    {
      UnobscuredLuminosity = unobscuredLuminosity;
      VisibleLuminosity = visibleLuminosity;
      EclipseStrength = eclipseStrength;
      RemainingLightFraction = remainingLightFraction;
      EclipsedStars = eclipsedStars;
    }
  }
}