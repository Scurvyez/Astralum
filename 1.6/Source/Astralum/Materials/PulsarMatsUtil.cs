using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public class PulsarMatsUtil
  {
    const float PulsarCanvasScale = 2f;
    
    public static readonly Material Pulsar;
    
    static PulsarMatsUtil()
    {
      Shader shader = InternalDefOf.Astra_Pulsar01.Shader;

      Pulsar = new Material(shader)
      {
        name = "Astralum_Pulsar01"
      };

      Pulsar.SetFloat(InternalShaderPropertyIds.CanvasScale, PulsarCanvasScale);

      Pulsar.SetColor(InternalShaderPropertyIds.ShellDarkColor, new Color(0.01f, 0.025f, 0.075f, 1f));
      Pulsar.SetColor(InternalShaderPropertyIds.ShellBrightColor, new Color(0.12f, 0.45f, 1.0f, 1f));
      Pulsar.SetColor(InternalShaderPropertyIds.CoreColor, Color.white);
      Pulsar.SetColor(InternalShaderPropertyIds.JetColor, new Color(0.55f, 0.85f, 1.0f, 1f));

      Pulsar.SetFloat(InternalShaderPropertyIds.Intensity, 1.1f);
      Pulsar.SetFloat(InternalShaderPropertyIds.Alpha, 0.82f);

      // Core
      Pulsar.SetVector(InternalShaderPropertyIds.CoreOffset, new Vector4(0.25f, 0f, 0f, 0f));
      Pulsar.SetFloat(InternalShaderPropertyIds.CoreRadius, 0.01f);
      Pulsar.SetFloat(InternalShaderPropertyIds.CoreGlowRadius, 0.055f);
      Pulsar.SetFloat(InternalShaderPropertyIds.CoreIntensity, 1.35f);
      Pulsar.SetFloat(InternalShaderPropertyIds.CorePulseSpeed, 0.8f);
      Pulsar.SetFloat(InternalShaderPropertyIds.CorePulseStrength, 0.18f);

      // Shell
      Pulsar.SetFloat(InternalShaderPropertyIds.ShellRadius, 0.33f);
      Pulsar.SetFloat(InternalShaderPropertyIds.ShellThickness, 0.095f);
      Pulsar.SetFloat(InternalShaderPropertyIds.ShellSoftness, 0.64f);
      Pulsar.SetFloat(InternalShaderPropertyIds.ShellPower, 3.75f);
      Pulsar.SetFloat(InternalShaderPropertyIds.ShellCoverage, 0.91f);
      Pulsar.SetFloat(InternalShaderPropertyIds.InnerCrescentRadiusOffset, -0.11f);
      Pulsar.SetFloat(InternalShaderPropertyIds.InnerCrescentThickness, 0.055f);
      Pulsar.SetFloat(InternalShaderPropertyIds.InnerCrescentSoftness, 0.2f);
      Pulsar.SetFloat(InternalShaderPropertyIds.InnerCrescentIntensity, 1.4f);

      // Inner distorted/dim band
      Pulsar.SetFloat(InternalShaderPropertyIds.BandRadiusOffset, -0.004f);
      Pulsar.SetFloat(InternalShaderPropertyIds.BandThickness, 0.003f);
      Pulsar.SetFloat(InternalShaderPropertyIds.BandIntensity, 0.09f);
      Pulsar.SetFloat(InternalShaderPropertyIds.BandSoftness, 0.45f);

      // Jet
      Pulsar.SetFloat(InternalShaderPropertyIds.JetLength, 1.9f);
      Pulsar.SetFloat(InternalShaderPropertyIds.JetWidth, 0.036f);
      Pulsar.SetFloat(InternalShaderPropertyIds.JetSpread, 0.075f);
      Pulsar.SetFloat(InternalShaderPropertyIds.JetIntensity, 0.92f);
      Pulsar.SetFloat(InternalShaderPropertyIds.JetFalloff, 1.25f);
      Pulsar.SetFloat(InternalShaderPropertyIds.JetFlicker, 0.03f);
      Pulsar.SetFloat(InternalShaderPropertyIds.JetFlickerSpeed, 0.5f);
      Pulsar.SetFloat(InternalShaderPropertyIds.JetSoftness, 0.035f);

      // Concave dust
      Pulsar.SetFloat(InternalShaderPropertyIds.DustIntensity, 0.48f);
      Pulsar.SetFloat(InternalShaderPropertyIds.DustAmount, 0.42f);
      Pulsar.SetFloat(InternalShaderPropertyIds.DustSpread, 0.28f);

      // Texture
      Pulsar.SetFloat(InternalShaderPropertyIds.NoiseScale, 7f);
      Pulsar.SetFloat(InternalShaderPropertyIds.NoiseStrength, 0.38f);
      Pulsar.SetFloat(InternalShaderPropertyIds.DetailScale, 14f);
      Pulsar.SetFloat(InternalShaderPropertyIds.DetailStrength, 0.12f);

      Object.DontDestroyOnLoad(Pulsar);
    }
  }
}