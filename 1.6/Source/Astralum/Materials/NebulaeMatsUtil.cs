using System.Collections.Generic;
using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class NebulaeMatsUtil
  {
    private static readonly Dictionary<int, Material> MaterialsByNebulaIndex = [];

    public static Material For(int nebulaIndex)
    {
      if (MaterialsByNebulaIndex.TryGetValue(nebulaIndex, out Material material))
        return material;

      material = CreateNebulaMaterial(nebulaIndex);
      MaterialsByNebulaIndex[nebulaIndex] = material;

      return material;
    }

    private static Material CreateNebulaMaterial(int nebulaIndex)
    {
      Shader shader = InternalDefOf.Astra_Nebulae01.Shader;

      Material material = new(shader)
      {
        name = $"Astralum_Astra_Nebulae01_{nebulaIndex}"
      };

      Object.DontDestroyOnLoad(material);
      return material;
    }

    public static void Clear()
    {
      foreach (Material material in MaterialsByNebulaIndex.Values)
        if (material != null)
          Object.Destroy(material);

      MaterialsByNebulaIndex.Clear();
    }
  }
}