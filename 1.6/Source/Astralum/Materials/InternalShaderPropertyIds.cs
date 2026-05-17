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
    
    #region Pulsar Properties
    
    public static readonly int ShellDarkColor = Shader.PropertyToID("_ShellDarkColor");
    public static readonly int ShellBrightColor = Shader.PropertyToID("_ShellBrightColor");
    public static readonly int CoreColor = Shader.PropertyToID("_CoreColor");
    public static readonly int JetColor = Shader.PropertyToID("_JetColor");
    public static readonly int CoreRadius = Shader.PropertyToID("_CoreRadius");
    public static readonly int CoreGlowRadius = Shader.PropertyToID("_CoreGlowRadius");
    public static readonly int CoreIntensity = Shader.PropertyToID("_CoreIntensity");
    public static readonly int CorePulseSpeed = Shader.PropertyToID("_CorePulseSpeed");
    public static readonly int CorePulseStrength = Shader.PropertyToID("_CorePulseStrength");
    public static readonly int ShellRadius = Shader.PropertyToID("_ShellRadius");
    public static readonly int ShellThickness = Shader.PropertyToID("_ShellThickness");
    public static readonly int ShellSoftness = Shader.PropertyToID("_ShellSoftness");
    public static readonly int ShellPower = Shader.PropertyToID("_ShellPower");
    public static readonly int ShellCoverage = Shader.PropertyToID("_ShellCoverage");
    public static readonly int InnerCrescentRadiusOffset = Shader.PropertyToID("_InnerCrescentRadiusOffset");
    public static readonly int InnerCrescentThickness = Shader.PropertyToID("_InnerCrescentThickness");
    public static readonly int InnerCrescentSoftness = Shader.PropertyToID("_InnerCrescentSoftness");
    public static readonly int InnerCrescentIntensity = Shader.PropertyToID("_InnerCrescentIntensity");
    public static readonly int BandRadiusOffset = Shader.PropertyToID("_BandRadiusOffset");
    public static readonly int BandThickness = Shader.PropertyToID("_BandThickness");
    public static readonly int BandIntensity = Shader.PropertyToID("_BandIntensity");
    public static readonly int BandSoftness = Shader.PropertyToID("_BandSoftness");
    public static readonly int JetLength = Shader.PropertyToID("_JetLength");
    public static readonly int JetWidth = Shader.PropertyToID("_JetWidth");
    public static readonly int JetSpread = Shader.PropertyToID("_JetSpread");
    public static readonly int JetIntensity = Shader.PropertyToID("_JetIntensity");
    public static readonly int JetFalloff = Shader.PropertyToID("_JetFalloff");
    public static readonly int JetFlicker = Shader.PropertyToID("_JetFlicker");
    public static readonly int JetFlickerSpeed = Shader.PropertyToID("_JetFlickerSpeed");
    public static readonly int JetSoftness = Shader.PropertyToID("_JetSoftness");
    public static readonly int DustIntensity = Shader.PropertyToID("_DustIntensity");
    public static readonly int DustAmount = Shader.PropertyToID("_DustAmount");
    public static readonly int DustSpread = Shader.PropertyToID("_DustSpread");
    public static readonly int DetailScale = Shader.PropertyToID("_DetailScale");
    public static readonly int DetailStrength = Shader.PropertyToID("_DetailStrength");
    
    #endregion
    
    #region Black Hole Properties
    
    public static readonly int EffectActive = Shader.PropertyToID("_EffectActive");
    public static readonly int CanvasScale = Shader.PropertyToID("_CanvasScale");
    public static readonly int ScreenEdgeFadeStart = Shader.PropertyToID("_ScreenEdgeFadeStart");
    public static readonly int ScreenEdgeFadeEnd = Shader.PropertyToID("_ScreenEdgeFadeEnd");
    public static readonly int Radius = Shader.PropertyToID("_Radius");
    public static readonly int DistortionRadius = Shader.PropertyToID("_DistortionRadius");
    public static readonly int DistortionStrength = Shader.PropertyToID("_DistortionStrength");
    public static readonly int Darkness = Shader.PropertyToID("_Darkness");
    public static readonly int HorizonFeather = Shader.PropertyToID("_HorizonFeather");
    public static readonly int DistortionFeather = Shader.PropertyToID("_DistortionFeather");
    public static readonly int RingFeather = Shader.PropertyToID("_RingFeather");
    
    #endregion
  }
}