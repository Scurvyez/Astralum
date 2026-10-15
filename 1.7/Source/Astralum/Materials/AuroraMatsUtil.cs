using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class AuroraMatsUtil
  {
    public static Material AuroraMaterial { get; }
    
    static AuroraMatsUtil()
    {
      var shader = InternalDefOf.Astra_Aurora01.Shader;
      
      AuroraMaterial = new Material(shader) 
      {
        name = "Astralum_PlanetaryAurora"
      };
      
      AuroraMaterial.SetFloat(InternalShaderPropertyIds.AuroraActive, 0f);
      AuroraMaterial.SetFloat(InternalShaderPropertyIds.Seed, 0f);
    }
  }
}