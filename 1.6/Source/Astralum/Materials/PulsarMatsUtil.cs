using System.Collections.Generic;
using Astralum.Astronomy.Pulsars;
using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public class PulsarMatsUtil
  {
    const float PulsarCanvasScale = 2f;
    
    private static readonly Dictionary<string, Material> MaterialsById = [];
    
    public static Material For(SavedPulsar pulsar)
    {
      if (pulsar == null)
        return null;
      
      return For(pulsar.Id);
    }
    
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
    
    private static Material CreateMaterial(string id)
    {
      Shader shader = InternalDefOf.Astra_Pulsar01.Shader;
      
      Material material = new(shader)
      {
        name = $"Astralum_Pulsar01_{id}"
      };
      
      material.SetFloat(InternalShaderPropertyIds.CanvasScale, PulsarCanvasScale);
      material.SetColor(InternalShaderPropertyIds.ShellDarkColor, new Color(0.01f, 0.025f, 0.075f, 1f));
      material.SetColor(InternalShaderPropertyIds.ShellBrightColor, new Color(0.12f, 0.45f, 1.0f, 1f));
      material.SetColor(InternalShaderPropertyIds.CoreColor, Color.white);
      material.SetColor(InternalShaderPropertyIds.JetColor, new Color(0.55f, 0.85f, 1.0f, 1f));
      material.SetFloat(InternalShaderPropertyIds.Intensity, 1.1f);
      material.SetFloat(InternalShaderPropertyIds.Alpha, 0.82f);
      
      // Core
      material.SetVector(InternalShaderPropertyIds.CoreOffset, new Vector4(0.25f, 0f, 0f, 0f));
      material.SetFloat(InternalShaderPropertyIds.CoreRadius, 0.01f);
      material.SetFloat(InternalShaderPropertyIds.CoreGlowRadius, 0.055f);
      material.SetFloat(InternalShaderPropertyIds.CoreIntensity, 1.35f);
      material.SetFloat(InternalShaderPropertyIds.CorePulseSpeed, 0.8f);
      material.SetFloat(InternalShaderPropertyIds.CorePulseStrength, 0.18f);
      
      // Shell
      material.SetFloat(InternalShaderPropertyIds.ShellRadius, 0.33f);
      material.SetFloat(InternalShaderPropertyIds.ShellThickness, 0.095f);
      material.SetFloat(InternalShaderPropertyIds.ShellSoftness, 0.64f);
      material.SetFloat(InternalShaderPropertyIds.ShellPower, 3.75f);
      material.SetFloat(InternalShaderPropertyIds.ShellCoverage, 0.91f);
      material.SetFloat(InternalShaderPropertyIds.InnerCrescentRadiusOffset, -0.11f);
      material.SetFloat(InternalShaderPropertyIds.InnerCrescentThickness, 0.055f);
      material.SetFloat(InternalShaderPropertyIds.InnerCrescentSoftness, 0.2f);
      material.SetFloat(InternalShaderPropertyIds.InnerCrescentIntensity, 1.4f);
      
      // Inner distorted/dim band
      material.SetFloat(InternalShaderPropertyIds.BandRadiusOffset, -0.004f);
      material.SetFloat(InternalShaderPropertyIds.BandThickness, 0.003f);
      material.SetFloat(InternalShaderPropertyIds.BandIntensity, 0.09f);
      material.SetFloat(InternalShaderPropertyIds.BandSoftness, 0.45f);
      
      // Jet
      material.SetFloat(InternalShaderPropertyIds.JetLength, 1.9f);
      material.SetFloat(InternalShaderPropertyIds.JetWidth, 0.036f);
      material.SetFloat(InternalShaderPropertyIds.JetSpread, 0.075f);
      material.SetFloat(InternalShaderPropertyIds.JetIntensity, 0.92f);
      material.SetFloat(InternalShaderPropertyIds.JetFalloff, 1.25f);
      material.SetFloat(InternalShaderPropertyIds.JetFlicker, 0.03f);
      material.SetFloat(InternalShaderPropertyIds.JetFlickerSpeed, 0.5f);
      material.SetFloat(InternalShaderPropertyIds.JetSoftness, 0.035f);
      
      // Concave dust
      material.SetFloat(InternalShaderPropertyIds.DustIntensity, 0.48f);
      material.SetFloat(InternalShaderPropertyIds.DustAmount, 0.42f);
      material.SetFloat(InternalShaderPropertyIds.DustSpread, 0.28f);
      
      // Texture
      material.SetFloat(InternalShaderPropertyIds.NoiseScale, 7f);
      material.SetFloat(InternalShaderPropertyIds.NoiseStrength, 0.38f);
      material.SetFloat(InternalShaderPropertyIds.DetailScale, 14f);
      material.SetFloat(InternalShaderPropertyIds.DetailStrength, 0.12f);
      
      material.SetFloat(InternalShaderPropertyIds.FocusShimmer, 0f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerSpeed, 0.3f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerWidth, 0.005f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerSoftness, 0.15f);
      material.SetFloat(InternalShaderPropertyIds.FocusShimmerIntensity, 1f);
      
      Object.DontDestroyOnLoad(material);
      
      return material;
    }
    
    public static void SetFocused(SavedPulsar pulsar, bool focused)
    {
      if (pulsar == null)
        return;
      
      Material material = For(pulsar);
      
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