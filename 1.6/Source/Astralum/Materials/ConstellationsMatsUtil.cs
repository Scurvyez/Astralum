using System.Collections.Generic;
using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class ConstellationsMatsUtil
  {
    private static readonly Dictionary<Texture2D, Material> MaterialsByTexture = new();

    public static readonly Texture2D ShowConstellationLinesIcon =
      ContentFinder<Texture2D>.Get("UI/Icons/ShowConstellationLines", false);

    public static Material For(Texture2D texture)
    {
      if (texture == null)
        return null;

      if (MaterialsByTexture.TryGetValue(texture, out Material material))
        return material;

      Shader shader = InternalDefOf.Astra_Constellation01.Shader;

      material = new Material(shader)
      {
        name = $"Astralum_ConstellationTexture01_{texture.name}",
        mainTexture = texture
      };

      material.SetTexture(InternalShaderPropertyIds.MainTex, texture);
      material.SetColor(ShaderPropertyIDs.Color, new Color(0.45f, 0.60f, 1.0f, 0.35f));
      material.SetFloat(InternalShaderPropertyIds.Intensity, 0.875f);
      material.SetFloat(InternalShaderPropertyIds.BlurStrength, 0.45f);

      Object.DontDestroyOnLoad(material);

      MaterialsByTexture[texture] = material;
      return material;
    }
  }
}