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
      var shader = InternalDefOf.Astra_BlackHole01.Shader;
      var material = new Material(shader)
      {
        name = $"Astralum_BlackHole01_{id}"
      };
      
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