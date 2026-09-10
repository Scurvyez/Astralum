using System.Collections.Generic;
using Astralum.Astronomy.LocalStars;
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
      CreateMaterial(SpectralClass.O);
      CreateMaterial(SpectralClass.B);
      CreateMaterial(SpectralClass.A);
      CreateMaterial(SpectralClass.F);
      CreateMaterial(SpectralClass.G);
      CreateMaterial(SpectralClass.K);
      CreateMaterial(SpectralClass.M);
    }

    public static Material For(SpectralClass spectralClass)
    {
      return Materials[spectralClass];
    }

    private static void CreateMaterial(SpectralClass spectralClass)
    {
      Shader shader = InternalDefOf.Astra_BackgroundStar01.Shader;

      Material material = new Material(shader)
      {
        name = $"Astralum_BackgroundStar{spectralClass}"
      };
      
      material.SetColor(ShaderPropertyIDs.Color, CelestialColorGetter.TryGetStarColorFor(spectralClass));
      material.SetFloat(InternalShaderPropertyIds.Intensity, 1f);

      Object.DontDestroyOnLoad(material);

      Materials[spectralClass] = material;
    }
  }
}