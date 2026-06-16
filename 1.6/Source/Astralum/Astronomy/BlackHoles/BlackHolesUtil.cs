using System.Collections.Generic;
using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.BlackHoles
{
  public static class BlackHolesUtil
  {
    private const float MinApartDistance = 3f;
    private const int MaxPlacementAttempts = 40;

    /// <summary>
    /// Attempts to place a new black hole in a random direction on the galactic plane, ensuring it does not overlap with existing black holes.
    /// </summary>
    /// <param name="placed">A list of already placed black holes to check for overlap.</param>
    /// <param name="dir">The output direction vector for the successfully placed black hole.</param>
    /// <param name="size">The output size of the successfully placed black hole.</param>
    /// <param name="galacticPlaneBounds">The angular bounds defining the galactic plane for placement.</param>
    /// <param name="blackHoleSize">The range of possible sizes for the new black hole.</param>
    /// <param name="blackHoleCanvasScale">A scaling factor for the black hole size applied during placement.</param>
    /// <returns>True if a black hole was successfully placed; otherwise, false.</returns>
    public static bool TryPlaceBlackHole(List<SavedBlackHole> placed, out Vector3 dir, out float size,
      FloatRange galacticPlaneBounds, FloatRange blackHoleSize, float blackHoleCanvasScale)
    {
      for (int attempt = 0; attempt < MaxPlacementAttempts; attempt++)
      {
        dir = WorldUtils.RandomGalacticPlaneDirection(galacticPlaneBounds);
        size = blackHoleSize.RandomInRange * blackHoleCanvasScale;
        
        if (!OverlapsExistingBlackHole(dir, size, placed))
          return true;
      }
      
      dir = default;
      size = 0f;
      return false;
    }
    
    /// <summary>
    /// Determines if a new black hole overlaps with any existing black holes based on their angular distances and radii.
    /// </summary>
    /// <param name="dir">The direction vector of the new black hole being checked.</param>
    /// <param name="size">The size of the new black hole being checked.</param>
    /// <param name="placed">A list of already placed black holes.</param>
    /// <returns>True if the new black hole overlaps with any existing black holes; otherwise, false.</returns>
    private static bool OverlapsExistingBlackHole(Vector3 dir, float size, List<SavedBlackHole> placed)
    {
      for (int i = 0; i < placed.Count; i++)
      {
        float angularDistance = Vector3.Angle(dir, placed[i].dir) * Mathf.Deg2Rad;
        
        float thisAngularRadius = Mathf.Atan((size * 0.5f) / MinApartDistance);
        float otherAngularRadius = Mathf.Atan((placed[i].size * 0.5f) / MinApartDistance);
        
        float requiredDistance = thisAngularRadius + otherAngularRadius;
        
        if (angularDistance < requiredDistance)
          return true;
      }
      
      return false;
    }
  }
}