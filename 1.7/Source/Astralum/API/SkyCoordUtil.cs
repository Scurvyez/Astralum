using UnityEngine;
using Verse;

namespace Astralum.API
{
  /// <summary>
  /// Provides utility methods and constants for celestial coordinate conversions, hemisphere categorization,
  /// latitude projections, and coordinate formatting in equatorial astronomical systems.
  /// </summary>
  public static class SkyCoordUtil
  {
    /// <summary>
    /// The normalized vertical direction threshold above which a position is considered part of the northern celestial hemisphere.
    /// </summary>
    public const float NorthernSkyThreshold = 0.15f;

    /// <summary>
    /// The normalized vertical direction threshold below which a position is considered part of the southern celestial hemisphere.
    /// </summary>
    public const float SouthernSkyThreshold = -0.15f;
    
    /// <summary>
    /// Converts a 3D directional vector in world or celestial space into equatorial sky coordinates (right ascension and declination).
    /// </summary>
    /// <param name="direction">The 3D directional vector pointing toward the celestial object or position.</param>
    /// <returns>A <see cref="SkyCoord"/> representing the corresponding right ascension in hours and declination in degrees.</returns>
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
    
    /// <summary>
    /// Calculates the normalized vertical celestial sphere height for a given latitude or declination angle.
    /// </summary>
    /// <param name="degrees">The latitude or declination angle in degrees.</param>
    /// <returns>The normalized vertical height along the Y axis.</returns>
    public static float SkyLatitudeHeight(float degrees)
    {
      return Mathf.Sin(degrees * Mathf.Deg2Rad);
    }

    /// <summary>
    /// Determines the localized celestial hemisphere name (Northern, Southern, or Equatorial) for a given 3D directional vector.
    /// </summary>
    /// <param name="direction">The directional vector pointing toward the celestial position.</param>
    /// <returns>A localized string representation of the celestial hemisphere.</returns>
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
    
    /// <summary>
    /// Formats a right ascension value into a standardized astronomical hours and minutes string (e.g., <c>"14h 30m"</c>).
    /// </summary>
    /// <param name="hours">The right ascension value in hours (automatically wrapped within [0, 24) hours).</param>
    /// <returns>A formatted string representing right ascension in hours and minutes.</returns>
    public static string FormatRightAscension(float hours)
    {
      hours = Mathf.Repeat(hours, 24f);
      
      int h = Mathf.FloorToInt(hours);
      int m = FractionalMinutes(hours, h);
      
      return $"{h:00}h {m:00}m";
    }
    
    /// <summary>
    /// Formats a declination value into a standardized astronomical degrees and arcminutes string with sign prefix (e.g., <c>"+45° 15'"</c>).
    /// </summary>
    /// <param name="degrees">The declination value in degrees.</param>
    /// <returns>A formatted string representing declination in degrees and arcminutes with explicit sign.</returns>
    public static string FormatDeclination(float degrees)
    {
      string sign = degrees >= 0f ? "+" : "-";
      degrees = Mathf.Abs(degrees);
      
      int d = Mathf.FloorToInt(degrees);
      int m = FractionalMinutes(degrees, d);
      
      return $"{sign}{d:00}° {m:00}'";
    }
    
    /// <summary>
    /// Extracts the fractional component of a floating-point unit value and converts it into whole minutes of arc or time.
    /// </summary>
    /// <param name="value">The full floating-point value.</param>
    /// <param name="wholeUnits">The integer floor unit component.</param>
    /// <returns>The calculated fractional minutes as an integer.</returns>
    private static int FractionalMinutes(float value, int wholeUnits)
    {
      return Mathf.FloorToInt((value - wholeUnits) * 60f);
    }
  }
}