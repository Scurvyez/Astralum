using System.Collections.Generic;
using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
    [StaticConstructorOnStartup]
    public static class BackgroundStarMatsUtil
    {
        private static readonly Dictionary<SpectralClass, Material> Materials = new();
        
        static BackgroundStarMatsUtil()
        {
            CreateMaterial(SpectralClass.O, new Color(0.62f, 0.78f, 1f, 1f));
            CreateMaterial(SpectralClass.B, new Color(0.70f, 0.85f, 1f, 1f));
            CreateMaterial(SpectralClass.A, new Color(0.85f, 0.93f, 1f, 1f));
            CreateMaterial(SpectralClass.F, new Color(1f, 0.98f, 0.88f, 1f));
            CreateMaterial(SpectralClass.G, new Color(1f, 0.90f, 0.62f, 1f));
            CreateMaterial(SpectralClass.K, new Color(1f, 0.62f, 0.32f, 1f));
            CreateMaterial(SpectralClass.M, new Color(1f, 0.34f, 0.22f, 1f));
        }
        
        public static Material For(SpectralClass spectralClass)
        {
            return Materials[spectralClass];
        }

        private static void CreateMaterial(SpectralClass spectralClass, Color color)
        {
            Shader shader = InternalDefOf.Astra_BackgroundStar01.Shader;

            Material material = new Material(shader)
            {
                name = $"Astralum_BackgroundStar{spectralClass}"
            };
            
            material.SetColor("_Color", color);
            material.SetFloat("_Intensity", 1f);
            
            Object.DontDestroyOnLoad(material);
            
            Materials[spectralClass] = material;
        }
    }
}