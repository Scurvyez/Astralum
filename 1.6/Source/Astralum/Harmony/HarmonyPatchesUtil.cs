using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Astralum.API;
using Astralum.Astronomy;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.Nebulae;
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
    
    private static readonly CelestialObjectType[] SkygazeObjectTypes =
    [
      CelestialObjectType.Constellation,
      CelestialObjectType.ConstellationStar,
      CelestialObjectType.Nebulae
    ];
    
    public static TelescopeReportData CreateTelescopeReportData(Pawn pawn)
    {
      if (!Rand.Chance(ConstellationReportChance))
        return new TelescopeReportData(false, null);
      
      SavedConstellation constellation = ConstellationObservationUtil.BestObservableConstellationFor(pawn);
      
      if (constellation?.DisplayName.NullOrEmpty() != false)
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

      Vector3 dir = WorldUtils.GetCurrentRotationForWorldSpace() * constellation.centerDir.normalized;

      string hemisphere = WorldUtils.SkyHemisphere(dir);
      
      SkyCoord coord = WorldUtils.DirectionToSkyCoord(dir);
      string ra = WorldUtils.FormatRightAscension(coord.rightAscensionHours);
      string dec = WorldUtils.FormatDeclination(coord.declinationDegrees);

      int maxPattern = hasTwoStars ? 8 : hasOneStar ? 6 : 2;
      int pattern = Rand.RangeInclusive(0, maxPattern);
      
      switch (pattern)
      {
        case 0:
          return "Astra_TelescopeReport_Constellation".Translate(constellation.DisplayName);
        
        case 1:
          return "Astra_TelescopeReport_ConstellationHemisphere".Translate(constellation.DisplayName, hemisphere);
        
        case 2:
          return "Astra_TelescopeReport_ConstellationCoords".Translate(constellation.DisplayName, ra, dec);
        
        case 3:
        {
          SavedConstellationStar star = stars.RandomElement();
          return "Astra_TelescopeReport_ConstellationStar".Translate(constellation.DisplayName, star.DisplayName);
        }
        
        case 4:
        {
          SavedConstellationStar star = stars.RandomElement();
          return "Astra_TelescopeReport_TracesStar".Translate(constellation.DisplayName, star.DisplayName);
        }
        
        case 5:
        {
          SavedConstellationStar star = stars.RandomElement();
          return "Astra_TelescopeReport_StarClass".Translate(constellation.DisplayName, star.DisplayName,
            star.spectralClass.ToString());
        }
        
        case 6:
        {
          SavedConstellationStar star = stars.RandomElement();
          return "Astra_TelescopeReport_StarHemisphere".Translate(constellation.DisplayName, star.DisplayName,
            hemisphere);
        }
        
        case 7:
        {
          GetTwoDifferentStars(stars, out SavedConstellationStar a, out SavedConstellationStar b);
          return "Astra_TelescopeReport_ConstellationTwoStars".Translate(constellation.DisplayName, 
            a.DisplayName, b.DisplayName);
        }
        
        default:
        {
          GetTwoDifferentStars(stars, out SavedConstellationStar a, out SavedConstellationStar b);
          return "Astra_TelescopeReport_TwoStarClasses".Translate(constellation.DisplayName, a.DisplayName,
            a.spectralClass.ToString(), b.DisplayName, b.spectralClass.ToString());
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
      Map map = pawn.MapHeld;
      bool darkEnoughOutside = TwilightUtility.SunAltitude(map) < -6f;
      
      if (map.TileInfo.Layer.Def.isSpace || darkEnoughOutside)
      {
        if (TryGetSkygazeObservation(pawn, out CelestialObjectInfo observation))
        {
          ObservationUtility.Notify_PawnObservedCelestialObject(pawn, observation);
        }
      }
      
      if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.Eclipse))
      {
        ObservationUtility.Notify_PawnObservedDistantStarsDuringEclipse(pawn);
      }
    }
    
    private static bool TryGetSkygazeObservation(Pawn pawn, out CelestialObjectInfo observation)
    {
      CelestialObjectType type = SkygazeObjectTypes.RandomElementByWeight(SkygazeWeightFor);
      
      return type switch
      {
        CelestialObjectType.Constellation => TryGetConstellationObservation(pawn, out observation),
        CelestialObjectType.ConstellationStar => TryGetConstellationStarObservation(pawn, out observation),
        CelestialObjectType.Nebulae => TryGetNebulaObservation(out observation),
        _ => Fail(out observation)
      };
    }
    
    private static float SkygazeWeightFor(CelestialObjectType type)
    {
      return type switch
      {
        CelestialObjectType.Constellation => CelestialObjectAvailability.HasConstellations() ? 0.75f : 0f,
        CelestialObjectType.ConstellationStar => CelestialObjectAvailability.HasConstellationStars() ? 0.50f : 0f,
        CelestialObjectType.Nebulae => CelestialObjectAvailability.HasNebulae() ? 0.05f : 0f,
        _ => 0f
      };
    }
    
    private static bool TryGetConstellationObservation(Pawn pawn, out CelestialObjectInfo observation)
    {
      SavedConstellation constellation = ConstellationObservationUtil.BestObservableConstellationFor(pawn);
      
      if (constellation == null)
      {
        observation = default;
        return false;
      }
      
      observation = CelestialObjectInfoUtil.FromConstellation(constellation);
      
      return true;
    }
    
    private static bool TryGetConstellationStarObservation(Pawn pawn, out CelestialObjectInfo observation)
    {
      SavedConstellation constellation = ConstellationObservationUtil.BestObservableConstellationFor(pawn);
      
      if (constellation == null || constellation.stars.NullOrEmpty())
      {
        observation = default;
        return false;
      }
      
      SavedConstellationStar star = constellation.stars.RandomElement();
      observation = CelestialObjectInfoUtil.FromConstellationStar(constellation, star);
      
      return true;
    }
    
    private static bool TryGetNebulaObservation(out CelestialObjectInfo observation)
    {
      WorldComponent_CelestialObjectDataCache comp = Find.World.GetComponent<WorldComponent_CelestialObjectDataCache>();
      
      if (comp?.Nebulas.NullOrEmpty() != false)
      {
        observation = default;
        return false;
      }
      
      SavedNebula nebula = comp.Nebulas.RandomElement();
      observation = CelestialObjectInfoUtil.FromNebula(nebula);
      
      return true;
    }
    
    private static bool Fail(out CelestialObjectInfo observation)
    {
      observation = default;
      return false;
    }
    
    public static void AddSkyGridToggle(WidgetRow row)
    {
      string tooltip = CelestialSettings.DrawSkyCoordGrid
        ? "Astra_DisableSkyGridToggleLabel".Translate()
        : "Astra_EnableSkyGridToggleLabel".Translate();
      
      row.ToggleableIcon(
        ref CelestialSettings.DrawSkyCoordGrid,
        UIMatsUtil.ShowSkyGridIcon,
        tooltip, 
        SoundDefOf.Mouseover_ButtonToggle
      );
    }
    
    public static void AddConstellationLinesToggle(WidgetRow row)
    {
      string constellationLinesTooltip = CelestialSettings.DrawConstellationLines
        ? "Astra_DisableConstellationLinesToggleLabel".Translate()
        : "Astra_EnableConstellationLinesToggleLabel".Translate();
      
      row.ToggleableIcon(
        ref CelestialSettings.DrawConstellationLines,
        UIMatsUtil.ShowConstellationLinesIcon,
        constellationLinesTooltip,
        SoundDefOf.Mouseover_ButtonToggle
      );
    }
    
    public static void AddBlackHoleInfoToggle(WidgetRow row)
    {
      string blackHoleTooltip = CelestialSettings.DrawBlackHoleInfo
        ? "Astra_DisableBlackHoleInfoToggleLabel".Translate()
        : "Astra_EnableBlackHoleInfoToggleLabel".Translate();
      
      row.ToggleableIcon(
        ref CelestialSettings.DrawBlackHoleInfo,
        UIMatsUtil.ShowBlackHoleInfoIcon,
        blackHoleTooltip,
        SoundDefOf.Mouseover_ButtonToggle);
    }
    
    public static void AddPulsarInfoToggle(WidgetRow row)
    {
      string pulsarTooltip = CelestialSettings.DrawPulsarInfo
        ? "Astra_DisablePulsarInfoToggleLabel".Translate()
        : "Astra_EnablePulsarInfoToggleLabel".Translate();
      
      row.ToggleableIcon(
        ref CelestialSettings.DrawPulsarInfo,
        UIMatsUtil.ShowPulsarInfoIcon,
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
        ref CelestialSettings.ShowLocalStarInfo,
        UIMatsUtil.ShowLocalStarInfoIcon,
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
        UIMatsUtil.ShowNamingDialogueIcon,
        namingTooltip,
        SoundDefOf.Mouseover_ButtonToggle
      );
    }
  }
}