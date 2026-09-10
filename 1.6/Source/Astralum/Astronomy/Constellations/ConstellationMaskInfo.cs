using UnityEngine;

namespace Astralum.Astronomy.Constellations
{
  public readonly struct ConstellationMaskInfo
  {
    public readonly Texture2D Texture;
    public readonly string CategoryId;

    public ConstellationMaskInfo(Texture2D texture, string categoryId)
    {
      Texture = texture;
      CategoryId = categoryId;
    }
  }
}