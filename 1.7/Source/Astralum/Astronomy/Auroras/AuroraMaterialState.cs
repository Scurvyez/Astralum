using Astralum.Materials;
using RimWorld;
using UnityEngine;

namespace Astralum.Astronomy.Auroras
{
  public static class AuroraMaterialState
  {
    public static void Update(Material material)
    {
      if (material == null)
        return;
      
      if (!AuroraRenderUtil.TryGetActiveAurora(out GameCondition_Aurora aurora))
      {
        material.SetFloat(InternalShaderPropertyIds.AuroraActive, 0f);
        return;
      }
      
      AuroraRenderUtil.GetGradientColors(aurora, out Color bottom, out Color middle, out Color top);
      
      material.SetFloat(InternalShaderPropertyIds.AuroraActive, 1f);
      material.SetColor(InternalShaderPropertyIds.AuroraColorBottom, bottom);
      material.SetColor(InternalShaderPropertyIds.AuroraColorMiddle, middle);
      material.SetColor(InternalShaderPropertyIds.AuroraColorTop, top);
    }
  }
}