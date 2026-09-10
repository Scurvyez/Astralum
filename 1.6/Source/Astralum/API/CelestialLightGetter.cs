using System.Collections.Generic;
using Astralum.Astronomy.LocalStars;
using Astralum.WorldComponents;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.API
{
  public static class CelestialLightGetter
  {
    private const float ReferenceLuminosity = 1f;

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
        return false;
      
      weightedColor /= totalWeight;
      weightedColor.a = 1f;
      lightColor = weightedColor.gamma;
      
      float normalizedLuminosity = totalWeight / ReferenceLuminosity;
      lightIntensity = 1f - Mathf.Exp(-normalizedLuminosity);
      
      return true;
    }
    
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
    
    private static float AngularRadiusFor(SavedLocalStar star)
    {
      Vector3 position = LocalStarOrbitUtil.PositionFor(star);
      float distance = position.magnitude;
      
      if (distance <= 0.0001f)
        return 0f;
      
      float radius = star.RenderSize * 0.5f;
      
      return Mathf.Atan(radius / distance);
    }
    
    private static float AngularSeparation(Vector3 a, Vector3 b)
    {
      float dot = Mathf.Clamp(Vector3.Dot(a.normalized, b.normalized), -1f, 1f);
      
      return Mathf.Acos(dot);
    }
    
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
    
    public static List<LocalStarLightInfo> GetLocalStarsShiningOn(Map map)
    {
      return GetLocalStarsShiningOn(map.Tile);
    }
    
    public static List<LocalStarLightInfo> GetLocalStarsShiningOn(PlanetTile tile)
    {
      List<LocalStarLightInfo> results = [];
      GetLocalStarsShiningOn(tile, results);
      
      return results;
    }
    
    public static void GetLocalStarsShiningOn(Map map, List<LocalStarLightInfo> results)
    {
      GetLocalStarsShiningOn(map.Tile, results);
    }
    
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
  }
}