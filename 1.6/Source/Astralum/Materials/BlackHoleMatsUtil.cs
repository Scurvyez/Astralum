using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class BlackHoleMatsUtil
  {
    public static readonly Material BlackHole;
    

    static BlackHoleMatsUtil()
    {
      Shader shader = InternalDefOf.Astra_BlackHole01.Shader;
      
      BlackHole = new Material(shader)
      {
        name = "Astralum_BlackHole01"
      };
      
      BlackHole.SetFloat(InternalShaderPropertyIds.EffectActive, 1f);
      BlackHole.SetFloat(InternalShaderPropertyIds.CanvasScale, 2.4f);
      BlackHole.SetFloat(InternalShaderPropertyIds.ScreenEdgeFadeStart, 0.01f);
      BlackHole.SetFloat(InternalShaderPropertyIds.ScreenEdgeFadeEnd, 0.08f);
      
      BlackHole.SetFloat(InternalShaderPropertyIds.Radius, 0.16f);
      BlackHole.SetFloat(InternalShaderPropertyIds.DistortionRadius, 1.0f);
      BlackHole.SetFloat(InternalShaderPropertyIds.DistortionStrength, 0.035f);
      BlackHole.SetFloat(InternalShaderPropertyIds.Darkness, 1f);
      
      BlackHole.SetFloat(InternalShaderPropertyIds.HorizonFeather, 0.025f);
      BlackHole.SetFloat(InternalShaderPropertyIds.DistortionFeather, 0.16f);
      BlackHole.SetFloat(InternalShaderPropertyIds.RingFeather, 0.08f);
      
      Object.DontDestroyOnLoad(BlackHole);
    }
  }
}