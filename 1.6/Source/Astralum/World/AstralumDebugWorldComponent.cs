using Astralum.Astronomy.Stars;
using Astralum.Materials;
using LudeonTK;
using RimWorld.Planet;
using UnityEngine;

namespace Astralum.World
{
    // TODO:
    // This debug world component should be gated before release.
    // This updates the material every WorldComponentUpdate() and overwrites shader values with TweakValue fields.
    // That is perfect for development, but it should be disabled, #if DEBUG-guarded,
    // or controlled by a setting before release.
    public class AstralumDebugWorldComponent : WorldComponent
    {
        private const string TVCategory = "Astralum";
        
        private static bool _initializedFromStar;
        
        #region Color Properties
        [TweakValue(TVCategory, 0f, 1f)] 
        public static float ChromaColR = 1f;
        
        [TweakValue(TVCategory, 0f, 1f)] 
        public static float ChromaColG = 1f;
        
        [TweakValue(TVCategory, 0f, 1f)] 
        public static float ChromaColB = 1f;
        
        [TweakValue(TVCategory, 0f, 1f)] 
        public static float CoronaColR = 1f;
        
        [TweakValue(TVCategory, 0f, 1f)] 
        public static float CoronaColG = 1f;
        
        [TweakValue(TVCategory, 0f, 1f)] 
        public static float CoronaColB = 1f;
        #endregion
        
        #region All Other Properties
        [TweakValue(TVCategory, 0f, 5f)]
        private static float SurfaceNoiseStrength = 0.03f;
        
        [TweakValue(TVCategory, -50f, 50f)]
        private static float CoronaRotationSpeed = 0f;
        
        [TweakValue(TVCategory, 0f, 2f)]
        private static float ChromaticityIntensity = 1f;
        
        [TweakValue(TVCategory, 0f, 2f)]
        private static float CoronaIntensity = 1f;
        
        [TweakValue(TVCategory, 0f, 2f)]
        private static float OuterCoronaIntensity = 0.25f;
        
        [TweakValue(TVCategory, 0.5f, 10f)]
        private static float ChromaticityFalloffPower = 2f;
        
        [TweakValue(TVCategory, 0f, 10f)]
        private static float CoronaPower = 5f;
        
        [TweakValue(TVCategory, 0f, 10f)]
        private static float OuterCoronaPower = 6f;
        
        [TweakValue(TVCategory, 0f, 0.5f)]
        private static float VariabilityAmount = 0f;
        
        [TweakValue(TVCategory, 0f, 5f)]
        private static float VariabilitySpeed = 0f;
        #endregion
        
        public AstralumDebugWorldComponent(RimWorld.Planet.World world) : base(world)
        {
        }
        
        public override void WorldComponentUpdate()
        {
            base.WorldComponentUpdate();
            
            EnsureInitializedFromCurrentStar();
            ApplyTweaks();
        }
        
        public static void ResetTweaksFromCurrentStar()
        {
            _initializedFromStar = false;
            EnsureInitializedFromCurrentStar();
        }
        
        private static void ApplyTweaks()
        {
            Material mat = MaterialsUtil.Sun01Mat;
            
            if (mat == null)
                return;
            
            Color chromaCol = new Color(ChromaColR, ChromaColG, ChromaColB, 1f);
            Color coronaCol = new Color(CoronaColR, CoronaColG, CoronaColB, 1f);
            mat.SetColor(InternalShaderPropertyIds.Chromaticity, chromaCol);
            mat.SetColor(InternalShaderPropertyIds.Corona, coronaCol);
            mat.SetFloat(InternalShaderPropertyIds.CoronaRotationSpeed, CoronaRotationSpeed);
            mat.SetFloat(InternalShaderPropertyIds.ChromaticityIntensity, ChromaticityIntensity);
            mat.SetFloat(InternalShaderPropertyIds.CoronaIntensity, CoronaIntensity);
            mat.SetFloat(InternalShaderPropertyIds.OuterCoronaIntensity, OuterCoronaIntensity);
            mat.SetFloat(InternalShaderPropertyIds.ChromaticityFalloffPower, ChromaticityFalloffPower);
            mat.SetFloat(InternalShaderPropertyIds.CoronaPower, CoronaPower);
            mat.SetFloat(InternalShaderPropertyIds.OuterCoronaPower, OuterCoronaPower);
            mat.SetFloat(InternalShaderPropertyIds.SurfaceNoiseStrength, SurfaceNoiseStrength);
            mat.SetFloat(InternalShaderPropertyIds.VariabilityAmount, VariabilityAmount);
            mat.SetFloat(InternalShaderPropertyIds.VariabilitySpeed, VariabilitySpeed);
        }
        
        private static void EnsureInitializedFromCurrentStar()
        {
            if (_initializedFromStar)
                return;
            
            SavedStar star = WorldUtils.CurrentStar;
            
            if (star == null)
                return;
            
            ChromaColR = star.chromaticity.r;
            ChromaColG = star.chromaticity.g;
            ChromaColB = star.chromaticity.b;
            
            CoronaColR = star.corona.r;
            CoronaColG = star.corona.g;
            CoronaColB = star.corona.b;
            
            CoronaRotationSpeed = star.rotation;
            ChromaticityIntensity = star.chromaticityIntensity;
            CoronaIntensity = star.coronaIntensity;
            OuterCoronaIntensity = star.outerCoronaIntensity;
            ChromaticityFalloffPower = star.chromaticityFalloffPower;
            CoronaPower = star.coronaPower;
            OuterCoronaPower = star.outerCoronaPower;
            SurfaceNoiseStrength = star.surfaceNoiseStrength;
            
            bool intrinsicVariable =
                star.variabilityType == StellarVariabilityUtil.StellarVariabilityType.Intrinsic &&
                star.variabilityAmount > 0f;
            
            VariabilityAmount = intrinsicVariable ? star.variabilityAmount : 0f;
            VariabilitySpeed = intrinsicVariable ? star.variabilitySpeed : 0f;
            
            _initializedFromStar = true;
        }
    }
}