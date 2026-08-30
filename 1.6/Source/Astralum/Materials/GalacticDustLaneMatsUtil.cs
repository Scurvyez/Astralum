using System.Collections.Generic;
using Astralum.Astronomy.GalacticDustLanes;
using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class GalacticDustLaneMatsUtil
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
    
    private static Material CreateMaterial(string id)
    {
      Shader shader = InternalDefOf.Astra_GalacticDustLane01.Shader;
      
      Material material = new(shader)
      {
        name = $"Astralum_GalacticDustLane01_{id}"
      };
      
      Object.DontDestroyOnLoad(material);
      return material;
    }
    
    public static void Clear()
    {
      foreach (Material material in MaterialsById.Values)
        if (material != null)
          Object.Destroy(material);

      MaterialsById.Clear();
    }

    private static void RandomDustPalette(out Color colorA, out Color colorB)
    {
      int palette = Rand.RangeInclusive(0, 4);
      
      switch (palette)
      {
        case 0: // cool blue-gray
          colorA = new Color(0.08f, 0.10f, 0.14f, 1f);
          colorB = new Color(0.18f, 0.22f, 0.28f, 1f);
          break;
        
        case 1: // blue
          colorA = new Color(0.06f, 0.10f, 0.18f, 1f);
          colorB = new Color(0.14f, 0.22f, 0.35f, 1f);
          break;
        
        case 2: // brown dust
          colorA = new Color(0.10f, 0.08f, 0.06f, 1f);
          colorB = new Color(0.24f, 0.18f, 0.12f, 1f);
          break;
        
        case 3: // purple
          colorA = new Color(0.08f, 0.05f, 0.12f, 1f);
          colorB = new Color(0.20f, 0.12f, 0.28f, 1f);
          break;
        
        default: // neutral gray
          colorA = new Color(0.10f, 0.10f, 0.10f, 1f);
          colorB = new Color(0.22f, 0.22f, 0.22f, 1f);
          break;
      }
    }

    public static void ApplyToMaterial(Material mat, SavedGalacticDustLane dustLane)
    {
      if (mat == null || dustLane == null)
        return;
      
      RandomDustPalette(out var colorA, out var colorB);
      
      mat.SetColor(InternalShaderPropertyIds.ColorA, colorA);
      mat.SetColor(InternalShaderPropertyIds.ColorB, colorB);
      mat.SetFloat(InternalShaderPropertyIds.Alpha, dustLane.alphaRange.RandomInRange);
      mat.SetFloat(InternalShaderPropertyIds.Intensity, dustLane.intensityRange.RandomInRange);
      mat.SetFloat(InternalShaderPropertyIds.CanvasScale, 1f);
      mat.SetFloat(InternalShaderPropertyIds.NoiseScale, dustLane.noiseScaleRange.RandomInRange);
      mat.SetFloat(InternalShaderPropertyIds.NoiseStrength, Rand.Range(0.55f, 0.9f));
      mat.SetFloat(InternalShaderPropertyIds.DetailScale, dustLane.detailScaleRange.RandomInRange);
      mat.SetFloat(InternalShaderPropertyIds.DetailStrength, Rand.Range(0.15f, 0.45f));
      mat.SetFloat(InternalShaderPropertyIds.CloudThreshold, Rand.Range(0.34f, 0.58f));
      mat.SetFloat(InternalShaderPropertyIds.EdgeSoftness, Rand.Range(0.24f, 0.48f));
      mat.SetFloat(InternalShaderPropertyIds.EdgeFadeStart, 0.02f);
      mat.SetFloat(InternalShaderPropertyIds.EdgeFadeEnd, 0.18f);
      mat.SetFloat(InternalShaderPropertyIds.StretchX, dustLane.stretchXRange.RandomInRange);
      mat.SetFloat(InternalShaderPropertyIds.StretchY, dustLane.stretchYRange.RandomInRange);
      mat.SetFloat(InternalShaderPropertyIds.Rotation, Rand.Range(-0.05f, 0.05f));
      
      mat.SetVector(InternalShaderPropertyIds.SeedOffset, new Vector4(
        Rand.Range(-1000f, 1000f),
        Rand.Range(-1000f, 1000f),
        Rand.Range(-1000f, 1000f),
        Rand.Range(-1000f, 1000f)
      ));
    }
  }
}