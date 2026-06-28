using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.UI
{
  public static class CelestialNamingCameraUtil
  {
    private const float PlanetRadius = 100f;
    private const float SkyRenderDistance = 100f;
    private const float TargetPositionBetweenPlanetAndScreenEdge = 0.5f;
    private const float AnimationDuration = 0.45f;
    
    private static bool _isAnimating;
    private static Quaternion _startSphereRotation;
    private static Quaternion _targetSphereRotation;
    private static float _animationStartTime;
    
    public static void FocusObject(Vector3 localSkyPos, Rect namingWindowRect)
    {
      WorldCameraDriver driver = Find.WorldCameraDriver;

      if (driver == null)
        return;

      Camera camera = driver.GetComponent<Camera>();

      if (camera == null)
        return;

      Quaternion targetSphereRotation =
        CalculateTargetSphereRotation(camera, localSkyPos, namingWindowRect);

      StartAnimation(driver.sphereRotation, targetSphereRotation);
    }

    public static void Update()
    {
      if (!_isAnimating)
        return;

      WorldCameraDriver driver = Find.WorldCameraDriver;

      if (driver == null)
      {
        _isAnimating = false;
        return;
      }

      float t = (Time.realtimeSinceStartup - _animationStartTime) / AnimationDuration;

      if (t >= 1f)
      {
        driver.sphereRotation = _targetSphereRotation;
        _isAnimating = false;
        return;
      }
      
      t = EaseInOutSine(Mathf.Clamp01(t));

      driver.sphereRotation = Quaternion.Slerp(_startSphereRotation, _targetSphereRotation, t);
    }
    
    public static void StopAnimation()
    {
      _isAnimating = false;
    }
    
    private static void StartAnimation(Quaternion startSphereRotation, Quaternion targetSphereRotation)
    {
      _startSphereRotation = startSphereRotation;
      _targetSphereRotation = targetSphereRotation;
      _animationStartTime = Time.realtimeSinceStartup;
      _isAnimating = true;
    }
    
    private static Quaternion CalculateTargetSphereRotation(Camera camera, Vector3 localSkyPos, Rect namingWindowRect)
    {
      Vector3 worldDir = World.WorldUtils.GetCurrentRotationForWorldSpace() * localSkyPos.normalized;
      
      bool windowOnLeft = namingWindowRect.center.x < Verse.UI.screenWidth * 0.5f;
      float sideSign = windowOnLeft ? 1f : -1f;
      
      float halfViewHeight = camera.orthographicSize;
      float halfViewWidth = halfViewHeight * camera.aspect;
      
      float planetEdgeX = PlanetRadius;
      float screenEdgeX = halfViewWidth;
      
      float desiredScreenX = Mathf.Lerp(planetEdgeX, screenEdgeX, TargetPositionBetweenPlanetAndScreenEdge) 
                             * sideSign;
      
      float desiredLocalX = Mathf.Clamp(desiredScreenX / SkyRenderDistance, -0.95f, 0.95f);
      Vector3 desiredLocalDir = new(desiredLocalX, 0f, Mathf.Sqrt(1f - desiredLocalX * desiredLocalX));
      
      Quaternion desiredCameraRotation =
        Quaternion.LookRotation(worldDir, Vector3.up) *
        Quaternion.Inverse(Quaternion.LookRotation(desiredLocalDir, Vector3.up));
      
      return Quaternion.Inverse(desiredCameraRotation);
    }
    
    private static float EaseInOutSine(float t)
    {
      return -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;
    }
  }
}