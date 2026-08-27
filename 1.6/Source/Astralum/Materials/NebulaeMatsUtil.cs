using System.Collections.Generic;
using Astralum.Astronomy.Nebulae;
using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class NebulaeMatsUtil
  {
    private static readonly Dictionary<string, Material> MaterialsByIndex = [];

    public static Material For(string id)
    {
      if (MaterialsByIndex.TryGetValue(id, out Material material))
        return material;

      material = CreateMaterial(id);
      MaterialsByIndex[id] = material;

      return material;
    }

    private static Material CreateMaterial(string id)
    {
      Shader shader = InternalDefOf.Astra_Nebulae01.Shader;

      Material material = new(shader)
      {
        name = $"Astralum_Astra_Nebulae01_{id}"
      };
      
      material.SetFloat(InternalShaderPropertyIds.FocusShimmer, 0f);
      
      Object.DontDestroyOnLoad(material);
      return material;
    }
    
    public static void SetFocused(SavedNebula nebula, bool focused)
    {
      if (nebula == null)
        return;
      
      Material material = For(nebula.Id);
      
      if (material == null)
        return;
      
      material.SetFloat(InternalShaderPropertyIds.FocusShimmer, focused ? 1f : 0f);
    }
    
    public static void ApplyToMaterial(Material mat, SavedNebula nebula)
    {
      if (mat == null || nebula == null)
        return;

      mat.SetColor(InternalShaderPropertyIds.ColorA, nebula.colorA);
      mat.SetColor(InternalShaderPropertyIds.ColorB, nebula.colorB);
      mat.SetColor(InternalShaderPropertyIds.ColorC, nebula.colorC);
      mat.SetColor(InternalShaderPropertyIds.ColorD, nebula.colorD);

      mat.SetFloat(InternalShaderPropertyIds.ColorStopB, nebula.colorStopB);
      mat.SetFloat(InternalShaderPropertyIds.ColorStopC, nebula.colorStopC);
      mat.SetFloat(InternalShaderPropertyIds.ColorBandSharpness, nebula.colorBandSharpness);

      mat.SetVector(InternalShaderPropertyIds.SeedOffset, nebula.seedOffset);
      mat.SetFloat(InternalShaderPropertyIds.Seed, nebula.seed);

      mat.SetFloat(InternalShaderPropertyIds.Intensity, nebula.intensity);
      mat.SetFloat(InternalShaderPropertyIds.Alpha, nebula.alpha);

      mat.SetFloat(InternalShaderPropertyIds.NoiseScale, nebula.noiseScale);
      mat.SetFloat(InternalShaderPropertyIds.NoiseStrength, nebula.noiseStrength);

      mat.SetFloat(InternalShaderPropertyIds.CloudThreshold, nebula.cloudThreshold);
      mat.SetFloat(InternalShaderPropertyIds.EdgeSoftness, nebula.edgeSoftness);

      mat.SetFloat(InternalShaderPropertyIds.WarpScale, nebula.warpScale);
      mat.SetFloat(InternalShaderPropertyIds.WarpStrength, nebula.warpStrength);
      mat.SetFloat(InternalShaderPropertyIds.ShapePower, nebula.shapePower);
      mat.SetVector(InternalShaderPropertyIds.CoreOffset, nebula.coreOffset);

      mat.SetFloat(InternalShaderPropertyIds.StretchX, nebula.stretchX);
      mat.SetFloat(InternalShaderPropertyIds.StretchY, nebula.stretchY);
      mat.SetFloat(InternalShaderPropertyIds.Rotation, nebula.shaderRotation);
    }
    
    public static void Clear()
    {
      foreach (Material material in MaterialsByIndex.Values)
        if (material != null)
          Object.Destroy(material);

      MaterialsByIndex.Clear();
    }
  }
}