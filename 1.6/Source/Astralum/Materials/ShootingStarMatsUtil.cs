using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
    [StaticConstructorOnStartup]
    public static class ShootingStarMatsUtil
    {
        public static readonly Material ShootingStar;
        
        static ShootingStarMatsUtil()
        {
            Shader shader = InternalDefOf.Astra_ShootingStar01.Shader;
            
            ShootingStar = new Material(shader)
            {
                name = "Astralum_ShootingStar01"
            };
            
            ShootingStar.SetColor(ShaderPropertyIDs.Color, new Color(0.75f, 0.9f, 1f, 0.85f));
            ShootingStar.SetFloat(InternalShaderPropertyIds.Intensity, 2.25f);
            ShootingStar.SetFloat(InternalShaderPropertyIds.CorePower, 4f);
            ShootingStar.SetFloat(InternalShaderPropertyIds.TailPower, 2.2f);
            
            Object.DontDestroyOnLoad(ShootingStar);
        }
    }
}