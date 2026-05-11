using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Astralum.Debugging;
using Astralum.Materials;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Astralum.Harmony
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyLib.Harmony harmony = new (id: "scurvyez.astralum.rimworld");
            
            PatchSunRegenerate(harmony);
            PatchWorldInterfaceOnGUI(harmony);
            PatchStartingSiteExtraOnGUI(harmony);
        }
        
        /// <summary>
        /// Replaces the sun material with the astralum sun material.
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
                original: moveNextSun,
                transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(GlobalDrawLayer_Sun_Regenerate_Transpiler))
            );
        }
        
        /// <summary>
        /// Patches the world interface to draw the world info window.
        /// </summary>
        private static void PatchWorldInterfaceOnGUI(HarmonyLib.Harmony harmony)
        {
            MethodInfo worldInterfaceOnGUI = HarmonyPatchesUtil.Method(
                typeof(WorldInterface), "WorldInterfaceOnGUI",
                "World interface GUI patch");
            
            harmony.Patch(
                original: worldInterfaceOnGUI,
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(WorldInterface_WorldInterfaceOnGUI_Postfix))
            );
        }
        
        /// <summary>
        /// Patches the starting site extra on GUI to draw the world info overlay.
        /// </summary>
        private static void PatchStartingSiteExtraOnGUI(HarmonyLib.Harmony harmony)
        {
            MethodInfo extraOnGUI = HarmonyPatchesUtil.Method(
                typeof(Page_SelectStartingSite), "ExtraOnGUI",
                "Starting site star UI patch");
            
            if (HarmonyPatchesUtil.Missing(extraOnGUI))
                return;
            
            harmony.Patch(
                original: extraOnGUI,
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Page_SelectStartingSite_ExtraOnGUI_Postfix))
            );
        }
        
        public static IEnumerable<CodeInstruction> GlobalDrawLayer_Sun_Regenerate_Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            
            MethodInfo vanillaSunGetter = AccessTools.PropertyGetter(
                typeof(WorldMaterials), nameof(WorldMaterials.Sun));
            FieldInfo vanillaSunField = AccessTools.Field(typeof(WorldMaterials), nameof(WorldMaterials.Sun));
            
            MethodInfo astralumSunGetter = AccessTools.PropertyGetter(
                typeof(StarsMaterialsUtil), nameof(StarsMaterialsUtil.Star01Mat));
            
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
            {
                AstraLog.Warning(
                    "Sun transpiler made 0 replacements. " +
                    "The IL may not reference WorldMaterials.Sun in the expected way." +
                    "Check your shit Steve..."
                );
            }
            return codes;
        }
        
        public static void WorldInterface_WorldInterfaceOnGUI_Postfix()
        {
            UI.AstralumWorldInfoWindowManager.Update(requirePlaying: true);
        }
        
        public static void Page_SelectStartingSite_ExtraOnGUI_Postfix()
        {
            UI.AstralumWorldInfoOverlay.DrawOnGUI(requirePlaying: false);
        }
    }
}