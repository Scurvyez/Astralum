using Astralum.API;
using RimWorld;
using UnityEngine;
using Verse;

namespace Astralum.GameConditions
{
  public class GameCondition_LocalStarLight : GameCondition
  {
    public bool enabled = true;
    
    public override float SkyTargetLerpFactor(Map map)
    {
      return enabled ? 1f : 0f;
    }

    public override SkyTarget? SkyTarget(Map map)
    {
      if (!enabled)
        return null;

      if (!CelestialLightGetter.TryGetLocalStarLight(map, out Color lightColor, out float intensity))
        return null;

      SkyColorSet colorSet = CreateSkyColorSet(lightColor, intensity);
      
      return new SkyTarget(intensity, colorSet, 1f, intensity);
    }
    
    private static SkyColorSet CreateSkyColorSet(Color lightColor, float intensity)
    {
      Color sky = Color.Lerp(Color.white, lightColor, 0.50f);
      Color shadow = Color.Lerp(Color.white, lightColor, 0.25f);
      Color overlay = Color.Lerp(Color.white, lightColor, 0.65f);
      
      return new SkyColorSet(sky, shadow, overlay, 1f);
    }
    
    public override void ExposeData()
    {
      base.ExposeData();
      
      Scribe_Values.Look(ref enabled, "enabled", true);
    }
  }
}