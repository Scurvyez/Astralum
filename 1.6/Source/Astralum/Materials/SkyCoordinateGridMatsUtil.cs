using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
    [StaticConstructorOnStartup]
    public static class SkyCoordinateGridMatsUtil
    {
        public static readonly Material Line;
        
        public static readonly Texture2D ShowSkyGridIcon =
            ContentFinder<Texture2D>.Get("Astralum/UI/Icons/ShowSkyGrid", reportFailure: false);
        
        static SkyCoordinateGridMatsUtil()
        {
            Shader shader = InternalDefOf.Astra_SkyCoordinateGrid01.Shader;
            
            Line = new Material(shader)
            {
                name = "Astralum_SkyCoordinateGrid01"
            };
            
            Line.SetColor(ShaderPropertyIDs.Color, new Color(0.35f, 0.65f, 1f, 0.28f));
            Line.SetFloat(InternalShaderPropertyIds.Intensity, 0.85f);
            
            Object.DontDestroyOnLoad(Line);
        }
    }
}