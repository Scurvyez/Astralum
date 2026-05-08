using UnityEngine;

namespace Astralum.Materials
{
    public static class InternalShaderPropertyIds
    {
        public static readonly int Chromaticity = Shader.PropertyToID("_Chromaticity");
        public static readonly int Corona = Shader.PropertyToID("_Corona");
        
        public static readonly int ChromaticityIntensity = Shader.PropertyToID("_ChromaticityIntensity");
        public static readonly int CoronaIntensity = Shader.PropertyToID("_CoronaIntensity");
        
        public static readonly int ChromaticityFalloffPower = Shader.PropertyToID("_ChromaticityFalloffPower");
        public static readonly int CoronaPower = Shader.PropertyToID("_CoronaPower");
        public static readonly int RadiusPower = Shader.PropertyToID("_RadiusPower");
        
        public static readonly int VariabilityAmount = Shader.PropertyToID("_VariabilityAmount");
        public static readonly int VariabilitySpeed = Shader.PropertyToID("_VariabilitySpeed");
    }
}