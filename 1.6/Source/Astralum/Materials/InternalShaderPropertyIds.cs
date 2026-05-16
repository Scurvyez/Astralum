using UnityEngine;

namespace Astralum.Materials
{
  public static class InternalShaderPropertyIds
  {
    #region Star Properties

    public static readonly int Chromaticity = Shader.PropertyToID("_Chromaticity");
    public static readonly int Corona = Shader.PropertyToID("_Corona");
    public static readonly int SurfaceNoiseStrength = Shader.PropertyToID("_SurfaceNoiseStrength");
    public static readonly int CoronaRotationSpeed = Shader.PropertyToID("_CoronaRotationSpeed");
    public static readonly int ChromaticityIntensity = Shader.PropertyToID("_ChromaticityIntensity");
    public static readonly int CoronaIntensity = Shader.PropertyToID("_CoronaIntensity");
    public static readonly int OuterCoronaIntensity = Shader.PropertyToID("_OuterCoronaIntensity");
    public static readonly int ChromaticityFalloffPower = Shader.PropertyToID("_ChromaticityFalloffPower");
    public static readonly int CoronaPower = Shader.PropertyToID("_CoronaPower");
    public static readonly int OuterCoronaPower = Shader.PropertyToID("_OuterCoronaPower");
    public static readonly int VariabilityAmount = Shader.PropertyToID("_VariabilityAmount");
    public static readonly int VariabilitySpeed = Shader.PropertyToID("_VariabilitySpeed");

    #endregion

    #region Nebula Properties

    public static readonly int ColorA = Shader.PropertyToID("_ColorA");
    public static readonly int ColorB = Shader.PropertyToID("_ColorB");
    public static readonly int ColorC = Shader.PropertyToID("_ColorC");
    public static readonly int ColorD = Shader.PropertyToID("_ColorD");
    public static readonly int ColorStopB = Shader.PropertyToID("_ColorStopB");
    public static readonly int ColorStopC = Shader.PropertyToID("_ColorStopC");
    public static readonly int ColorBandSharpness = Shader.PropertyToID("_ColorBandSharpness");
    public static readonly int SeedOffset = Shader.PropertyToID("_SeedOffset");
    public static readonly int Seed = Shader.PropertyToID("_Seed");
    public static readonly int Intensity = Shader.PropertyToID("_Intensity");
    public static readonly int Alpha = Shader.PropertyToID("_Alpha");
    public static readonly int NoiseScale = Shader.PropertyToID("_NoiseScale");
    public static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");
    public static readonly int CloudThreshold = Shader.PropertyToID("_CloudThreshold");
    public static readonly int EdgeSoftness = Shader.PropertyToID("_EdgeSoftness");
    public static readonly int WarpScale = Shader.PropertyToID("_WarpScale");
    public static readonly int WarpStrength = Shader.PropertyToID("_WarpStrength");
    public static readonly int ShapePower = Shader.PropertyToID("_ShapePower");
    public static readonly int CoreOffset = Shader.PropertyToID("_CoreOffset");
    public static readonly int StretchX = Shader.PropertyToID("_StretchX");
    public static readonly int StretchY = Shader.PropertyToID("_StretchY");
    public static readonly int Rotation = Shader.PropertyToID("_Rotation");

    #endregion

    #region Constellation Properties

    public static readonly int MainTex = Shader.PropertyToID("_MainTex");
    public static readonly int BlurStrength = Shader.PropertyToID("_BlurStrength");

    #endregion

    #region Constellation Hover Ring Properties

    public static readonly int RingRadius = Shader.PropertyToID("_RingRadius");
    public static readonly int RingThickness = Shader.PropertyToID("_RingThickness");
    public static readonly int PulseSpeed = Shader.PropertyToID("_PulseSpeed");
    public static readonly int PulseStrength = Shader.PropertyToID("_PulseStrength");
    public static readonly int AlphaPulseMin = Shader.PropertyToID("_AlphaPulseMin");
    public static readonly int PulseTime = Shader.PropertyToID("_PulseTime");

    #endregion

    #region Shooting Star Properties

    public static readonly int CorePower = Shader.PropertyToID("_CorePower");
    public static readonly int TailPower = Shader.PropertyToID("_TailPower");

    #endregion
  }
}