using Astralum.Astronomy.LocalStars;
using UnityEngine;

namespace Astralum.API
{
  /// <summary>
  /// Provides utility methods for retrieving visual colors associated with celestial objects.
  /// </summary>
  public static class CelestialColorGetter
  {
    /// <summary>
    /// Gets the display color corresponding to the specified stellar spectral classification.
    /// </summary>
    /// <param name="spectralClass">The spectral classification of the star.</param>
    /// <returns>The <see cref="Color"/> corresponding to the spectral class, or <see cref="Color.white"/> if unrecognized.</returns>
    public static Color TryGetStarColorFor(SpectralClass spectralClass)
    {
      return spectralClass switch
      {
        SpectralClass.O => new Color(0.62f, 0.78f, 1f, 1f),
        SpectralClass.B => new Color(0.70f, 0.85f, 1f, 1f),
        SpectralClass.A => new Color(0.85f, 0.93f, 1f, 1f),
        SpectralClass.F => new Color(1f, 0.98f, 0.88f, 1f),
        SpectralClass.G => new Color(1f, 0.90f, 0.62f, 1f),
        SpectralClass.K => new Color(1f, 0.62f, 0.32f, 1f),
        SpectralClass.M => new Color(1f, 0.34f, 0.22f, 1f),
        _ => Color.white
      };
    }
  }
}