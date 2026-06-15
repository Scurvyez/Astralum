using Astralum.World;
using RimWorld;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public static class ConstellationObservationUtil
  {
    private const float ImmediateViewDot = 0.35f;
    
    public static SavedConstellation BestObservableConstellationFor(Pawn pawn)
    {
      WorldComponent_ConstellationData data = ConstellationDataUtil.Data;
      
      if (data?.constellations.NullOrEmpty() != false)
        return null;
      
      Vector3 observerDir = ObserverWorldDirection(pawn);
      Quaternion skyRotation = CurrentSkyRotation();
      
      SavedConstellation bestVisible = null;
      float bestVisibleScore = float.MinValue;
      
      SavedConstellation bestFallback = null;
      float bestFallbackScore = float.MinValue;
      
      foreach (SavedConstellation constellation in data.constellations)
      {
        Vector3 constellationWorldDir = skyRotation * constellation.centerDir.normalized;
        float score = Vector3.Dot(observerDir, constellationWorldDir);
        
        if (score > bestFallbackScore)
        {
          bestFallbackScore = score;
          bestFallback = constellation;
        }
        
        if (score < ImmediateViewDot)
          continue;
        
        if (score > bestVisibleScore)
        {
          bestVisibleScore = score;
          bestVisible = constellation;
        }
      }
      
      return bestVisible ?? bestFallback;
    }
    
    private static Vector3 ObserverWorldDirection(Pawn pawn)
    {
      if (pawn?.Map != null)
        return Find.WorldGrid.GetTileCenter(pawn.Map.Tile).normalized;
      
      if (Find.CurrentMap != null)
        return Find.WorldGrid.GetTileCenter(Find.CurrentMap.Tile).normalized;

      return Vector3.up;
    }
    
    public static Quaternion CurrentSkyRotation()
    {
      if (Current.ProgramState == ProgramState.Entry)
        return Quaternion.identity;
      
      return Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
    }
  }
}