using Astralum.Astronomy.LocalSystem.Stars;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.World
{
  public static class WorldUtils
  {
    public const float NorthernSkyThreshold = 0.15f;
    public const float SouthernSkyThreshold = -0.15f;
    private const float PlanetRadius = 100f;

    public static Vector3 GalacticPole => Quaternion.Euler(
      GenCelestial.CurSunPositionInWorldSpace()) * Vector3.up;

    public static SavedStar CurrentLocalStar
    {
      get
      {
        RimWorld.Planet.World world = Find.World;
        WorldComponent_LocalStar comp = world?.GetComponent<WorldComponent_LocalStar>();

        return comp?.Star;
      }
    }
    
    public static Quaternion GetCurrentRotationForWorldSpace()
    {
      return Current.ProgramState == ProgramState.Entry 
        ? Quaternion.identity 
        : Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
    }
    
    public static Vector3 RandomGalacticPlaneDirection(FloatRange bounds = default)
    {
      if (bounds == default)
        bounds = new FloatRange(-0.15f, 0.15f);
      
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
    
    public static SkyCoord DirectionToSkyCoord(Vector3 direction)
    {
      direction.Normalize();
      
      float raDegrees = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
      
      if (raDegrees < 0f)
        raDegrees += 360f;
      
      float declinationDegrees = Mathf.Asin(direction.y) * Mathf.Rad2Deg;
      float raHours = raDegrees / 15f;
      
      return new SkyCoord(raHours, declinationDegrees);
    }

    public static string FormatRightAscension(float hours)
    {
      hours = Mathf.Repeat(hours, 24f);
      
      int h = Mathf.FloorToInt(hours);
      int m = FractionalMinutes(hours, h);
      
      return $"{h:00}h {m:00}m";
    }

    public static string FormatDeclination(float degrees)
    {
      string sign = degrees >= 0f ? "+" : "-";
      degrees = Mathf.Abs(degrees);
      
      int d = Mathf.FloorToInt(degrees);
      int m = FractionalMinutes(degrees, d);
      
      return $"{sign}{d:00}° {m:00}'";
    }
    
    private static int FractionalMinutes(float value, int wholeUnits)
    {
      return Mathf.FloorToInt((value - wholeUnits) * 60f);
    }
    
    public static float SkyLatitudeHeight(float degrees)
    {
      return Mathf.Sin(degrees * Mathf.Deg2Rad);
    }

    public static string SkyHemisphere(Vector3 direction)
    {
      direction.Normalize();

      return direction.y switch
      {
        > NorthernSkyThreshold => "Astra_Hemisphere_NorthernSky".Translate(),
        < SouthernSkyThreshold => "Astra_Hemisphere_SouthernSky".Translate(),
        _ => "Astra_Hemisphere_EquatorialSky".Translate()
      };
    }
    
    public static bool ShouldDrawGUI()
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