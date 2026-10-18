namespace Astralum.API
{
  /// <summary>
  /// Represents celestial coordinates in the equatorial coordinate system, defined by right ascension and declination.
  /// </summary>
  public readonly struct SkyCoord
  {
    /// <summary>
    /// The right ascension coordinate in hours (typically ranging from 0 to 24 hours).
    /// </summary>
    public readonly float RightAscensionHours;

    /// <summary>
    /// The declination coordinate in degrees (ranging from -90 to +90 degrees relative to the celestial equator).
    /// </summary>
    public readonly float DeclinationDegrees;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SkyCoord"/> struct with the specified right ascension and declination coordinates.
    /// </summary>
    /// <param name="rightAscensionHours">The right ascension coordinate in hours.</param>
    /// <param name="declinationDegrees">The declination coordinate in degrees.</param>
    public SkyCoord(float rightAscensionHours, float declinationDegrees)
    {
      RightAscensionHours = rightAscensionHours;
      DeclinationDegrees = declinationDegrees;
    }
  }
}