using UnityEngine;

namespace Astralum.Astronomy.Constellations
{
  public readonly struct ConstellationMaskInfo
  {
    public readonly Texture2D texture;
    public readonly string categoryId;

    public ConstellationMaskInfo(Texture2D texture, string categoryId)
    {
      this.texture = texture;
      this.categoryId = categoryId;
    }
  }
}