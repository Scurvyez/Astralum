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
            
            harmony.Patch(original: moveNextSun,
                transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(GlobalDrawLayer_Sun_Regenerate_Transpiler)));
            
            MethodInfo renderLayerGetter = AccessTools.PropertyGetter(typeof(GlobalDrawLayer_Sun), "RenderLayer");
            if (renderLayerGetter == null)
            {
                AstraLog.Warning("Could not find GlobalDrawLayer_Sun.RenderLayer getter. Sun render layer patch was not applied.");
                return;
            }
            
            harmony.Patch(original: renderLayerGetter,
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(GlobalDrawLayer_Sun_RenderLayer_Postfix)));
            
            MethodInfo regenerateStars = HarmonyPatchesUtil.RequiredMethod(
                typeof(GlobalDrawLayer_Stars), nameof(GlobalDrawLayer_Stars.Regenerate),
                "Stars patch");
            
            MethodInfo moveNextStars = HarmonyPatchesUtil.EnumeratorMoveNext(
                regenerateStars, "GlobalDrawLayer_Stars.Regenerate",
                "Stars patch");
            if (moveNextStars == null)
            {
                AstraLog.Warning("Could not find GlobalDrawLayer_Stars.Regenerate MoveNext. Stars patch was not applied.");
                return;
            }
            
            if (HarmonyPatchesUtil.Missing(moveNextStars))
                return;
            
            harmony.Patch(original: moveNextStars,
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(GlobalDrawLayer_Stars_Regenerate_Prefix)));
            
            MethodInfo worldInterfaceOnGUI = HarmonyPatchesUtil.Method(
                typeof(WorldInterface), "WorldInterfaceOnGUI", 
                "World interface GUI patch");
            
            harmony.Patch(original: worldInterfaceOnGUI,
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(WorldInterface_WorldInterfaceOnGUI_Postfix)));
            
            MethodInfo extraOnGUI = HarmonyPatchesUtil.Method(
                typeof(Page_SelectStartingSite), "ExtraOnGUI", 
                "Starting site star UI patch");
            if (!HarmonyPatchesUtil.Missing(extraOnGUI))
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
                typeof(StarMaterialsUtil), nameof(StarMaterialsUtil.Star01Mat));
            
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
        
        public static void GlobalDrawLayer_Sun_RenderLayer_Postfix(ref int __result)
        {
            __result = WorldCameraManager.WorldLayer;
        }
        
        public static bool GlobalDrawLayer_Stars_Regenerate_Prefix()
        {
            return false;
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