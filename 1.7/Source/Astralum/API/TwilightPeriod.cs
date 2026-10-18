namespace Astralum.API
{
  /// <summary>
  /// Specifies the daylight and twilight phase of the sun relative to the horizon based on solar altitude angle.
  /// </summary>
  public enum TwilightPeriod
  {
    /// <summary>
    /// Full daylight; the sun is at or above the horizon (altitude >= 0 degrees).
    /// </summary>
    Day,

    /// <summary>
    /// Civil twilight; the center of the sun is between 0 and -6 degrees below the horizon.
    /// Terrestrial objects are clearly distinguishable and artificial illumination is generally unnecessary.
    /// </summary>
    Civil,

    /// <summary>
    /// Nautical twilight; the center of the sun is between -6 and -12 degrees below the horizon.
    /// Navigational horizons remain faintly discernible and primary navigational stars are visible.
    /// </summary>
    Nautical,

    /// <summary>
    /// Astronomical twilight; the center of the sun is between -12 and -18 degrees below the horizon.
    /// The sky is dark enough for astronomical observation of celestial bodies and deep-sky objects.
    /// </summary>
    Astronomical,

    /// <summary>
    /// True night; the center of the sun is lower than -18 degrees below the horizon.
    /// Complete atmospheric darkness with no residual solar scattering.
    /// </summary>
    Night
  }
}