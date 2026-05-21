using UnityEngine;

namespace Astralum.Astronomy.BlackHoles
{
  public struct PlacedBlackHole
  {
    public readonly Vector3 dir;
    public readonly float size;
      
    public PlacedBlackHole(Vector3 dir, float size)
    {
      this.dir = dir.normalized;
      this.size = size;
    }
  }
}