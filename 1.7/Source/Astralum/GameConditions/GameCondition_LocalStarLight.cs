using Astralum.API;
using Astralum.Settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace Astralum.GameConditions
{
  public class GameCondition_LocalStarLight : GameCondition
  {
    private const float MinimumGlow = 0.8f;
    
    public override float SkyTargetLerpFactor(Map map)
    {
      return 1f;
    }
    
    public override SkyTarget? SkyTarget(Map map)
    {
      if (map.GameConditionManager.IsAlwaysDarkOutside)
        return null;
      
      if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.Aurora) 
          || !CelestialLightGetter.TryGetLocalStarLight(map, out Color lightColor, out float intensity))
        return null;
      
      SkyColorSet colorSet = CreateSkyColorSet(lightColor);
      
      float mapGlow = AstraSettings.UseLocalStarLightForMapGlow 
        ? Mathf.Lerp(MinimumGlow, 1f, intensity) 
        : 1f;
      
      return new SkyTarget(mapGlow, colorSet, 1f, intensity);
    }
    
    private static SkyColorSet CreateSkyColorSet(Color lightColor)
    {
      Color sky = Color.Lerp(Color.white, lightColor, 0.50f);
      Color shadow = Color.Lerp(Color.white, lightColor, 0.25f);
      Color overlay = Color.Lerp(Color.white, lightColor, 0.65f);
      
      return new SkyColorSet(sky, shadow, overlay, 1f);
    }
  }
}