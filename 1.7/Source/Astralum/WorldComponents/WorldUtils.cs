using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.WorldComponents
{
  public static class WorldUtils
  {
    public const float PlanetRadius = 100f;
    
    public static bool ShouldDrawGui()
    {
      Camera worldCamera = Find.WorldCamera;

      return Current.ProgramState == ProgramState.Playing
             && Find.World != null
             && worldCamera != null
             && worldCamera.gameObject.activeInHierarchy
             && WorldRendererUtility.WorldSelected
             && Find.UIRoot?.screenshotMode?.FiltersCurrentEvent != true;
    }
    
    public static bool GuiPointIsOverPlanetDisk(Vector2 guiPos, Vector2 guiOffset)
    {
      return GuiPointIsOverPlanetDisk(guiPos - guiOffset);
    }
    
    public static bool MouseIsOverPlanetDisk()
    {
      return Event.current != null && GuiPointIsOverPlanetDisk(Event.current.mousePosition);
    }
    
    private static bool GuiPointIsOverPlanetDisk(Vector2 guiPos)
    {
      return TryGetPlanetDiskGuiBounds(out Vector2 centerGui, out float radius)
             && Vector2.Distance(guiPos, centerGui) <= radius;
    }
    
    private static bool TryGetPlanetDiskGuiBounds(out Vector2 centerGui, out float radius)
    {
      centerGui = default;
      radius = 0f;
      
      Camera worldCamera = Find.WorldCamera;
      
      if (worldCamera == null)
        return false;
      
      Vector3 planetCenter = Vector3.zero;
      Vector3 planetEdge = planetCenter + worldCamera.transform.right * PlanetRadius;
      
      centerGui = WorldToGuiPoint(worldCamera, planetCenter);
      Vector2 edgeGui = WorldToGuiPoint(worldCamera, planetEdge);
      
      radius = Vector2.Distance(centerGui, edgeGui);
      
      return true;
    }
    
    private static Vector2 WorldToGuiPoint(Camera camera, Vector3 worldPoint)
    {
      Vector3 screenPoint = camera.WorldToScreenPoint(worldPoint);
      
      return GUIUtility.ScreenToGUIPoint(new Vector2(screenPoint.x, Screen.height - screenPoint.y));
    }
    
    public static bool TryRaySphereIntersectionForHoverInfo(Ray ray, Vector3 sphereCenter, float radius, out Vector3 hit)
    {
      Vector3 oc = ray.origin - sphereCenter;

      float a = Vector3.Dot(ray.direction, ray.direction);
      float b = 2f * Vector3.Dot(oc, ray.direction);
      float c = Vector3.Dot(oc, oc) - radius * radius;
      float discriminant = b * b - 4f * a * c;

      if (discriminant < 0f)
      {
        hit = default;
        return false;
      }

      float sqrt = Mathf.Sqrt(discriminant);
      float t0 = (-b - sqrt) / (2f * a);
      float t1 = (-b + sqrt) / (2f * a);
      float t = t0 > 0f ? t0 : t1;

      if (t <= 0f)
      {
        hit = default;
        return false;
      }

      hit = ray.origin + ray.direction * t;
      return true;
    }
  }
}