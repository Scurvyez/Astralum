using RimWorld;
using UnityEngine;
using Verse;

namespace Astralum.API
{
  public static class CelestialTransformUtil
  {
    public static Vector3 GalacticPole => Quaternion.Euler(GenCelestial.CurSunPositionInWorldSpace()) * Vector3.up;
    
    public static Quaternion GetCurrentRotationForWorldSpace()
    {
      return Current.ProgramState == ProgramState.Entry 
        ? Quaternion.identity 
        : Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
    }
    
    public static Vector3 RandomGalacticPlaneDirection(FloatRange bounds = default)
    {
      if (bounds == default)
        bounds = new FloatRange(-0.2f, 0.2f);
      
      float angle = Rand.Range(0f, Mathf.PI * 2f);
      float localY = Mathf.Clamp(bounds.RandomInRange, -1f, 1f);
      
      float radius = Mathf.Sqrt(1f - localY * localY);
      
      Vector3 localDir = new Vector3(
        Mathf.Cos(angle) * radius,
        localY,
        Mathf.Sin(angle) * radius
      ).normalized;
      
      Quaternion planeRotation = Quaternion.FromToRotation(Vector3.up, GalacticPole.normalized);
      
      return (planeRotation * localDir).normalized;
    }
  }
}