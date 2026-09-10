using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class ShootingStarMatsUtil
  {
    public static readonly Material ShootingStarMaterial;

    static ShootingStarMatsUtil()
    {
      Shader shader = InternalDefOf.Astra_ShootingStar01.Shader;

      ShootingStarMaterial = new Material(shader)
      {
        name = "Astralum_ShootingStar01"
      };

      ShootingStarMaterial.SetColor(ShaderPropertyIDs.Color, CelestialColorGetter.ShootingStarColor);
      ShootingStarMaterial.SetFloat(InternalShaderPropertyIds.Intensity, 2.25f);
      ShootingStarMaterial.SetFloat(InternalShaderPropertyIds.CorePower, 4f);
      ShootingStarMaterial.SetFloat(InternalShaderPropertyIds.TailPower, 2.2f);

      Object.DontDestroyOnLoad(ShootingStarMaterial);
    }
  }
}