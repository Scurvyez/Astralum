using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Astralum.API;
using Astralum.Astronomy;
using Astralum.Astronomy.BlackHoles;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.Astronomy.Pulsars;
using Astralum.Astronomy.SkyGrid;
using Astralum.Debugging;
using Astralum.Materials;
using Astralum.World;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Astralum.Harmony
{
  public static class HarmonyPatchesUtil
  {
    public const float ConstellationReportChance = 1f;
    
    public static readonly ConditionalWeakTable<Job, TelescopeReportData> TelescopeReports = new();
    
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
    
    public static void NotifySkygazeObservation(Pawn pawn)
    {
      SavedConstellation constellation = ConstellationObservationUtil.BestObservableConstellationFor(pawn);
      
      if (constellation == null)
        return;
      
      float curSunGlow = GenCelestial.CurCelestialSunGlow(pawn.MapHeld);
      
      if (pawn.MapHeld.TileInfo.Layer.Def.isSpace || curSunGlow < 0.1f)
        ObservationUtility.Notify_PawnObservedCelestialObject(
          pawn, CelestialObjectInfoUtil.FromConstellation(constellation));
      
      if (pawn.MapHeld.gameConditionManager.ConditionIsActive(GameConditionDefOf.Eclipse))
        ObservationUtility.Notify_PawnObservedDistantStarsDuringEclipse(pawn);
    }

    public static void AddSkyGridToggle(WidgetRow row)
    {
      string tooltip = SkyGridSettings.DrawGrid
        ? "Astra_DisableSkyGridToggleLabel".Translate()
        : "Astra_EnableSkyGridToggleLabel".Translate();
      
      row.ToggleableIcon(
        ref SkyGridSettings.DrawGrid,
        SkyCoordinateGridMatsUtil.ShowSkyGridIcon,
        tooltip, 
        SoundDefOf.Mouseover_ButtonToggle
      );
    }

    public static void AddConstellationLinesToggle(WidgetRow row)
    {
      string constellationLinesTooltip = ConstellationSettings.DrawConstellationLines
        ? "Astra_DisableConstellationLinesToggleLabel".Translate()
        : "Astra_EnableConstellationLinesToggleLabel".Translate();
      
      row.ToggleableIcon(
        ref ConstellationSettings.DrawConstellationLines,
        ConstellationsMatsUtil.ShowConstellationLinesIcon,
        constellationLinesTooltip,
        SoundDefOf.Mouseover_ButtonToggle
      );
    }
    
    public static void AddBlackHoleInfoToggle(WidgetRow row)
    {
      string blackHoleTooltip = BlackHoleSettings.DrawBlackHoleInfo
        ? "Astra_DisableBlackHoleInfoToggleLabel".Translate()
        : "Astra_EnableBlackHoleInfoToggleLabel".Translate();
      
      row.ToggleableIcon(
        ref BlackHoleSettings.DrawBlackHoleInfo,
        BlackHoleMatsUtil.ShowBlackHoleInfoIcon,
        blackHoleTooltip,
        SoundDefOf.Mouseover_ButtonToggle);
    }
    
    public static void AddPulsarInfoToggle(WidgetRow row)
    {
      string pulsarTooltip = PulsarSettings.DrawPulsarInfo
        ? "Astra_DisablePulsarInfoToggleLabel".Translate()
        : "Astra_EnablePulsarInfoToggleLabel".Translate();
      
      row.ToggleableIcon(
        ref PulsarSettings.DrawPulsarInfo,
        PulsarMatsUtil.ShowPulsarInfoIcon,
        pulsarTooltip,
        SoundDefOf.Mouseover_ButtonToggle
      );
    }
    
    public static void AddLocalStarInfoToggle(WidgetRow row)
    {
      string localStarInfoTooltip = CelestialNamingSettings.ShowNamingWindow
        ? "Astra_DisableLocalStarInfoToggleLabel".Translate()
        : "Astra_EnableLocalStarInfoToggleLabel".Translate();
      
      row.ToggleableIcon(
        ref LocalStarSettings.ShowLocalStarInfo,
        LocalSystemStarMatsUtil.ShowLocalStarInfoIcon,
        localStarInfoTooltip,
        SoundDefOf.Mouseover_ButtonToggle
      );
    }
    
    public static void AddCelestialNamingToggle(WidgetRow row)
    {
      string namingTooltip = CelestialNamingSettings.ShowNamingWindow
        ? "Astra_DisableCelestialNamingWindowToggleLabel".Translate()
        : "Astra_EnableCelestialNamingWindowToggleLabel".Translate();
      
      row.ToggleableIcon(
        ref CelestialNamingSettings.ShowNamingWindow,
        SkyCoordinateGridMatsUtil.ShowNamingDialogueIcon,
        namingTooltip,
        SoundDefOf.Mouseover_ButtonToggle
      );
    }
  }
}