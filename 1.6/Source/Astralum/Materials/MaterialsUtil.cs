using Astralum.Astronomy.Stars;
using Astralum.Debugging;
using Astralum.DefOfs;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
    [StaticConstructorOnStartup]
    public static class MaterialsUtil
    {
        private static Material _sun01Mat;
        private static string _lastAppliedStarSignature;
        
        public static Material Sun01Mat
        {
            get
            {
                if (_sun01Mat != null)
                    return _sun01Mat;
                
                _sun01Mat = CreateSun01Mat();
                
                return _sun01Mat;
            }
        }
        
        public static void RefreshSun01Mat()
        {
            if (_sun01Mat == null)
                return;
            
            ApplyCurrentStarToMaterial(_sun01Mat, force: true);
        }
        
        private static Material CreateSun01Mat()
        {
            Shader shader = InternalDefOf.Astra_Sun01.Shader;

            if (shader == null)
            {
                AstraLog.Warning("Could not find shader for Astra_Sun01. Falling back to WorldMaterials.Sun.");
                return WorldMaterials.Sun;
            }
            
            Material mat = new(shader)
            {
                name = "Astralum_Sun01",
                renderQueue = WorldMaterials.Sun.renderQueue
            };
            
            ApplyCurrentStarToMaterial(mat, force: true);
            Object.DontDestroyOnLoad(mat);
            return mat;
        }
        
        private static void ApplyCurrentStarToMaterial(Material mat, bool force = false)
        {
            SavedStar star = World.WorldUtils.CurrentStar;
            
            if (star == null)
            {
                ApplyFallbackStar(mat);
                return;
            }
            
            string signature =
                $"{star.chromaticity.r:0.000}_{star.chromaticity.g:0.000}_{star.chromaticity.b:0.000}_" +
                $"{star.coronaColor.r:0.000}_{star.coronaColor.g:0.000}_{star.coronaColor.b:0.000}_" +
                $"{star.coronaIntensity:0.000}_{star.glowPower:0.000}";
            
            if (!force && signature == _lastAppliedStarSignature)
                return;
            
            _lastAppliedStarSignature = signature;
            
            mat.SetColor(InternalShaderPropertyIds.Chromaticity, star.chromaticity);
            mat.SetColor(InternalShaderPropertyIds.Corona, star.coronaColor);
            
            mat.SetFloat(InternalShaderPropertyIds.ChromaticityIntensity, 1f);
            mat.SetFloat(InternalShaderPropertyIds.CoronaIntensity, star.coronaIntensity);
            
            mat.SetFloat(InternalShaderPropertyIds.ChromaticityFalloffPower, 2f);
            mat.SetFloat(InternalShaderPropertyIds.RadiusPower, star.radiusPower);
            mat.SetFloat(InternalShaderPropertyIds.CoronaPower, star.glowPower);
            
            bool intrinsicVariable =
                star.variabilityType == StellarVariabilityUtil.StellarVariabilityType.Intrinsic &&
                star.variabilityAmount > 0f;
            
            mat.SetFloat(InternalShaderPropertyIds.VariabilityAmount,
                intrinsicVariable ? star.variabilityAmount : 0f);
            
            mat.SetFloat(InternalShaderPropertyIds.VariabilitySpeed,
                intrinsicVariable ? 1f : 0f);
            
            AstraLog.Message($"Applied world star to material. Class={star.spectralClass}");
        }
        
        private static void ApplyFallbackStar(Material mat)
        {
            const string fallbackSignature = "Fallback_G";
            
            if (_lastAppliedStarSignature == fallbackSignature)
                return;
            
            _lastAppliedStarSignature = fallbackSignature;
            
            AstraLog.Warning("No saved world star found. Using fallback G-class values.");
            
            mat.SetColor(InternalShaderPropertyIds.Chromaticity, new Color(1f, 0.93f, 0.89f, 1f));
            mat.SetColor(InternalShaderPropertyIds.Corona, new Color(1f, 0.82f, 0.47f, 1f));
            
            mat.SetFloat(InternalShaderPropertyIds.ChromaticityIntensity, 1f);
            mat.SetFloat(InternalShaderPropertyIds.CoronaIntensity, 1f);
            
            mat.SetFloat(InternalShaderPropertyIds.ChromaticityFalloffPower, 2f);
            mat.SetFloat(InternalShaderPropertyIds.RadiusPower, 1f);
            mat.SetFloat(InternalShaderPropertyIds.CoronaPower, 5f);
            mat.SetFloat(InternalShaderPropertyIds.VariabilityAmount, 0f);
            mat.SetFloat(InternalShaderPropertyIds.VariabilitySpeed, 0f);
        }
    }
}