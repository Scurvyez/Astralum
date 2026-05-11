using Astralum.Astronomy.Stars;
using Astralum.Materials;
using LudeonTK;
using RimWorld.Planet;
using UnityEngine;

namespace Astralum.World
{
    public class AstralumDebugWorldComponent : WorldComponent
    {
        private const string TVCategory = "Astralum";
        
        private static bool _initializedFromStar;
        private static string _lastAppliedDebugSignature;
        
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

            if (!AstralumEntry.EnableDebugTweaks)
                return;
            
            EnsureInitializedFromCurrentStar();
            ApplyTweaks();
        }
        
        public static void ResetTweaksFromCurrentStar()
        {
            if (!AstralumEntry.EnableDebugTweaks)
                return;
            
            _initializedFromStar = false;
            EnsureInitializedFromCurrentStar();
        }

        private static void ApplyTweaks()
        {
            SavedStar star = WorldUtils.CurrentStar;

            if (star == null)
                return;
            
            string signature =
                $"{ChromaColR:F4}|{ChromaColG:F4}|{ChromaColB:F4}|" +
                $"{CoronaColR:F4}|{CoronaColG:F4}|{CoronaColB:F4}|" +
                $"{SurfaceNoiseStrength:F4}|{CoronaRotationSpeed:F4}|" +
                $"{ChromaticityIntensity:F4}|{CoronaIntensity:F4}|{OuterCoronaIntensity:F4}|" +
                $"{ChromaticityFalloffPower:F4}|{CoronaPower:F4}|{OuterCoronaPower:F4}|" +
                $"{VariabilityAmount:F4}|{VariabilitySpeed:F4}";
            
            if (signature == _lastAppliedDebugSignature)
                return;
            
            _lastAppliedDebugSignature = signature;
            
            star.chromaticity = new Color(ChromaColR, ChromaColG, ChromaColB, 1f);
            star.corona = new Color(CoronaColR, CoronaColG, CoronaColB, 1f);
            star.rotation = CoronaRotationSpeed;
            star.chromaticityIntensity = ChromaticityIntensity;
            star.coronaIntensity = CoronaIntensity;
            star.outerCoronaIntensity = OuterCoronaIntensity;
            star.chromaticityFalloffPower = ChromaticityFalloffPower;
            star.coronaPower = CoronaPower;
            star.outerCoronaPower = OuterCoronaPower;
            star.surfaceNoiseStrength = SurfaceNoiseStrength;
            star.variabilityAmount = VariabilityAmount;
            star.variabilitySpeed = VariabilitySpeed;
            
            StarMaterialsUtil.RefreshSun01Mat();
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