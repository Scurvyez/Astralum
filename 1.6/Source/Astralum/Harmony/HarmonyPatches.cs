using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.SkyGrid;
using Astralum.Debugging;
using Astralum.DefOfs;
using Astralum.Materials;
using Astralum.Settings;
using Astralum.UI;
using Astralum.World;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace Astralum.Harmony
{
  [StaticConstructorOnStartup]
  public static class HarmonyPatches
  {
    static HarmonyPatches()
    {
      HarmonyLib.Harmony harmony = new("scurvyez.astralum.rimworld");

      PatchSunRegenerate(harmony);
      PatchWorldInterfaceOnGUI(harmony);
      PatchStartingSiteExtraOnGUI(harmony);
      PatchPlaySettings(harmony);
      PatchJobDriverGetReport(harmony);
    }
    
    private sealed class TelescopeReportData
    {
      public readonly bool useConstellationReport;
      public readonly string constellationName;

      public TelescopeReportData(bool useConstellationReport, string constellationName)
      {
        this.useConstellationReport = useConstellationReport;
        this.constellationName = constellationName;
      }
    }
    
    private static readonly ConditionalWeakTable<Job, TelescopeReportData> TelescopeReports = new();

    /// <summary>
    ///   Replaces the sun material with the astralum sun material.
    /// </summary>
    private static void PatchSunRegenerate(HarmonyLib.Harmony harmony)
    {
      MethodInfo regenerateSun = HarmonyPatchesUtil.RequiredMethod(
        typeof(GlobalDrawLayer_Sun), nameof(GlobalDrawLayer_Sun.Regenerate),
        "Sun patch");

      MethodInfo moveNextSun = HarmonyPatchesUtil.EnumeratorMoveNext(
        regenerateSun, "GlobalDrawLayer_Sun.Regenerate",
        "Sun patch");

      if (moveNextSun == null)
      {
        AstraLog.Warning("Could not find GlobalDrawLayer_Sun.Regenerate MoveNext. Sun patch was not applied.");
        return;
      }

      if (HarmonyPatchesUtil.Missing(moveNextSun))
        return;

      harmony.Patch(
        moveNextSun,
        transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(GlobalDrawLayer_Sun_Regenerate_Transpiler)));
    }

    /// <summary>
    ///   Patches the world interface to draw the world info window.
    /// </summary>
    private static void PatchWorldInterfaceOnGUI(HarmonyLib.Harmony harmony)
    {
      MethodInfo worldInterfaceOnGUI = HarmonyPatchesUtil.Method(
        typeof(WorldInterface), "WorldInterfaceOnGUI",
        "World interface GUI patch");

      harmony.Patch(
        worldInterfaceOnGUI,
        postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(WorldInterface_WorldInterfaceOnGUI_Postfix)));
    }

    /// <summary>
    ///   Patches the starting site extra on GUI to draw the world info overlay.
    /// </summary>
    private static void PatchStartingSiteExtraOnGUI(HarmonyLib.Harmony harmony)
    {
      MethodInfo extraOnGUI = HarmonyPatchesUtil.Method(
        typeof(Page_SelectStartingSite), "ExtraOnGUI",
        "Starting site star UI patch");

      if (HarmonyPatchesUtil.Missing(extraOnGUI))
        return;

      harmony.Patch(
        extraOnGUI,
        postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Page_SelectStartingSite_ExtraOnGUI_Postfix)));
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

      harmony.Patch(
        doWorldViewControls,
        postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(PlaySettings_DoWorldViewControls_Postfix)));
    }

    private static void PatchJobDriverGetReport(HarmonyLib.Harmony harmony)
    {
      MethodInfo getReport = HarmonyPatchesUtil.Method(
        typeof(JobDriver), "GetReport",
        "Get report patch");

      if (HarmonyPatchesUtil.Missing(getReport))
        return;

      harmony.Patch(
        getReport,
        postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(JobDriver_GetReport_Postfix)));
    }

    public static IEnumerable<CodeInstruction> GlobalDrawLayer_Sun_Regenerate_Transpiler(
      IEnumerable<CodeInstruction> instructions)
    {
      if (!AstraSettings.OverrideVanillaSun)
        return instructions;
      
      List<CodeInstruction> codes = instructions.ToList();

      MethodInfo vanillaSunGetter = AccessTools.PropertyGetter(
        typeof(WorldMaterials), nameof(WorldMaterials.Sun));
      FieldInfo vanillaSunField = AccessTools.Field(typeof(WorldMaterials), nameof(WorldMaterials.Sun));

      MethodInfo astralumSunGetter = AccessTools.PropertyGetter(
        typeof(LocalSystemStarMatsUtil), nameof(LocalSystemStarMatsUtil.Star01Mat));

      if (astralumSunGetter == null)
      {
        AstraLog.Warning("Could not find MaterialsUtil.Sun01Mat getter. Returning original instructions.");
        return codes;
      }

      int replacements = 0;

      for (int i = 0; i < codes.Count; i++)
      {
        CodeInstruction instruction = codes[i];

        bool isVanillaSunGetterCall = vanillaSunGetter != null && instruction.Calls(vanillaSunGetter);

        bool isVanillaSunFieldLoad =
          vanillaSunField != null &&
          instruction.opcode == OpCodes.Ldsfld &&
          Equals(instruction.operand, vanillaSunField);

        if (!isVanillaSunGetterCall && !isVanillaSunFieldLoad) continue;
        codes[i] = new CodeInstruction(OpCodes.Call, astralumSunGetter)
        {
          labels = instruction.labels,
          blocks = instruction.blocks
        };

        replacements++;
      }

      if (replacements == 0)
        AstraLog.Warning(
          "Sun transpiler made 0 replacements. " +
          "The IL may not reference WorldMaterials.Sun in the expected way." +
          "Check your shit Steve..."
        );
      return codes;
    }

    public static void WorldInterface_WorldInterfaceOnGUI_Postfix()
    {
      AstralumWorldInfoWindowManager.Update(true);
    }

    public static void Page_SelectStartingSite_ExtraOnGUI_Postfix()
    {
      AstralumWorldInfoOverlay.DrawOnGUI();
    }

    public static void PlaySettings_DoWorldViewControls_Postfix(WidgetRow row)
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

    public static void JobDriver_GetReport_Postfix(JobDriver __instance, ref string __result)
    {
      Job job = __instance?.job;
      
      if (job?.def != InternalDefOf.UseTelescope)
        return;
      
      TelescopeReportData reportData = TelescopeReports.GetValue(job, _ => CreateTelescopeReportData());
      
      if (!reportData.useConstellationReport)
        return;
      
      if (reportData.constellationName.NullOrEmpty())
        return;
      
      __result = $"observing {reportData.constellationName} through a telescope.";
    }
    
    private static TelescopeReportData CreateTelescopeReportData()
    {
      if (!Rand.Chance(HarmonyPatchesUtil.ConstellationReportChance))
        return new TelescopeReportData(false, null);
      
      string constellationName = RandomConstellationName();
      
      if (constellationName.NullOrEmpty())
        return new TelescopeReportData(false, null);
      
      return new TelescopeReportData(true, constellationName);
    }
    
    private static string RandomConstellationName()
    {
      WorldComponent_ConstellationData data = ConstellationDataUtil.Data;
      
      if (data?.constellations.NullOrEmpty() != false)
        return null;
      
      SavedConstellation constellation = data.constellations.RandomElement();
      
      return constellation?.name;
    }
  }
}