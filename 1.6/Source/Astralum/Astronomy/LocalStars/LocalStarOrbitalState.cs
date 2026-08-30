using UnityEngine;

namespace Astralum.Astronomy.LocalStars
{
  public readonly struct LocalStarOrbitalState
  {
    public Vector3 Position { get; }
    public float Depth { get; }
    
    public LocalStarOrbitalState(Vector3 position, float depth)
    {
      Position = position;
      Depth = depth;
    }
  }
}