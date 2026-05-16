using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class ConstellationHoverMatsUtil
  {
    public static readonly Material Ring;


    static ConstellationHoverMatsUtil()
    {
      Shader shader = InternalDefOf.Astra_ConstellationHoverRing01.Shader;

      Ring = new Material(shader)
      {
        name = "Astralum_ConstellationHoverRing01"
      };

      Ring.SetColor(ShaderPropertyIDs.Color, new Color(0.65f, 0.85f, 1f, 0.85f));
      Ring.SetFloat(InternalShaderPropertyIds.Intensity, 1.35f);
      Ring.SetFloat(InternalShaderPropertyIds.RingRadius, 0.42f);
      Ring.SetFloat(InternalShaderPropertyIds.RingThickness, 0.045f);
      Ring.SetFloat(InternalShaderPropertyIds.PulseSpeed, 1.5f);
      Ring.SetFloat(InternalShaderPropertyIds.PulseStrength, 0.35f);
      Ring.SetFloat(InternalShaderPropertyIds.AlphaPulseMin, 0.4f);
      Ring.SetFloat(InternalShaderPropertyIds.PulseTime, 0f);

      Object.DontDestroyOnLoad(Ring);
    }
  }
}