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

            MethodInfo regenerate = AccessTools.Method(
                typeof(GlobalDrawLayer_Sun), nameof(GlobalDrawLayer_Sun.Regenerate));
            
            MethodInfo moveNext = AccessTools.EnumeratorMoveNext(regenerate);
            MethodInfo extraOnGUI = AccessTools.Method(typeof(Page_SelectStartingSite), "ExtraOnGUI");
            
            if (moveNext == null)
            {
                AstraLog.Warning("Could not find GlobalDrawLayer_Sun.Regenerate MoveNext. Sun patch was not applied.");
                return;
            }
            
            harmony.Patch(original: moveNext,
                transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(GlobalDrawLayer_Sun_Regenerate_Transpiler)));
            
            harmony.Patch(original: AccessTools.Method(typeof(WorldInterface), "WorldInterfaceOnGUI"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(WorldInterface_WorldInterfaceOnGUI_Postfix)));
            
            if (extraOnGUI == null)
            {
                AstraLog.Warning("Could not find Page_SelectStartingSite.ExtraOnGUI. " +
                                 "Starting site star UI patch was not applied.");
            }
            else
            {
                harmony.Patch(
                    original: extraOnGUI,
                    postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Page_SelectStartingSite_ExtraOnGUI_Postfix))
                );
            }
        }

        public static IEnumerable<CodeInstruction> GlobalDrawLayer_Sun_Regenerate_Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            
            MethodInfo vanillaSunGetter = AccessTools.PropertyGetter(
                typeof(WorldMaterials), nameof(WorldMaterials.Sun));
            FieldInfo vanillaSunField = AccessTools.Field(typeof(WorldMaterials), nameof(WorldMaterials.Sun));
            
            MethodInfo astralumSunGetter = AccessTools.PropertyGetter(
                typeof(MaterialsUtil), nameof(MaterialsUtil.Sun01Mat));
            
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