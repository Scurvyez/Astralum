using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.API
{
  public static class TwilightUtility
  {
    public const float CivilTwilightAltitude = -6f;
    public const float NauticalTwilightAltitude = -12f;
    public const float AstronomicalTwilightAltitude = -18f;
    
    public static float SunAltitude(Map map)
    {
      return SunAltitude(map.Tile);
    }
    
    public static float SunAltitude(PlanetTile tile)
    {
      Vector3 tileNormal = Find.WorldGrid.GetTileCenter(tile).normalized;
      Vector3 sunDirection = GenCelestial.CurSunPositionInWorldSpace().normalized;
      
      float dot = Mathf.Clamp(Vector3.Dot(tileNormal, sunDirection), -1f, 1f);
      
      return Mathf.Asin(dot) * Mathf.Rad2Deg;
    }
    
    public static TwilightPeriod GetTwilightPeriod(Map map)
    {
      return GetTwilightPeriod(map.Tile);
    }
    
    public static TwilightPeriod GetTwilightPeriod(PlanetTile tile)
    {
      float altitude = SunAltitude(tile);
      
      return altitude switch
      {
        >= 0f => TwilightPeriod.Day,
        >= CivilTwilightAltitude => TwilightPeriod.Civil,
        >= NauticalTwilightAltitude => TwilightPeriod.Nautical,
        >= AstronomicalTwilightAltitude => TwilightPeriod.Astronomical,
        _ => TwilightPeriod.Night
      };
    }
    
    public static bool IsDay(Map map)
    {
      return IsDay(map.Tile);
    }
    
    public static bool IsDay(PlanetTile tile)
    {
      return SunAltitude(tile) >= 0f;
    }
    
    public static bool IsCivilTwilight(Map map)
    {
      return IsCivilTwilight(map.Tile);
    }
    
    public static bool IsCivilTwilight(PlanetTile tile)
    {
      float altitude = SunAltitude(tile);
      
      return altitude is < 0f and >= CivilTwilightAltitude;
    }
    
    public static bool IsNauticalTwilight(Map map)
    {
      return IsNauticalTwilight(map.Tile);
    }
    
    public static bool IsNauticalTwilight(PlanetTile tile)
    {
      float altitude = SunAltitude(tile);
      
      return altitude is < CivilTwilightAltitude and >= NauticalTwilightAltitude;
    }
    
    public static bool IsAstronomicalTwilight(Map map)
    {
      return IsAstronomicalTwilight(map.Tile);
    }
    
    public static bool IsAstronomicalTwilight(PlanetTile tile)
    {
      float altitude = SunAltitude(tile);

      return altitude is < NauticalTwilightAltitude and >= AstronomicalTwilightAltitude;
    }
    
    public static bool IsNight(Map map)
    {
      return IsNight(map.Tile);
    }
    
    public static bool IsNight(PlanetTile tile)
    {
      return SunAltitude(tile) < AstronomicalTwilightAltitude;
    }
  }
}