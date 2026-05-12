using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
    [StaticConstructorOnStartup]
    public static class ConstellationsMatsUtil
    {
        public static readonly Material ConstellationLine;
        
        static ConstellationsMatsUtil()
        {
            Shader conLineShader = InternalDefOf.Astra_ConstellationLine01.Shader;
            
            ConstellationLine = new Material(conLineShader)
            {
                name = "Astralum_ConstellationLine01"
            };
            
            ConstellationLine.SetFloat("_Intensity", 0.875f);
            ConstellationLine.SetFloat("_BlurStrength", 0.45f);
            ConstellationLine.SetFloat("_CoreStrength", 0.15f);
            
            Object.DontDestroyOnLoad(ConstellationLine);
        }
    }
}