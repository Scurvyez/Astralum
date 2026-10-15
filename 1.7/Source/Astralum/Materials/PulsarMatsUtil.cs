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
    private static readonly Dictionary<string, Material> MaterialsById = [];
    
    public static Material For(SavedPulsar pulsar)
    {
      return pulsar == null 
        ? null 
        : For(pulsar.Id);
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
      var shader = InternalDefOf.Astra_Pulsar01.Shader;
      
      Material material = new(shader)
      {
        name = $"Astralum_Pulsar01_{id}"
      };
      
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