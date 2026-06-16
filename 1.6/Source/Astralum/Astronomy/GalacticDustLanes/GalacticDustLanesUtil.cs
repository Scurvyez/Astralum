using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.GalacticDustLanes
{
  public static class GalacticDustLanesUtil
  {
    public static float DustLaneRotationDegrees(Vector3 localDir)
    {
      Vector3 pole = WorldUtils.GalacticPole.normalized;
      Vector3 tangent = Vector3.Cross(pole, localDir).normalized;
      
      if (tangent == Vector3.zero)
        return Rand.Range(-8f, 8f);
      
      Quaternion tilt = Quaternion.AngleAxis(Rand.Range(-5f, 5f), localDir);
      tangent = tilt * tangent;
      
      GetDustLaneBasis(localDir, out Vector3 tangentA, out Vector3 tangentB);
      
      float x = Vector3.Dot(tangent, tangentA);
      float y = Vector3.Dot(tangent, tangentB);
      
      return Mathf.Atan2(y, x) * Mathf.Rad2Deg + 90f;
    }

    private static void GetDustLaneBasis(Vector3 centerDir, out Vector3 tangentA, out Vector3 tangentB)
    {
      centerDir.Normalize();
      
      Vector3 reference = Mathf.Abs(Vector3.Dot(centerDir, Vector3.up)) > 0.95f
        ? Vector3.forward
        : Vector3.up;
      
      tangentA = Vector3.Cross(reference, centerDir).normalized;
      tangentB = Vector3.Cross(centerDir, tangentA).normalized;
    }
  }
}