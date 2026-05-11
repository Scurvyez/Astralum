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
    }
}