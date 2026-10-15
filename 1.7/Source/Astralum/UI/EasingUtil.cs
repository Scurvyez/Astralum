using UnityEngine;

namespace Astralum.UI
{
  public static class EasingUtil
  {
    public static float Evaluate(CameraPanEasing easing, float t)
    {
      t = Mathf.Clamp01(t);
      
      return easing switch
      {
        CameraPanEasing.Linear => t,
        CameraPanEasing.Sine => EaseInOutSine(t),
        CameraPanEasing.Quadratic => EaseInOutQuad(t),
        CameraPanEasing.Cubic => EaseInOutCubic(t),
        CameraPanEasing.Quartic => EaseInOutQuart(t),
        _ => t
      };
    }
    
    private static float EaseInOutSine(float t)
    {
      return -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;
    }
    
    private static float EaseInOutQuad(float t)
    {
      return t < 0.5f
        ? 2f * t * t
        : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
    }
    
    private static float EaseInOutCubic(float t)
    {
      return t < 0.5f
        ? 4f * t * t * t
        : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
    }
    
    private static float EaseInOutQuart(float t)
    {
      return t < 0.5f
        ? 8f * t * t * t * t
        : 1f - Mathf.Pow(-2f * t + 2f, 4f) * 0.5f;
    }
  }
}