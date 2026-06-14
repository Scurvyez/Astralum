using System.Collections.Generic;
using Astralum.Debugging;
using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  public static class GalacticDustLaneMatsUtil
  {
    private static readonly Dictionary<int, Material> MaterialsByIndex = [];
    
    public static Material For(int index)
    {
      if (MaterialsByIndex.TryGetValue(index, out Material material))
        return material;
      
      material = CreateMaterial(index);
      MaterialsByIndex[index] = material;
      
      return material;
    }
    
    private static Material CreateMaterial(int index)
    {
      ShaderTypeDef shaderDef = InternalDefOf.Astra_GalacticDustLane01;
      
      if (shaderDef?.Shader == null)
      {
        AstraLog.Warning("Astra_GalacticDustLane01 shader is missing.");
        return null;
      }
      
      Material material = new(shaderDef.Shader)
      {
        name = $"Astralum_GalacticDustLane01_{index}"
      };
      
      Object.DontDestroyOnLoad(material);
      return material;
    }
    
    public static void RandomDustPalette(out Color colorA, out Color colorB)
    {
      int palette = Rand.RangeInclusive(0, 4);
      
      switch (palette)
      {
        case 0:
          colorA = new Color(0.020f, 0.020f, 0.032f, 1f);
          colorB = new Color(0.080f, 0.060f, 0.045f, 1f);
          break;
        
        case 1:
          colorA = new Color(0.018f, 0.026f, 0.045f, 1f);
          colorB = new Color(0.050f, 0.075f, 0.110f, 1f);
          break;
        
        case 2:
          colorA = new Color(0.030f, 0.024f, 0.020f, 1f);
          colorB = new Color(0.095f, 0.070f, 0.045f, 1f);
          break;
        
        case 3:
          colorA = new Color(0.022f, 0.018f, 0.028f, 1f);
          colorB = new Color(0.070f, 0.050f, 0.085f, 1f);
          break;
        
        default:
          colorA = new Color(0.018f, 0.022f, 0.026f, 1f);
          colorB = new Color(0.060f, 0.065f, 0.070f, 1f);
          break;
      }
    }
  }
}