using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Astralum.DefOfs;
using Astralum.Materials;
using Astralum.Settings;
using Astralum.UI;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
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
      PatchWorldGridGizmos(harmony);
      PatchSelectStartingSiteDoCustomBottomButtons(harmony);
      PatchJobDriverGetReport(harmony);
      PatchSkygazeMakeNewToils(harmony);
      PatchMemoryUtility(harmony);
      PatchGlobalDrawLayerSun(harmony);
      PatchGlobalDrawLayerStars(harmony);
    }
    
    /// <summary>
    /// Patches the world interface to draw the world info window.
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
    /// Patches the play settings to add a toggleable sky coordinate grid.
    /// </summary>
    private static void PatchPlaySettings(HarmonyLib.Harmony harmony)
    {
      MethodInfo globalControls = HarmonyPatchesUtil.Method(
        typeof(PlaySettings), "DoPlaySettingsGlobalControls",
        "Play settings patch");

      if (HarmonyPatchesUtil.Missing(globalControls))
        return;

      harmony.Patch(globalControls,
        postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(PlaySettings_DoPlaySettingsGlobalControls_Postfix)));
    }

    private static void PatchWorldGridGizmos(HarmonyLib.Harmony harmony)
    {
      MethodInfo worldGridGizmos = HarmonyPatchesUtil.Method(
        typeof(WorldGrid), "GetGizmos",
        "Get gizmos patch");

      if (HarmonyPatchesUtil.Missing(worldGridGizmos))
        return;
      
      harmony.Patch(worldGridGizmos,
        postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(WorldGrid_GetGizmos_Postfix)));
    }

    private static void PatchSelectStartingSiteDoCustomBottomButtons(HarmonyLib.Harmony harmony)
    {
      MethodInfo doCustomBottomButtons = HarmonyPatchesUtil.Method(
        typeof(Page_SelectStartingSite), "DoCustomBottomButtons",
        "Settlement selection screen bottom buttons patch");

      if (HarmonyPatchesUtil.Missing(doCustomBottomButtons))
        return;

      harmony.Patch(doCustomBottomButtons,
        postfix: new HarmonyMethod(typeof(HarmonyPatches),
          nameof(Page_SelectStartingSite_DoCustomBottomButtons_Postfix)));
    }
    
    /// <summary>
    /// Patches the "use telescope" job to output the currently viewed constellation or star(s) within. 
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
    
    private static void PatchGlobalDrawLayerStars(HarmonyLib.Harmony harmony)
    {
      MethodInfo regenerate = HarmonyPatchesUtil.Method(
        typeof(GlobalDrawLayer_Stars), "Regenerate",
        "Regenerate cancellation patch");
      
      if (HarmonyPatchesUtil.Missing(regenerate))
        return;
      
      harmony.Patch(regenerate,
        prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(GlobalDrawLayer_Sun_Regenerate_Prefix)));
    }
    
    public static void WorldInterface_WorldInterfaceOnGUI_Postfix()
    {
      CelestialCatalogueDialogUtil.Update(true);
    }
    
    public static void PlaySettings_DoPlaySettingsGlobalControls_Postfix(WidgetRow row, bool worldView)
    {
      if (!worldView || Current.ProgramState != ProgramState.Playing) 
        return;
      
      // displayed sequentially in order of addition
      HarmonyPatchesUtil.AddSkyGridToggle(row);
      HarmonyPatchesUtil.AddLocalStarInfoToggle(row);
      HarmonyPatchesUtil.AddConstellationLinesToggle(row);
      HarmonyPatchesUtil.AddBlackHoleInfoToggle(row);
      HarmonyPatchesUtil.AddPulsarInfoToggle(row);
    }
    
    public static IEnumerable<Gizmo> WorldGrid_GetGizmos_Postfix(IEnumerable<Gizmo> __result)
    {
      foreach (Gizmo gizmo in __result)
        yield return gizmo;
      
      if (Current.ProgramState != ProgramState.Playing)
        yield break;
      
      yield return new Command_Action
      {
        defaultLabel = "Astra_UI_CelestialCatalogueLabel".Translate(),
        defaultDesc = "Astra_UI_CelestialCatalogueDesc".Translate(),
        icon = UIMatsUtil.CelestialCatalogueCommandIcon,
        action = () =>
        {
          var window = Find.WindowStack.WindowOfType<Dialog_CelestialCatalogue>();
          
          if (window != null)
          {
            window.Close();
            return;
          }
          
          Find.WindowStack.Add(new Dialog_CelestialCatalogue());
        }
      };
    }

    public static void Page_SelectStartingSite_DoCustomBottomButtons_Postfix()
    {
      const float gap = 10f;
      const int vanillaButtonCount = 4;
      
      const float buttonWidth = 200f;
      const float buttonHeight = 38f;
      
      float screenWidth = Screen.width / Prefs.UIScale;
      float screenHeight = Screen.height / Prefs.UIScale;
      
      int rows = vanillaButtonCount < 3 || screenWidth >= 540f + vanillaButtonCount * (buttonWidth + gap) ? 1 : 2;
      int buttonsPerRow = Mathf.CeilToInt(vanillaButtonCount / (float)rows);
      
      float panelWidth = buttonWidth * buttonsPerRow + gap * (buttonsPerRow + 1);
      float panelHeight = rows * buttonHeight + gap * (rows + 1);
      float panelX = (screenWidth - panelWidth) * 0.5f;
      float panelY = screenHeight - panelHeight - 4f;
      
      Rect buttonRect = new(
        panelX + (panelWidth - buttonWidth) * 0.5f,
        panelY - buttonHeight - gap,
        buttonWidth,
        buttonHeight
      );
      
      if (Widgets.ButtonText(buttonRect, "Astra_UI_CelestialOverview".Translate()))
      {
        Find.WindowStack.Add(new Dialog_CelestialWorldOverview(buttonRect));
      }
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
      if (!AstraSettings.OverrideVanillaSun)
        return true;
      
      __result = Array.Empty<object>();
      return false;
    }
    
    public static bool GlobalDrawLayer_Stars_Regenerate_Prefix(ref IEnumerable __result)
    {
      if (!AstraSettings.RenderBackgroundStars)
        return true;
      
      __result = Array.Empty<object>();
      return false;
    }
  }
}