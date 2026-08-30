using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Astralum.DefOfs;
using Astralum.Materials;
using Astralum.UI;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.Profile;

namespace Astralum.Harmony
{
  [StaticConstructorOnStartup]
  public static class HarmonyPatches
  {
    static HarmonyPatches()
    {
      HarmonyLib.Harmony harmony = new("scurvyez.astralum.rimworld");

      PatchWorldInterfaceOnGUI(harmony);
      PatchPlaySettings(harmony);
      PatchJobDriverGetReport(harmony);
      PatchSkygazeMakeNewToils(harmony);
      PatchMemoryUtility(harmony);
      PatchGlobalDrawLayerSun(harmony);
    }
    
    /// <summary>
    ///   Patches the world interface to draw the world info window.
    /// </summary>
    private static void PatchWorldInterfaceOnGUI(HarmonyLib.Harmony harmony)
    {
      MethodInfo worldInterfaceOnGUI = HarmonyPatchesUtil.Method(
        typeof(WorldInterface), "WorldInterfaceOnGUI",
        "World interface GUI patch");

      harmony.Patch(worldInterfaceOnGUI,
        postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(WorldInterface_WorldInterfaceOnGUI_Postfix)));
    }
    
    /// <summary>
    ///   Patches the play settings to add a toggleable sky coordinate grid.
    /// </summary>
    private static void PatchPlaySettings(HarmonyLib.Harmony harmony)
    {
      MethodInfo doWorldViewControls = HarmonyPatchesUtil.Method(
        typeof(PlaySettings), "DoWorldViewControls",
        "Play settings patch");

      if (HarmonyPatchesUtil.Missing(doWorldViewControls))
        return;

      harmony.Patch(doWorldViewControls,
        postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(PlaySettings_DoWorldViewControls_Postfix)));
    }
    
    /// <summary>
    ///   Patches the "use telescope" job to output the currently viewed constellation or star(s) within. 
    /// </summary>
    private static void PatchJobDriverGetReport(HarmonyLib.Harmony harmony)
    {
      MethodInfo getReport = HarmonyPatchesUtil.Method(
        typeof(JobDriver), "GetReport",
        "Get report patch");

      if (HarmonyPatchesUtil.Missing(getReport))
        return;

      harmony.Patch(getReport, 
        postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(JobDriver_GetReport_Postfix)));
    }
    
    private static void PatchSkygazeMakeNewToils(HarmonyLib.Harmony harmony)
    {
      MethodInfo makeNewToils = HarmonyPatchesUtil.Method(
        typeof(JobDriver_Skygaze), "MakeNewToils",
        "Skygaze observation hook patch");
      
      if (HarmonyPatchesUtil.Missing(makeNewToils))
        return;
      
      harmony.Patch(makeNewToils,
        postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(JobDriver_Skygaze_MakeNewToils_Postfix)));
    }
    
    private static void PatchMemoryUtility(HarmonyLib.Harmony harmony)
    {
      MethodInfo clearAllMapsAndWorld = HarmonyPatchesUtil.Method(
        typeof(MemoryUtility), "ClearAllMapsAndWorld",
        "Clear all custom Unity Materials patch");
      
      if (HarmonyPatchesUtil.Missing(clearAllMapsAndWorld))
        return;
      
      harmony.Patch(clearAllMapsAndWorld,
        postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(MemoryUtility_ClearAllMapsAndWorld_Postfix)));
    }
    
    private static void PatchGlobalDrawLayerSun(HarmonyLib.Harmony harmony)
    {
      MethodInfo regenerate = HarmonyPatchesUtil.Method(
        typeof(GlobalDrawLayer_Sun), "Regenerate",
        "Regenerate cancellation patch");
      
      if (HarmonyPatchesUtil.Missing(regenerate))
        return;
      
      harmony.Patch(regenerate,
        prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(GlobalDrawLayer_Sun_Regenerate_Prefix)));
    }

    public static void WorldInterface_WorldInterfaceOnGUI_Postfix()
    {
      CelestialNamingDialogUtil.Update(true);
    }
    
    public static void PlaySettings_DoWorldViewControls_Postfix(WidgetRow row)
    {
      // displayed sequentially in order of addition
      HarmonyPatchesUtil.AddSkyGridToggle(row);
      HarmonyPatchesUtil.AddCelestialNamingToggle(row);
      HarmonyPatchesUtil.AddLocalStarInfoToggle(row);
      HarmonyPatchesUtil.AddConstellationLinesToggle(row);
      HarmonyPatchesUtil.AddBlackHoleInfoToggle(row);
      HarmonyPatchesUtil.AddPulsarInfoToggle(row);
    }

    public static void JobDriver_GetReport_Postfix(JobDriver __instance, ref string __result)
    {
      Job job = __instance?.job;
      
      if (job?.def != InternalDefOf.UseTelescope)
        return;
      
      if (job == null) 
        return;
      
      TelescopeReportData reportData = HarmonyPatchesUtil.TelescopeReports.GetValue(
        job,
        _ => HarmonyPatchesUtil.CreateTelescopeReportData(__instance.pawn)
      );
      
      if (!reportData.useConstellationReport || reportData.report.NullOrEmpty())
        return;
      
      __result = reportData.report;
    }
    
    public static IEnumerable<Toil> JobDriver_Skygaze_MakeNewToils_Postfix(IEnumerable<Toil> __result, 
      JobDriver_Skygaze __instance)
    {
      int index = 0;
      
      foreach (Toil toil in __result)
      {
        if (index == 1)
        {
          Action oldInitAction = toil.initAction;
          toil.initAction = delegate
          {
            oldInitAction?.Invoke();
            
            HarmonyPatchesUtil.NotifySkygazeObservation(__instance.pawn);
          };
        }
        
        index++;
        yield return toil;
      }
    }
    
    public static void MemoryUtility_ClearAllMapsAndWorld_Postfix()
    {
      LocalStarsMatsUtil.Clear();
      BlackHoleMatsUtil.Clear();
      ConstellationsMatsUtil.Clear();
      GalacticDustLaneMatsUtil.Clear();
      NebulaeMatsUtil.Clear();
      PulsarMatsUtil.Clear();
    }
    
    public static bool GlobalDrawLayer_Sun_Regenerate_Prefix(ref IEnumerable __result)
    {
      __result = Array.Empty<object>();
      return false;
    }
  }
}