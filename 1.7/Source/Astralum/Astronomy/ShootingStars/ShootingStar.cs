using UnityEngine;

namespace Astralum.Astronomy.ShootingStars
{
  public class ShootingStar
  {
    public float age;
    public float length;
    public float lifetime;
    public Vector3 origin;
    public Vector3 travelDir;

    public float travelDistance;
    public float width;

    public float Progress => Mathf.Clamp01(age / lifetime);
    public bool Expired => age >= lifetime;
  }
}