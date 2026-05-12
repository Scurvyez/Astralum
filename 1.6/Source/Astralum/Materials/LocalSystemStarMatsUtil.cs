using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.Debugging;
using Astralum.DefOfs;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
    [StaticConstructorOnStartup]
    public static class LocalSystemStarMatsUtil
    {
        private static Material _star01Mat;
        private static bool _star01MatDirty = true;
        
        public static Material Star01Mat
        {
            get
            {
                if (_star01Mat == null)
                {
                    _star01Mat = CreateSun01Mat();
                    _star01MatDirty = true;
                }
                
                ApplyCurrentStarToMaterialIfDirty(_star01Mat);
                return _star01Mat;
            }
        }

        public static void MarkSun01MatDirty()
        {
            _star01MatDirty = true;
        }
        
        public static void RefreshSun01Mat()
        {
            MarkSun01MatDirty();
            
            if (_star01Mat != null)
                ApplyCurrentStarToMaterialIfDirty(_star01Mat);
        }
        
        public static void ForceRefreshSun01Mat()
        {
            if (_star01Mat == null)
                return;
            
            ApplyCurrentStarToMaterial(_star01Mat);
            _star01MatDirty = false;
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
            
            Object.DontDestroyOnLoad(mat);
            return mat;
        }
        
        private static void ApplyCurrentStarToMaterialIfDirty(Material mat)
        {
            if (!_star01MatDirty)
                return;
            
            ApplyCurrentStarToMaterial(mat);
            _star01MatDirty = false;
        }
        
        private static void ApplyCurrentStarToMaterial(Material mat)
        {
            SavedStar star = World.WorldUtils.CurrentStar;
            
            if (star == null)
            {
                ApplyFallbackStar(mat);
                return;
            }
            
            mat.SetColor(InternalShaderPropertyIds.Chromaticity, star.chromaticity);
            mat.SetColor(InternalShaderPropertyIds.Corona, star.corona);
            
            mat.SetFloat(InternalShaderPropertyIds.CoronaRotationSpeed, star.rotation);
            
            float chromaticityIntensity =
                star.chromaticityIntensity *
                StellarChromaticityUtil.GetChromaticityIntensitySpectralFactor(star.spectralClass);
            
            mat.SetFloat(InternalShaderPropertyIds.ChromaticityIntensity, chromaticityIntensity);
            mat.SetFloat(InternalShaderPropertyIds.CoronaIntensity, star.coronaIntensity);
            mat.SetFloat(InternalShaderPropertyIds.OuterCoronaIntensity, star.outerCoronaIntensity);
            
            mat.SetFloat(InternalShaderPropertyIds.ChromaticityFalloffPower, star.chromaticityFalloffPower);
            mat.SetFloat(InternalShaderPropertyIds.CoronaPower, star.coronaPower);
            mat.SetFloat(InternalShaderPropertyIds.OuterCoronaPower, star.outerCoronaPower);
            mat.SetFloat(InternalShaderPropertyIds.SurfaceNoiseStrength, star.surfaceNoiseStrength);
            
            bool intrinsicVariable =
                star.variabilityType == StellarVariabilityUtil.StellarVariabilityType.Intrinsic &&
                star.variabilityAmount > 0f;
            
            mat.SetFloat(InternalShaderPropertyIds.VariabilityAmount,
                intrinsicVariable ? star.variabilityAmount : 0f);
            
            mat.SetFloat(InternalShaderPropertyIds.VariabilitySpeed, 
                intrinsicVariable ? star.variabilitySpeed : 0f);
        }
        
        private static void ApplyFallbackStar(Material mat)
        {
            AstraLog.Warning("No saved world star found. Using fallback G-class values.");
            
            mat.SetColor(InternalShaderPropertyIds.Chromaticity, new Color(1f, 0.93f, 0.89f, 1f));
            mat.SetColor(InternalShaderPropertyIds.Corona, new Color(1f, 0.82f, 0.47f, 1f));
            
            mat.SetFloat(InternalShaderPropertyIds.CoronaRotationSpeed, 0.5f);
            mat.SetFloat(InternalShaderPropertyIds.ChromaticityIntensity, 1f);
            mat.SetFloat(InternalShaderPropertyIds.CoronaIntensity, 1f);
            mat.SetFloat(InternalShaderPropertyIds.OuterCoronaIntensity, 0.25f);
            
            mat.SetFloat(InternalShaderPropertyIds.ChromaticityFalloffPower, 2f);
            mat.SetFloat(InternalShaderPropertyIds.CoronaPower, 5f);
            mat.SetFloat(InternalShaderPropertyIds.OuterCoronaPower, 6f);
            
            mat.SetFloat(InternalShaderPropertyIds.VariabilityAmount, 0f);
            mat.SetFloat(InternalShaderPropertyIds.VariabilitySpeed, 0f);
        }
    }
}