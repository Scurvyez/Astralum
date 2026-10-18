using UnityEngine;

namespace Astralum.API
{
  /// <summary>
  /// Represents the illumination, positional metrics, and visual properties of a single local star
  /// shining upon an observer's planet tile or map.
  /// </summary>
  public readonly struct LocalStarLightInfo
  {
    /// <summary>
    /// Descriptor containing metadata and orbital position information for the illuminating star.
    /// </summary>
    public readonly CelestialObjectInfo Star;
    
    /// <summary>
    /// The altitude angle of the star in degrees above the local horizon.
    /// </summary>
    public readonly float Altitude;
    
    /// <summary>
    /// The geometric incidence factor of light reaching the surface, calculated from the star's altitude angle.
    /// </summary>
    public readonly float Incidence;
    
    /// <summary>
    /// The unoccluded fraction of the star's apparent disk (0.0 to 1.0) after accounting for eclipses by foreground stars.
    /// </summary>
    public readonly float VisibleFraction;
    
    /// <summary>
    /// The net effective luminosity delivered to the surface after factoring in baseline luminosity, unoccluded disk fraction, and incidence.
    /// </summary>
    public readonly float EffectiveLuminosity;
    
    /// <summary>
    /// The characteristic emission or corona color of the star.
    /// </summary>
    public readonly Color Color;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStarLightInfo"/> struct.
    /// </summary>
    /// <param name="star">The celestial object descriptor for the star.</param>
    /// <param name="altitude">The altitude angle in degrees above the horizon.</param>
    /// <param name="incidence">The geometric light incidence factor.</param>
    /// <param name="visibleFraction">The fraction of the star disk unobstructed by eclipses (0.0 to 1.0).</param>
    /// <param name="effectiveLuminosity">The net illumination intensity reaching the observer.</param>
    /// <param name="color">The star's light and corona color.</param>
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