using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.UI
{
  public static class CelestialNamingCameraUtil
  { 
    private const float TargetViewportXFromEdge = 0.005f;
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
      
      Quaternion targetSphereRotation = CalculateTargetSphereRotation(camera, localSkyPos, namingWindowRect);
      StartAnimation(driver.sphereRotation, targetSphereRotation);
    }
    
    public static void Update()
    {
      if (!_isAnimating)
        return;
      
      WorldCameraDriver driver = Find.WorldCameraDriver;
      
      if (driver == null)
      {
        StopAnimation();
        return;
      }
      
      float t = (Time.realtimeSinceStartup - _animationStartTime) / AnimationDuration;
      
      if (t >= 1f)
      {
        driver.sphereRotation = _targetSphereRotation;
        StopAnimation();
        return;
      }
      
      t = EaseInOutSine(Mathf.Clamp01(t));
      driver.sphereRotation = Quaternion.Slerp(_startSphereRotation, _targetSphereRotation, t);
    }
    
    public static void StopAnimation()
    {
      _isAnimating = false;
    }
    
    private static Quaternion CalculateTargetSphereRotation(Camera camera, Vector3 localSkyPos, Rect namingWindowRect)
    {
      Vector3 worldDir = World.WorldUtils.GetCurrentRotationForWorldSpace() * localSkyPos.normalized;
      bool windowOnLeft = namingWindowRect.center.x < Verse.UI.screenWidth * 0.5f;
      
      float targetViewportX = windowOnLeft 
        ? 1f - TargetViewportXFromEdge 
        : TargetViewportXFromEdge;
      
      float halfViewWidth = camera.orthographicSize * camera.aspect;
      float desiredCameraX = (targetViewportX - 0.5f) * 2f * halfViewWidth;
      float skyDistance = localSkyPos.magnitude;
      
      if (skyDistance <= 0.001f)
        return Find.WorldCameraDriver.sphereRotation;
      
      float normalizedX = Mathf.Clamp(desiredCameraX / skyDistance, -0.95f, 0.95f);
      float normalizedZ = Mathf.Sqrt(1f - normalizedX * normalizedX);
      
      // construct an "up" direction that is perpendicular
      // to the celestial object's world direction...
      Vector3 cameraUp = Vector3.ProjectOnPlane(Vector3.up, worldDir);
      
      // near the poles, Vector3.up can become parallel to
      // worldDir, so use the current camera's up as fallback....
      if (cameraUp.sqrMagnitude < 0.001f)
      {
        cameraUp = Vector3.ProjectOnPlane(camera.transform.up, worldDir);
      }
      
      cameraUp.Normalize();

      Vector3 tangent = Vector3.Cross(cameraUp, worldDir).normalized;
      Vector3 cameraRight = tangent * normalizedZ + worldDir * normalizedX;
      Vector3 cameraForward = -tangent * normalizedX + worldDir * normalizedZ;
      
      cameraRight.Normalize();
      cameraForward.Normalize();
      
      Quaternion desiredCameraRotation = Quaternion.LookRotation(cameraForward, cameraUp);
      
      return Quaternion.Inverse(desiredCameraRotation
      );
    }
    
    private static void StartAnimation(Quaternion startSphereRotation, Quaternion targetSphereRotation)
    {
      _startSphereRotation = startSphereRotation;
      _targetSphereRotation = targetSphereRotation;
      _animationStartTime = Time.realtimeSinceStartup;
      _isAnimating = true;
    }
    
    private static float EaseInOutSine(float t)
    {
      return -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;
    }
  }
}