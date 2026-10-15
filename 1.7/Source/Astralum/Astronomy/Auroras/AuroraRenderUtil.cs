using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Auroras
{
  public static class AuroraRenderUtil
  {
    private static readonly AccessTools.FieldRef<GameCondition_Aurora, int> CurColorIndex =
      AccessTools.FieldRefAccess<GameCondition_Aurora, int>("curColorIndex");
    
    private static readonly AccessTools.FieldRef<GameCondition_Aurora, int> PrevColorIndex =
      AccessTools.FieldRefAccess<GameCondition_Aurora, int>("prevColorIndex");
    
    private static readonly AccessTools.FieldRef<GameCondition_Aurora, float> CurColorTransition =
      AccessTools.FieldRefAccess<GameCondition_Aurora, float>("curColorTransition");
    
    private static readonly Color[] AuroraColors =
    [
      new(0f, 1f, 0f),
      new(0.3f, 1f, 0f),
      new(0f, 1f, 0.7f),
      new(0.3f, 1f, 0.7f),
      new(0f, 0.5f, 1f),
      new(0f, 0f, 1f),
      new(0.87f, 0f, 1f),
      new(0.75f, 0f, 1f)
    ];
    
    public static bool TryGetActiveAurora(out GameCondition_Aurora aurora)
    {
      aurora = null;
      
      List<Map> maps = Find.Maps;
      
      if (maps == null)
        return false;
      
      for (int i = 0; i < maps.Count; i++)
      {
        Map map = maps[i];

        var condition = map?.gameConditionManager.GetActiveCondition(GameConditionDefOf.Aurora);
        
        if (condition is not GameCondition_Aurora activeAurora)
          continue;
        
        aurora = activeAurora;
        return true;
      }
      
      return false;
    }
    
    public static void GetGradientColors(GameCondition_Aurora aurora, out Color bottom, out Color middle, out Color top)
    {
      int currentIndex = CurColorIndex(aurora);
      int previousIndex = PrevColorIndex(aurora);
      float transition = CurColorTransition(aurora);
      
      bottom = aurora.CurrentColor;
      
      int middleFromIndex = (previousIndex - 1 + AuroraColors.Length) % AuroraColors.Length;
      int middleToIndex = (currentIndex - 1 + AuroraColors.Length) % AuroraColors.Length;
      int topFromIndex = (previousIndex - 2 + AuroraColors.Length) % AuroraColors.Length;
      int topToIndex = (currentIndex - 2 + AuroraColors.Length) % AuroraColors.Length;
      
      middle = Color.Lerp(AuroraColors[middleFromIndex], AuroraColors[middleToIndex], transition);
      top = Color.Lerp(AuroraColors[topFromIndex], AuroraColors[topToIndex], transition);
    }
  }
}