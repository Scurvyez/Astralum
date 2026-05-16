using UnityEngine;

namespace Astralum.Astronomy.Constellations
{
  public static class ConstellationHoverState
  {
    public static ConstellationInteractionRegistry.HoverStar? CurrentStar;
    public static bool Dirty;
    private static float _hoverStartTime;

    public static float PulseTime =>
      CurrentStar.HasValue ? Time.time - _hoverStartTime : 0f;

    public static void SetHovered(ConstellationInteractionRegistry.HoverStar? star)
    {
      bool changed =
        CurrentStar.HasValue != star.HasValue ||
        (CurrentStar.HasValue &&
         star.HasValue &&
         CurrentStar.Value.localSkyPos != star.Value.localSkyPos);

      if (!changed)
        return;

      CurrentStar = star;
      _hoverStartTime = Time.time;
      Dirty = true;
    }

    public static void Clear()
    {
      SetHovered(null);
    }
  }
}