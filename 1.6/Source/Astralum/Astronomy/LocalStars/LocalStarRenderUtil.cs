using UnityEngine;

namespace Astralum.Astronomy.LocalStars
{
  public static class LocalStarRenderUtil
  {
    private const float MinRenderSize = 4.5f;
    private const float MaxRenderSize = 12f;
    
    public static float RenderSizeFor(SavedLocalStar star)
    {
      if (star == null)
        return 7.5f;
      
      float radius = Mathf.Max(star.radius, 0.01f);
      float size = star.RenderSize * Mathf.Sqrt(radius);
      
      return Mathf.Clamp(size, MinRenderSize, MaxRenderSize);
    }
  }
}