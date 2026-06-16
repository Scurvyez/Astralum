using System;
using System.Collections.Generic;
using System.Reflection;
using Astralum.Astronomy.Constellations;
using Astralum.Debugging;
using Astralum.World;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace Astralum.Harmony
{
  public static class HarmonyPatchesUtil
  {
    public const float ConstellationReportChance = 1f;
    
    public static MethodInfo Method(Type type, string methodName, string patchDescription)
    {
      MethodInfo method = AccessTools.Method(type, methodName);

      if (method == null)
        AstraLog.Warning($"Could not find {type.Name}.{methodName}. {patchDescription} was not applied.");

      return method;
    }

    public static MethodInfo RequiredMethod(Type type, string methodName, string patchDescription)
    {
      MethodInfo method = Method(type, methodName, patchDescription);

      if (method == null) AstraLog.Warning("Required Harmony patch target was missing. Patch setup will stop.");

      return method;
    }

    public static MethodInfo EnumeratorMoveNext(MethodInfo enumerableMethod, string ownerDescription,
      string patchDescription)
    {
      if (enumerableMethod == null)
      {
        AstraLog.Warning($"Could not find {ownerDescription}. {patchDescription} was not applied.");
        return null;
      }

      MethodInfo moveNext = AccessTools.EnumeratorMoveNext(enumerableMethod);

      if (moveNext == null)
        AstraLog.Warning($"Could not find {ownerDescription} MoveNext. {patchDescription} was not applied.");

      return moveNext;
    }

    public static bool Missing(MethodInfo method)
    {
      return method == null;
    }
    
    public static TelescopeReportData CreateTelescopeReportData(Pawn pawn)
    {
      if (!Rand.Chance(ConstellationReportChance))
        return new TelescopeReportData(false, null);
      
      SavedConstellation constellation = ConstellationObservationUtil.BestObservableConstellationFor(pawn);
      
      if (constellation?.name.NullOrEmpty() != false)
        return new TelescopeReportData(false, null);
      
      string report = BuildTelescopeReport(constellation);
      
      return report.NullOrEmpty() 
        ? new TelescopeReportData(false, null) 
        : new TelescopeReportData(true, report);
    }
    
    private static string BuildTelescopeReport(SavedConstellation constellation)
    {
      List<SavedConstellationStar> stars = constellation.stars;
      
      bool hasOneStar = stars is { Count: >= 1 };
      bool hasTwoStars = stars is { Count: >= 2 };

      Vector3 dir = ConstellationObservationUtil.CurrentSkyRotation() * constellation.centerDir.normalized;

      string hemisphere = WorldUtils.SkyHemisphere(dir);
      
      SkyCoord coord = WorldUtils.DirectionToSkyCoord(dir);
      string ra = WorldUtils.FormatRightAscension(coord.rightAscensionHours);
      string dec = WorldUtils.FormatDeclination(coord.declinationDegrees);

      int maxPattern = hasTwoStars ? 8 : hasOneStar ? 6 : 2;
      int pattern = Rand.RangeInclusive(0, maxPattern);
      
      switch (pattern)
      {
        case 0:
          return "Astra_TelescopeReport_Constellation".Translate(constellation.name);
        
        case 1:
          return "Astra_TelescopeReport_ConstellationHemisphere".Translate(constellation.name, hemisphere);
        
        case 2:
          return "Astra_TelescopeReport_ConstellationCoords".Translate(constellation.name, ra, dec);
        
        case 3:
        {
          SavedConstellationStar star = stars.RandomElement();
          
          return "Astra_TelescopeReport_ConstellationStar".Translate(constellation.name, star.name);
        }
        
        case 4:
        {
          SavedConstellationStar star = stars.RandomElement();
          
          return "Astra_TelescopeReport_TracesStar".Translate(constellation.name, star.name);
        }
        
        case 5:
        {
          SavedConstellationStar star = stars.RandomElement();
          
          return "Astra_TelescopeReport_StarClass".Translate(constellation.name, star.name,
            star.spectralClass.ToString());
        }
        
        case 6:
        {
          SavedConstellationStar star = stars.RandomElement();
          
          return "Astra_TelescopeReport_StarHemisphere".Translate(constellation.name, star.name,
            hemisphere);
        }
        
        case 7:
        {
          GetTwoDifferentStars(stars, out SavedConstellationStar a, out SavedConstellationStar b);
          
          return "Astra_TelescopeReport_ConstellationTwoStars".Translate(constellation.name, 
            a.name, b.name);
        }
        
        default:
        {
          GetTwoDifferentStars(stars, out SavedConstellationStar a, out SavedConstellationStar b);
          
          return "Astra_TelescopeReport_TwoStarClasses".Translate(constellation.name, a.name,
            a.spectralClass.ToString(), b.name, b.spectralClass.ToString());
        }
      }
    }
    
    private static void GetTwoDifferentStars(List<SavedConstellationStar> stars,
      out SavedConstellationStar a, out SavedConstellationStar b)
    {
      a = stars.RandomElement();
      b = stars.RandomElement();
      
      for (int i = 0; i < 8 && ReferenceEquals(a, b); i++)
        b = stars.RandomElement();
      
      if (ReferenceEquals(a, b))
        b = stars[(stars.IndexOf(a) + 1) % stars.Count];
    }
  }
}