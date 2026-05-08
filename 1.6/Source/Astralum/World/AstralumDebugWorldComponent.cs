using Astralum.Materials;
using LudeonTK;
using RimWorld.Planet;
using UnityEngine;

namespace Astralum.World
{
    public class AstralumDebugWorldComponent : WorldComponent
    {
        [TweakValue("Astralum", 0f, 2f)]
        private static float ChromaticityIntensity = 1f;
        
        [TweakValue("Astralum", 0f, 2f)]
        private static float CoronaIntensity = 1f;
        
        [TweakValue("Astralum", 0.5f, 10f)]
        private static float ChromaticityFalloffPower = 2f;
        
        [TweakValue("Astralum", 0f, 10f)]
        private static float CoronaPower = 5f;
        
        [TweakValue("Astralum", 0f, 1f)]
        private static float RadiusPower = 1f;
        
        [TweakValue("Astralum", 0f, 0.5f)]
        private static float VariabilityAmount = 0f;
        
        [TweakValue("Astralum", 0f, 5f)]
        private static float VariabilitySpeed = 1f;
        
        public AstralumDebugWorldComponent(RimWorld.Planet.World world) : base(world)
        {
        }

        public override void WorldComponentUpdate()
        {
            base.WorldComponentUpdate();
            ApplyTweaks();
        }
        
        private static void ApplyTweaks()
        {
            Material mat = MaterialsUtil.Sun01Mat;
            
            if (mat == null)
                return;
            
            mat.SetFloat(InternalShaderPropertyIds.ChromaticityIntensity, ChromaticityIntensity);
            mat.SetFloat(InternalShaderPropertyIds.CoronaIntensity, CoronaIntensity);
            mat.SetFloat(InternalShaderPropertyIds.ChromaticityFalloffPower, ChromaticityFalloffPower);
            mat.SetFloat(InternalShaderPropertyIds.CoronaPower, CoronaPower);
            mat.SetFloat(InternalShaderPropertyIds.VariabilityAmount, VariabilityAmount);
            mat.SetFloat(InternalShaderPropertyIds.VariabilitySpeed, VariabilitySpeed);
        }
    }
}