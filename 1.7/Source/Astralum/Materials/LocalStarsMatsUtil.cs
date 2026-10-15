using System.Collections.Generic;
using Astralum.Astronomy.LocalStars;
using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class LocalStarsMatsUtil
  {
    private static readonly Dictionary<string, Material> MaterialsById = [];
    
    public static Material For(SavedLocalStar star)
    {
      if (star == null)
        return null;
      
      if (MaterialsById.TryGetValue(star.Id, out Material material))
      {
        return material;
      }
      
      material = CreateMaterial(star);
      MaterialsById[star.Id] = material;
      
      return material;
    }
    
    private static Material CreateMaterial(SavedLocalStar star)
    {
      Shader shader = InternalDefOf.Astra_LocalStar01.Shader;
      
      Material material = new Material(shader)
      {
        name = $"Astralum_LocalStar01_{star.Id}"
      };
      
      ApplyStarProperties(material, star);
      Object.DontDestroyOnLoad(material);
      
      return material;
    }
    
    private static void ApplyStarProperties(Material material, SavedLocalStar star)
    {
      material.SetColor(InternalShaderPropertyIds.Chromaticity, star.chromaticity);
      material.SetColor(InternalShaderPropertyIds.Corona, star.corona);
      material.SetFloat(InternalShaderPropertyIds.CoronaRotationSpeed, star.rotationsPerDay);
      material.SetFloat(InternalShaderPropertyIds.ChromaticityIntensity, star.chromaticityIntensity);
      material.SetFloat(InternalShaderPropertyIds.CoronaIntensity, star.coronaIntensity);
      material.SetFloat(InternalShaderPropertyIds.OuterCoronaIntensity, star.outerCoronaIntensity);
      material.SetFloat(InternalShaderPropertyIds.ChromaticityFalloffPower, star.chromaticityFalloffPower);
      material.SetFloat(InternalShaderPropertyIds.CoronaPower, star.coronaPower);
      material.SetFloat(InternalShaderPropertyIds.OuterCoronaPower, star.outerCoronaPower);
      material.SetFloat(InternalShaderPropertyIds.SurfaceNoiseStrength, star.surfaceNoiseStrength);
      material.SetFloat(InternalShaderPropertyIds.VariabilityAmount, star.variabilityAmount);
      material.SetFloat(InternalShaderPropertyIds.VariabilitySpeed, star.variabilitySpeed);
      
      material.SetFloat(InternalShaderPropertyIds.FocusShimmer, 0f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerSpeed, 0.2f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerWidth, 0.0025f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerSoftness, 0.3f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerIntensity, 1f);
    }
    
    public static void Clear()
    {
      foreach (Material material in MaterialsById.Values)
      {
        if (material != null)
          Object.Destroy(material);
      }
      
      MaterialsById.Clear();
    }
    
    public static void Refresh(SavedLocalStar star)
    {
      if (star == null)
        return;
      
      if (!MaterialsById.TryGetValue(star.Id, out Material material))
      {
        return;
      }
      
      ApplyStarProperties(material, star);
    }
    
    public static void SetFocused(SavedLocalStar localStar, bool focused)
    {
      if (localStar == null)
        return;
      
      Material material = For(localStar);
      
      if (material == null)
        return;
      
      material.SetFloat(InternalShaderPropertyIds.FocusShimmer, focused ? 1f : 0f);
    }
  }
}