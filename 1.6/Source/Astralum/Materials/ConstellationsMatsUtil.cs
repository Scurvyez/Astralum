using System.Collections.Generic;
using Astralum.Astronomy.Constellations;
using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class ConstellationsMatsUtil
  {
    private static readonly Dictionary<Texture2D, Material> MaterialsByTexture = new();

    public static Material For(Texture2D texture)
    {
      if (texture == null)
        return null;

      if (MaterialsByTexture.TryGetValue(texture, out Material material))
        return material;

      Shader shader = InternalDefOf.Astra_Constellation01.Shader;

      material = new Material(shader)
      {
        name = $"Astralum_ConstellationTexture01_{texture.name}",
        mainTexture = texture
      };
      
      material.SetTexture(InternalShaderPropertyIds.MainTex, texture);
      material.SetColor(ShaderPropertyIDs.Color, new Color(0.45f, 0.60f, 1.0f, 0.35f));
      material.SetFloat(InternalShaderPropertyIds.Intensity, 0.875f);
      material.SetFloat(InternalShaderPropertyIds.BlurStrength, 0.45f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmer, 0f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerSpeed, 0.3f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerWidth, 0.005f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerSoftness, 0.2f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerIntensity, 2f);
      
      Object.DontDestroyOnLoad(material);

      MaterialsByTexture[texture] = material;
      return material;
    }
    
    public static void SetFocused(SavedConstellation constellation, bool focused)
    {
      if (constellation == null)
        return;
      
      Texture2D mask = ConstellationMaskUtil.GetMaskByName(constellation.maskName);
      
      if (mask == null)
        return;
      
      Material material = For(mask);
      
      if (material == null)
        return;
      
      material.SetFloat(InternalShaderPropertyIds.FocusShimmer, focused ? 1f : 0f);
    }
    
    public static void Clear()
    {
      foreach (Material material in MaterialsByTexture.Values)
        if (material != null)
          Object.Destroy(material);
      
      MaterialsByTexture.Clear();
    }
  }
}