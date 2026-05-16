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

      Pulsar.SetFloat("_CanvasScale", PulsarCanvasScale);

      Pulsar.SetColor("_ShellDarkColor", new Color(0.01f, 0.025f, 0.075f, 1f));
      Pulsar.SetColor("_ShellBrightColor", new Color(0.12f, 0.45f, 1.0f, 1f));
      Pulsar.SetColor("_CoreColor", Color.white);
      Pulsar.SetColor("_JetColor", new Color(0.55f, 0.85f, 1.0f, 1f));

      Pulsar.SetFloat("_Intensity", 1.1f);
      Pulsar.SetFloat("_Alpha", 0.82f);

      // Core
      Pulsar.SetVector("_CoreOffset", new Vector4(0.25f, 0f, 0f, 0f));
      Pulsar.SetFloat("_CoreRadius", 0.01f);
      Pulsar.SetFloat("_CoreGlowRadius", 0.055f);
      Pulsar.SetFloat("_CoreIntensity", 1.35f);
      Pulsar.SetFloat("_CorePulseSpeed", 0.8f);
      Pulsar.SetFloat("_CorePulseStrength", 0.18f);

      // Shell
      Pulsar.SetFloat("_ShellRadius", 0.33f);
      Pulsar.SetFloat("_ShellThickness", 0.095f);
      Pulsar.SetFloat("_ShellSoftness", 0.64f);
      Pulsar.SetFloat("_ShellPower", 3.75f);
      Pulsar.SetFloat("_ShellCoverage", 0.91f);
      Pulsar.SetFloat("_InnerCrescentRadiusOffset", -0.11f);
      Pulsar.SetFloat("_InnerCrescentThickness", 0.055f);
      Pulsar.SetFloat("_InnerCrescentSoftness", 0.2f);
      Pulsar.SetFloat("_InnerCrescentIntensity", 1.4f);

      // Inner distorted/dim band
      Pulsar.SetFloat("_BandRadiusOffset", -0.004f);
      Pulsar.SetFloat("_BandThickness", 0.003f);
      Pulsar.SetFloat("_BandIntensity", 0.09f);
      Pulsar.SetFloat("_BandSoftness", 0.45f);

      // Jet
      Pulsar.SetFloat("_JetLength", 1.9f);
      Pulsar.SetFloat("_JetWidth", 0.036f);
      Pulsar.SetFloat("_JetSpread", 0.075f);
      Pulsar.SetFloat("_JetIntensity", 0.92f);
      Pulsar.SetFloat("_JetFalloff", 1.25f);
      Pulsar.SetFloat("_JetFlicker", 0.03f);
      Pulsar.SetFloat("_JetFlickerSpeed", 0.5f);
      Pulsar.SetFloat("_JetSoftness", 0.035f);

      // Concave dust
      Pulsar.SetFloat("_DustIntensity", 0.48f);
      Pulsar.SetFloat("_DustAmount", 0.42f);
      Pulsar.SetFloat("_DustSpread", 0.28f);

      // Texture
      Pulsar.SetFloat("_NoiseScale", 7f);
      Pulsar.SetFloat("_NoiseStrength", 0.38f);
      Pulsar.SetFloat("_DetailScale", 14f);
      Pulsar.SetFloat("_DetailStrength", 0.12f);

      Object.DontDestroyOnLoad(Pulsar);
    }
  }
}