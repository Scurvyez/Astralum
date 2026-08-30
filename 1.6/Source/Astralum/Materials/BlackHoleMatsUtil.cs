using System.Collections.Generic;
using Astralum.Astronomy.BlackHoles;
using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class BlackHoleMatsUtil
  {
    private static readonly Dictionary<string, Material> MaterialsById = [];
    
    public static Material For(string id)
    {
      if (id.NullOrEmpty())
        return null;
      
      if (MaterialsById.TryGetValue(id, out Material material))
        return material;
      
      material = CreateMaterial(id);
      MaterialsById[id] = material;
      
      return material;
    }
    
    public static Material For(SavedBlackHole blackHole)
    {
      return blackHole == null 
        ? null 
        : For(blackHole.Id);
    }
    
    private static Material CreateMaterial(string id)
    {
      Shader shader = InternalDefOf.Astra_BlackHole01.Shader;
      
      Material material = new(shader)
      {
        name = $"Astralum_BlackHole01_{id}"
      };
      
      material.SetFloat(InternalShaderPropertyIds.EffectActive, 1f);
      material.SetFloat(InternalShaderPropertyIds.CanvasScale, 2.4f);
      material.SetFloat(InternalShaderPropertyIds.ScreenEdgeFadeStart, 0.01f);
      material.SetFloat(InternalShaderPropertyIds.ScreenEdgeFadeEnd, 0.08f);
      material.SetFloat(InternalShaderPropertyIds.Radius, 0.16f);
      material.SetFloat(InternalShaderPropertyIds.DistortionRadius, 1.0f);
      material.SetFloat(InternalShaderPropertyIds.DistortionStrength, 0.035f);
      material.SetFloat(InternalShaderPropertyIds.Darkness, 1f);
      material.SetFloat(InternalShaderPropertyIds.HorizonFeather, 0.025f);
      material.SetFloat(InternalShaderPropertyIds.DistortionFeather, 0.16f);
      material.SetFloat(InternalShaderPropertyIds.RingFeather, 0.08f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmer, 0f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerSpeed, 0.3f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerWidth, 0.005f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerSoftness, 0.15f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerIntensity, 1f);
      
      Object.DontDestroyOnLoad(material);
      
      return material;
    }
    
    public static void SetFocused(SavedBlackHole blackHole, bool focused)
    {
      if (blackHole == null)
        return;
      
      Material material = For(blackHole);
      
      if (material == null)
        return;
      
      material.SetFloat(InternalShaderPropertyIds.FocusShimmer, focused ? 1f : 0f);
    }
    
    public static void Clear()
    {
      foreach (Material material in MaterialsById.Values)
        if (material != null)
          Object.Destroy(material);
      
      MaterialsById.Clear();
    }
  }
}