using System.Collections.Generic;

namespace Astralum.API
{
  /// <summary>
  /// Represents the state of a solar or stellar eclipse among local stars,
  /// including light attenuation metrics and affected celestial objects.
  /// </summary>
  public readonly struct LocalStarEclipseInfo
  {
    /// <summary>
    /// The total baseline luminosity of all stars above the horizon without occlusion.
    /// </summary>
    public readonly float UnobscuredLuminosity;
    
    /// <summary>
    /// The actual remaining luminosity reaching the observer after accounting for occlusions.
    /// </summary>
    public readonly float VisibleLuminosity;
    
    /// <summary>
    /// The normalized strength of the eclipse ranging from 0.0 (no occlusion) to 1.0 (complete darkness).
    /// </summary>
    public readonly float EclipseStrength;
    
    /// <summary>
    /// The fraction of baseline light still visible ranging from 1.0 (unobscured) to 0.0 (total eclipse).
    /// </summary>
    public readonly float RemainingLightFraction;
    
    /// <summary>
    /// The collection of celestial stars currently experiencing partial or total occlusion.
    /// </summary>
    public readonly IReadOnlyList<CelestialObjectInfo> EclipsedStars;
    
    /// <summary>
    /// Indicates whether an eclipse is actively occurring (eclipse strength exceeds threshold).
    /// </summary>
    public bool IsEclipse => EclipseStrength > 0.001f;

    /// <summary>
    /// Indicates whether all illuminating stars are completely occluded.
    /// </summary>
    public bool IsTotalEclipse => RemainingLightFraction <= 0.001f;

    /// <summary>
    /// The number of distinct stars currently occluded.
    /// </summary>
    public int EclipsedStarCount => EclipsedStars?.Count ?? 0;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStarEclipseInfo"/> struct.
    /// </summary>
    /// <param name="unobscuredLuminosity">Total baseline luminosity prior to occlusion.</param>
    /// <param name="visibleLuminosity">Effective luminosity reaching the observer.</param>
    /// <param name="eclipseStrength">Normalized magnitude of the eclipse (0.0 to 1.0).</param>
    /// <param name="remainingLightFraction">Fraction of initial light preserved (0.0 to 1.0).</param>
    /// <param name="eclipsedStars">List of celestial objects undergoing eclipse.</param>
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