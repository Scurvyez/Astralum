using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
    [StaticConstructorOnStartup]
    public static class SkyCoordinateGridMatsUtil
    {
        public static readonly Material Line;
        
        private static Texture2D _toggleIcon;
        
        public static Texture2D ToggleIcon =>
            _toggleIcon ??= ContentFinder<Texture2D>.Get("UI/Icons/AstralumSkyGrid", reportFailure: false)
                            ?? TexButton.Info;
        
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