using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class SkyCoordinateGridMatsUtil
  {
    public static readonly Material LineMaterial;

    static SkyCoordinateGridMatsUtil()
    {
      Shader shader = InternalDefOf.Astra_SkyCoordinateGrid01.Shader;

      LineMaterial = new Material(shader)
      {
        name = "Astralum_SkyCoordinateGrid01"
      };
      
      LineMaterial.SetColor(ShaderPropertyIDs.Color, CelestialColorGetter.SkyCoordGridLineColor);
      LineMaterial.SetFloat(InternalShaderPropertyIds.Intensity, 0.85f);

      Object.DontDestroyOnLoad(LineMaterial);
    }
  }
}