using System.Collections.Generic;
using Astralum.Astronomy.LocalSystem.Stars;

namespace Astralum.UI
{
  public static class StarInfoLineCache
  {
    private static SavedStar _cachedStar;
    private static List<StarInfoLine> _cachedLines;

    public static List<StarInfoLine> GetLines(SavedStar star)
    {
      if (star == null)
        return [];

      if (_cachedStar == star && _cachedLines != null)
        return _cachedLines;

      _cachedStar = star;
      _cachedLines = BuildLines(star);

      return _cachedLines;
    }

    public static void Clear()
    {
      _cachedStar = null;
      _cachedLines = null;
    }

    public static void Rebuild(SavedStar star)
    {
      _cachedStar = star;
      _cachedLines = BuildLines(star);
    }

    private static List<StarInfoLine> BuildLines(SavedStar star)
    {
      List<StarInfoLine> lines =
      [
        new(StellarNamingUtil.SafeName(star.starName, "Unknown Star")),
        new($"System: {StellarNamingUtil.SafeName(star.systemName, "Unknown System")}"),

        new($"Class: {star.spectralClass}"),
        new($"Age: {StellarAgeUtil.FormatAge(star.age)}"),
        new($"Temperature: {StellarTemperatureUtil.FormatTemperature(star.temperatureKelvin)}"),
        new($"Rotation: {StellarRotationUtil.FormatRotation(star.rotation)}"),
        new($"Magnetic Field: {StellarMagneticFieldUtil.FormatMagneticField(star.magneticField)}"),
        new($"Variability: {StellarVariabilityUtil.FormatVariability(
          star.variabilityType, star.variabilityAmount)}"),
        new($"Radius: {StellarRadiusUtil.FormatRadius(star.radius)}"),
        new($"Luminosity: {StellarLuminosityUtil.FormatLuminosity(star.luminosity)}"),
        new($"Mass: {StellarMassUtil.FormatMass(star.mass)}"),
        new($"Metallicity: {StellarCompositionUtil.FormatMetallicity(star.metallicity)}"),
        new("Composition:")
      ];

      foreach (string compositionLine in StellarCompositionUtil.FormatCompositionLines(
                 star.composition, 3))
        lines.Add(new StarInfoLine($"  {compositionLine}"));

      lines.Add(new StarInfoLine("Chromaticity:", star.chromaticity));
      lines.Add(new StarInfoLine("Corona Glow:", star.corona));
      lines.Add(new StarInfoLine($"Corona Intensity: " +
                                 $"{StellarCoronaUtil.FormatCoronaIntensity(star.coronaIntensity)}"));

      return lines;
    }
  }
}