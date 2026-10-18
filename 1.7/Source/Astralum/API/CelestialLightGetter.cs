using System.Collections.Generic;
using Astralum.Astronomy.LocalStars;
using Astralum.WorldComponents;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.API
{
  /// <summary>
  /// Provides utility methods for calculating illumination, visibility, and mutual eclipse states
  /// of local stars on maps and planet tiles.
  /// </summary>
  public static class CelestialLightGetter
  {
    private const float ReferenceLuminosity = 1f;

    /// <summary>
    /// Calculates the combined lighting contributed by all visible local stars on the specified map.
    /// </summary>
    /// <param name="map">The map to calculate star lighting for.</param>
    /// <param name="lightColor">Outputs the gamma-corrected weighted average light color of all visible stars.</param>
    /// <param name="lightIntensity">Outputs the total normalized light intensity clamped between 0 and 1.</param>
    /// <returns><c>true</c> if star lighting was successfully computed (even if total intensity is zero); otherwise, <c>false</c> if no star data is available.</returns>
    public static bool TryGetLocalStarLight(Map map, out Color lightColor, out float lightIntensity)
    {
      lightColor = Color.white;
      lightIntensity = 0f;
      
      var data = Find.World?.GetComponent<WorldComponent_CelestialObjectDataCache>();
      
      if (data?.LocalStars.NullOrEmpty() != false)
        return false;
      
      Color weightedColor = Color.black;
      float totalWeight = 0f;
      
      List<SavedLocalStar> stars = data.LocalStars;
      
      for (int i = 0; i < stars.Count; i++)
      {
        if (!TryGetLightFromStar(stars[i], stars, map.Tile, out LocalStarLightInfo light))
        {
          continue;
        }
        
        Color starColor = light.Color.linear;
        weightedColor += starColor * light.EffectiveLuminosity;
        totalWeight += light.EffectiveLuminosity;
      }
      
      if (totalWeight <= 0f)
      {
        lightColor = Color.white;
        lightIntensity = 0f;
        return true;
      }
      
      weightedColor /= totalWeight;
      weightedColor.a = 1f;
      lightColor = weightedColor.gamma;
      lightIntensity = Mathf.Clamp01(totalWeight / ReferenceLuminosity);
      
      return true;
    }
    
    /// <summary>
    /// Attempts to calculate the lighting information for a single local star at a given planet tile,
    /// taking into account altitude above the horizon, incidence angle, and mutual occlusions.
    /// </summary>
    /// <param name="star">The local star to calculate light for.</param>
    /// <param name="stars">The list of all local stars in the system.</param>
    /// <param name="tile">The observer's planet tile.</param>
    /// <param name="result">Outputs the calculated <see cref="LocalStarLightInfo"/> if the star is illuminating the tile.</param>
    /// <returns><c>true</c> if the star is above the horizon and provides light; otherwise, <c>false</c>.</returns>
    private static bool TryGetLightFromStar(SavedLocalStar star, List<SavedLocalStar> stars, PlanetTile tile,
      out LocalStarLightInfo result)
    {
      result = default;
      
      if (star == null)
        return false;
      
      float altitude = CelestialObjectFinder.AltitudeFor(star, tile);
      
      if (altitude <= 0f)
        return false;

      float visibleFraction = VisibleFractionFor(star, stars);
      
      if (visibleFraction <= 0f)
        return false;
      
      float incidence = Mathf.Sin(altitude * Mathf.Deg2Rad);
      float effectiveLuminosity = Mathf.Max(0f, star.luminosity) * visibleFraction * incidence;
      
      if (effectiveLuminosity <= 0f)
        return false;
      
      CelestialObjectInfo starInfo = CelestialObjectInfoUtil.From(star);
      
      result = new LocalStarLightInfo(starInfo, altitude, incidence, visibleFraction, effectiveLuminosity, star.corona);
      
      return true;
    }
    
    /// <summary>
    /// Computes the unoccluded visible fraction (0.0 to 1.0) of a star's disk when considering
    /// occlusions by any other foreground local stars in the system.
    /// </summary>
    /// <param name="star">The target star whose visible fraction is being evaluated.</param>
    /// <param name="stars">The list of all local stars in the system.</param>
    /// <returns>A value between 0.0 (completely eclipsed) and 1.0 (completely unobscured).</returns>
    public static float VisibleFractionFor(SavedLocalStar star, List<SavedLocalStar> stars)
    {
      float visibleFraction = 1f;
      Vector3 starPosition = LocalStarOrbitUtil.PositionFor(star);
      float starAngularRadius = AngularRadiusFor(star);
      
      for (int i = 0; i < stars.Count; i++)
      {
        SavedLocalStar other = stars[i];
        
        if (other == null || ReferenceEquals(other, star))
          continue;
        
        // if LocalStarOrbitUtil ever exposes real observer-relative depth,
        // replace this with a physical distance comparison (this is never happening but like... yeah...)
        if (!IsInFrontOf(other, star))
          continue;
        
        Vector3 otherPosition = LocalStarOrbitUtil.PositionFor(other);
        float otherAngularRadius = AngularRadiusFor(other);
        float separation = AngularSeparation(starPosition, otherPosition);
        float coveredFraction = CoveredFraction(starAngularRadius, otherAngularRadius, separation);
        
        visibleFraction *= 1f - coveredFraction;
        
        if (visibleFraction <= 0.001f)
          return 0f;
      }
      
      return Mathf.Clamp01(visibleFraction);
    }
    
    /// <summary>
    /// Determines whether one star is in front of another along the line of sight when their projected disks overlap.
    /// </summary>
    /// <param name="candidateForeground">The candidate foreground star.</param>
    /// <param name="candidateBackground">The candidate background star.</param>
    /// <returns><c>true</c> if the candidate foreground star is in front of the background star; otherwise, <c>false</c>.</returns>
    private static bool IsInFrontOf(SavedLocalStar candidateForeground, SavedLocalStar candidateBackground)
    {
      // TEMPORARY:
      // lower systemIndex wins when disks overlap.
      //
      // Replace later with(?):
      //
      // return LocalStarOrbitUtil.DistanceFromPlanetFor(candidateForeground)
      //      < LocalStarOrbitUtil.DistanceFromPlanetFor(candidateBackground);
      
      return candidateForeground.systemIndex < candidateBackground.systemIndex;
    }
    
    /// <summary>
    /// Calculates the apparent angular radius (in radians) of a local star from the planet's vantage point.
    /// </summary>
    /// <param name="star">The local star.</param>
    /// <returns>The angular radius in radians.</returns>
    private static float AngularRadiusFor(SavedLocalStar star)
    {
      Vector3 position = LocalStarOrbitUtil.PositionFor(star);
      float distance = position.magnitude;
      
      if (distance <= 0.0001f)
        return 0f;
      
      float radius = LocalStarRenderUtil.PhotosphereRadiusFor(star);
      
      return Mathf.Atan(radius / distance);
    }
    
    /// <summary>
    /// Calculates the angular separation (in radians) between two direction/position vectors from the planet.
    /// </summary>
    /// <param name="a">The first direction vector.</param>
    /// <param name="b">The second direction vector.</param>
    /// <returns>The angle between the vectors in radians.</returns>
    private static float AngularSeparation(Vector3 a, Vector3 b)
    {
      float dot = Mathf.Clamp(Vector3.Dot(a.normalized, b.normalized), -1f, 1f);
      
      return Mathf.Acos(dot);
    }
    
    /// <summary>
    /// Calculates the fraction (0.0 to 1.0) of a circular background disk covered by an overlapping foreground disk.
    /// </summary>
    /// <param name="backgroundRadius">The angular radius of the background disk in radians.</param>
    /// <param name="foregroundRadius">The angular radius of the foreground disk in radians.</param>
    /// <param name="separation">The angular separation between the centers of the two disks in radians.</param>
    /// <returns>The covered fraction between 0.0 and 1.0.</returns>
    private static float CoveredFraction(float backgroundRadius, float foregroundRadius, float separation)
    {
      if (backgroundRadius <= 0f)
        return 0f;
      
      if (separation >= backgroundRadius + foregroundRadius)
        return 0f;
      
      // foreground disk completely covers background disk
      if (foregroundRadius >= backgroundRadius + separation)
        return 1f;
      
      // foreground disk is entirely inside background disk
      if (backgroundRadius >= foregroundRadius + separation)
      {
        float foregroundArea = Mathf.PI * foregroundRadius * foregroundRadius;
        float backgroundArea = Mathf.PI * backgroundRadius * backgroundRadius;
        
        return Mathf.Clamp01(foregroundArea / backgroundArea);
      }
      
      float r1 = backgroundRadius;
      float r2 = foregroundRadius;
      float d = separation;
      
      float r1Squared = r1 * r1;
      float r2Squared = r2 * r2;
      
      float angle1 = Mathf.Acos(Mathf.Clamp((d * d + r1Squared - r2Squared) / (2f * d * r1), -1f, 1f));
      float angle2 = Mathf.Acos(Mathf.Clamp((d * d + r2Squared - r1Squared) / (2f * d * r2), -1f, 1f));
      
      float overlapArea = r1Squared * angle1 + r2Squared * angle2 - 0.5f 
        * Mathf.Sqrt(Mathf.Max(0f, (-d + r1 + r2) * (d + r1 - r2) * (d - r1 + r2) * (d + r1 + r2)));
      
      float finalBackgroundArea = Mathf.PI * r1Squared;
      
      return Mathf.Clamp01(overlapArea / finalBackgroundArea);
    }
    
    /// <summary>
    /// Gets a list of lighting information for all local stars currently shining on the specified map.
    /// </summary>
    /// <param name="map">The map to check.</param>
    /// <returns>A list of <see cref="LocalStarLightInfo"/> for each illuminating star.</returns>
    public static List<LocalStarLightInfo> GetLocalStarsShiningOn(Map map)
    {
      return GetLocalStarsShiningOn(map.Tile);
    }
    
    /// <summary>
    /// Gets a list of lighting information for all local stars currently shining on the specified planet tile.
    /// </summary>
    /// <param name="tile">The planet tile to check.</param>
    /// <returns>A list of <see cref="LocalStarLightInfo"/> for each illuminating star.</returns>
    public static List<LocalStarLightInfo> GetLocalStarsShiningOn(PlanetTile tile)
    {
      List<LocalStarLightInfo> results = [];
      GetLocalStarsShiningOn(tile, results);
      
      return results;
    }
    
    /// <summary>
    /// Populates a list with lighting information for all local stars currently shining on the specified map.
    /// </summary>
    /// <param name="map">The map to check.</param>
    /// <param name="results">The list to populate with <see cref="LocalStarLightInfo"/> items. Existing contents will be cleared.</param>
    public static void GetLocalStarsShiningOn(Map map, List<LocalStarLightInfo> results)
    {
      GetLocalStarsShiningOn(map.Tile, results);
    }
    
    /// <summary>
    /// Populates a list with lighting information for all local stars currently shining on the specified planet tile.
    /// </summary>
    /// <param name="tile">The planet tile to check.</param>
    /// <param name="results">The list to populate with <see cref="LocalStarLightInfo"/> items. Existing contents will be cleared.</param>
    public static void GetLocalStarsShiningOn(PlanetTile tile, List<LocalStarLightInfo> results)
    {
      results.Clear();
      
      var data = Find.World?.GetComponent<WorldComponent_CelestialObjectDataCache>();
      
      if (data?.LocalStars.NullOrEmpty() != false)
        return;
      
      List<SavedLocalStar> stars = data.LocalStars;
      
      for (int i = 0; i < stars.Count; i++)
      {
        if (TryGetLightFromStar(stars[i], stars, tile, out LocalStarLightInfo result))
        {
          results.Add(result);
        }
      }
    }
    
    /// <summary>
    /// Convenience alias for <see cref="TryGetLocalStarEclipse(Map, out LocalStarEclipseInfo)"/>.
    /// </summary>
    /// <param name="map">The map to check for an eclipse.</param>
    /// <param name="eclipse">Outputs eclipse information if an eclipse is currently occurring.</param>
    /// <returns><c>true</c> if a local star eclipse is currently active on the map; otherwise, <c>false</c>.</returns>
    public static bool TryGetEclipse(Map map, out LocalStarEclipseInfo eclipse)
    {
      return TryGetLocalStarEclipse(map, out eclipse);
    }
    
    /// <summary>
    /// Checks whether any local star eclipse is occurring on the specified map and calculates detailed eclipse metrics.
    /// </summary>
    /// <param name="map">The map to check for an eclipse.</param>
    /// <param name="result">Outputs detailed eclipse metrics in a <see cref="LocalStarEclipseInfo"/> structure.</param>
    /// <returns><c>true</c> if a local star eclipse is active; otherwise, <c>false</c>.</returns>
    public static bool TryGetLocalStarEclipse(Map map, out LocalStarEclipseInfo result)
    {
      return TryGetLocalStarEclipse(map.Tile, out result);
    }
    
    /// <summary>
    /// Checks whether any local star eclipse is occurring at the specified planet tile and calculates detailed eclipse metrics.
    /// </summary>
    /// <param name="tile">The planet tile to check for an eclipse.</param>
    /// <param name="result">Outputs detailed eclipse metrics in a <see cref="LocalStarEclipseInfo"/> structure.</param>
    /// <returns><c>true</c> if a local star eclipse is active; otherwise, <c>false</c>.</returns>
    public static bool TryGetLocalStarEclipse(PlanetTile tile, out LocalStarEclipseInfo result)
    {
      result = default;
      
      var data = Find.World?.GetComponent<WorldComponent_CelestialObjectDataCache>();
      
      if (data?.LocalStars.NullOrEmpty() != false)
        return false;
      
      List<SavedLocalStar> stars = data.LocalStars;
      
      if (stars.Count < 2)
        return false;
      
      float unobscuredLuminosity = 0f;
      float visibleLuminosity = 0f;
      List<CelestialObjectInfo> eclipsedStars = [];
      
      for (int i = 0; i < stars.Count; i++)
      {
        SavedLocalStar star = stars[i];
        
        if (star == null)
          continue;
        
        float altitude = CelestialObjectFinder.AltitudeFor(star, tile);
        
        if (altitude <= 0f)
          continue;
        
        float incidence = Mathf.Sin(altitude * Mathf.Deg2Rad);
        
        if (incidence <= 0f)
          continue;
        
        float luminosity = Mathf.Max(0f, star.luminosity);
        float unobscuredContribution = luminosity * incidence;
        unobscuredLuminosity += unobscuredContribution;
        
        float visibleFraction = VisibleFractionFor(star, stars);
        
        if (visibleFraction < 0.999f)
          eclipsedStars.Add(CelestialObjectInfoUtil.From(star));
        
        float visibleContribution = unobscuredContribution * visibleFraction;
        visibleLuminosity += visibleContribution;
      }
      
      if (unobscuredLuminosity <= 0f)
        return false;
      
      float remainingLightFraction = Mathf.Clamp01(visibleLuminosity / unobscuredLuminosity);
      float eclipseStrength = 1f - remainingLightFraction;
      
      result = new LocalStarEclipseInfo(unobscuredLuminosity, visibleLuminosity, eclipseStrength,
          remainingLightFraction, eclipsedStars);
      
      return result.IsEclipse;
    }
  }
}