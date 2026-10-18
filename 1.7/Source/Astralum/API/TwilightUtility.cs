using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.API
{
  /// <summary>
  /// Provides utility methods and threshold constants for calculating solar altitude
  /// and evaluating twilight and daylight phases across maps and planet tiles.
  /// </summary>
  public static class TwilightUtility
  {
    /// <summary>
    /// The solar altitude angle threshold (in degrees) defining the boundary of civil twilight (-6°).
    /// </summary>
    public const float CivilTwilightAltitude = -6f;

    /// <summary>
    /// The solar altitude angle threshold (in degrees) defining the boundary of nautical twilight (-12°).
    /// </summary>
    public const float NauticalTwilightAltitude = -12f;

    /// <summary>
    /// The solar altitude angle threshold (in degrees) defining the boundary of astronomical twilight (-18°).
    /// </summary>
    public const float AstronomicalTwilightAltitude = -18f;
    
    /// <summary>
    /// Calculates the solar altitude angle (in degrees above or below the local horizon) for the given map.
    /// </summary>
    /// <param name="map">The map whose tile is evaluated.</param>
    /// <returns>The sun's altitude angle in degrees (-90 to +90).</returns>
    public static float SunAltitude(Map map)
    {
      return SunAltitude(map.Tile);
    }
    
    /// <summary>
    /// Determines the current twilight or daylight phase for the specified map.
    /// </summary>
    /// <param name="map">The map to evaluate.</param>
    /// <returns>The active <see cref="TwilightPeriod"/> on the map.</returns>
    public static TwilightPeriod GetTwilightPeriod(Map map)
    {
      return GetTwilightPeriod(map.Tile);
    }

    /// <summary>
    /// Calculates the solar altitude angle (in degrees above or below the local horizon) at the specified planet tile.
    /// </summary>
    /// <param name="tile">The planet tile to evaluate.</param>
    /// <returns>The sun's altitude angle in degrees (-90 to +90).</returns>
    public static float SunAltitude(PlanetTile tile)
    {
      Vector3 tileNormal = Find.WorldGrid.GetTileCenter(tile).normalized;
      Vector3 sunDirection = GenCelestial.CurSunPositionInWorldSpace().normalized;
      
      float dot = Mathf.Clamp(Vector3.Dot(tileNormal, sunDirection), -1f, 1f);
      
      return Mathf.Asin(dot) * Mathf.Rad2Deg;
    }
    
    /// <summary>
    /// Determines the current twilight or daylight phase at the specified planet tile based on solar altitude.
    /// </summary>
    /// <param name="tile">The planet tile to evaluate.</param>
    /// <returns>The active <see cref="TwilightPeriod"/> at the planet tile.</returns>
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
    
    /// <summary>
    /// Determines whether the sun is currently at or above the horizon on the specified map.
    /// </summary>
    /// <param name="map">The map to check.</param>
    /// <returns><c>true</c> if the solar altitude is 0° or higher; otherwise, <c>false</c>.</returns>
    public static bool IsDay(Map map)
    {
      return IsDay(map.Tile);
    }
    
    /// <summary>
    /// Determines whether the sun is currently at or above the horizon at the specified planet tile.
    /// </summary>
    /// <param name="tile">The planet tile to check.</param>
    /// <returns><c>true</c> if the solar altitude is 0° or higher; otherwise, <c>false</c>.</returns>
    public static bool IsDay(PlanetTile tile)
    {
      return SunAltitude(tile) >= 0f;
    }
    
    /// <summary>
    /// Determines whether the specified map is currently in civil twilight (solar altitude between 0° and -6°).
    /// </summary>
    /// <param name="map">The map to check.</param>
    /// <returns><c>true</c> if in civil twilight; otherwise, <c>false</c>.</returns>
    public static bool IsCivilTwilight(Map map)
    {
      return IsCivilTwilight(map.Tile);
    }
    
    /// <summary>
    /// Determines whether the specified planet tile is currently in civil twilight (solar altitude between 0° and -6°).
    /// </summary>
    /// <param name="tile">The planet tile to check.</param>
    /// <returns><c>true</c> if in civil twilight; otherwise, <c>false</c>.</returns>
    public static bool IsCivilTwilight(PlanetTile tile)
    {
      float altitude = SunAltitude(tile);
      
      return altitude is < 0f and >= CivilTwilightAltitude;
    }
    
    /// <summary>
    /// Determines whether the specified map is currently in nautical twilight (solar altitude between -6° and -12°).
    /// </summary>
    /// <param name="map">The map to check.</param>
    /// <returns><c>true</c> if in nautical twilight; otherwise, <c>false</c>.</returns>
    public static bool IsNauticalTwilight(Map map)
    {
      return IsNauticalTwilight(map.Tile);
    }
    
    /// <summary>
    /// Determines whether the specified planet tile is currently in nautical twilight (solar altitude between -6° and -12°).
    /// </summary>
    /// <param name="tile">The planet tile to check.</param>
    /// <returns><c>true</c> if in nautical twilight; otherwise, <c>false</c>.</returns>
    public static bool IsNauticalTwilight(PlanetTile tile)
    {
      float altitude = SunAltitude(tile);
      
      return altitude is < CivilTwilightAltitude and >= NauticalTwilightAltitude;
    }
    
    /// <summary>
    /// Determines whether the specified map is currently in astronomical twilight (solar altitude between -12° and -18°).
    /// </summary>
    /// <param name="map">The map to check.</param>
    /// <returns><c>true</c> if in astronomical twilight; otherwise, <c>false</c>.</returns>
    public static bool IsAstronomicalTwilight(Map map)
    {
      return IsAstronomicalTwilight(map.Tile);
    }
    
    /// <summary>
    /// Determines whether the specified planet tile is currently in astronomical twilight (solar altitude between -12° and -18°).
    /// </summary>
    /// <param name="tile">The planet tile to check.</param>
    /// <returns><c>true</c> if in astronomical twilight; otherwise, <c>false</c>.</returns>
    public static bool IsAstronomicalTwilight(PlanetTile tile)
    {
      float altitude = SunAltitude(tile);

      return altitude is < NauticalTwilightAltitude and >= AstronomicalTwilightAltitude;
    }
    
    /// <summary>
    /// Determines whether the specified map is currently in night (solar altitude below -18°).
    /// </summary>
    /// <param name="map">The map to check.</param>
    /// <returns><c>true</c> if in night; otherwise, <c>false</c>.</returns>
    public static bool IsNight(Map map)
    {
      return IsNight(map.Tile);
    }
    
    /// <summary>
    /// Determines whether the specified planet tile is currently in night (solar altitude below -18°).
    /// </summary>
    /// <param name="tile">The planet tile to check.</param>
    /// <returns><c>true</c> if in night; otherwise, <c>false</c>.</returns>
    public static bool IsNight(PlanetTile tile)
    {
      return SunAltitude(tile) < AstronomicalTwilightAltitude;
    }
  }
}