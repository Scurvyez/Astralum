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
  }
}