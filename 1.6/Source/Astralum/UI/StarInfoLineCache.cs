using System.Collections.Generic;
using Astralum.Astronomy.LocalSystem.Stars;
using Verse;

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
        new(StellarNamingUtil.SafeName(star.starName, "Astra_Stars_Unknown".Translate())),
        new("Astra_Stars_System".Translate() + $": {StellarNamingUtil.SafeName(star.systemName, 
          "Astra_Stars_UnknownSystem".Translate())}"),

        new("Astra_Stars_Class".Translate() + $": {star.spectralClass}"),
        new("Astra_Stars_Age".Translate() + $": {StellarAgeUtil.FormatAge(star.age)}"),
        new("Astra_Stars_Temperature".Translate() + 
            $": {StellarTemperatureUtil.FormatTemperature(star.temperatureKelvin)}"),
        new("Astra_Stars_Rotation".Translate() + $": {StellarRotationUtil.FormatRotation(star.rotation)}"),
        new("Astra_Stars_Magnetic_Field".Translate() +
            $": {StellarMagneticFieldUtil.FormatMagneticField(star.magneticField)}"),
        new("Astra_Stars_Variability".Translate() + $": {StellarVariabilityUtil.FormatVariability(
          star.variabilityType, star.variabilityAmount)}"),
        new("Astra_Stars_Radius".Translate() + $": {StellarRadiusUtil.FormatRadius(star.radius)}"),
        new("Astra_Stars_Luminosity".Translate() 
            + $": {StellarLuminosityUtil.FormatLuminosity(star.luminosity)}"),
        new("Astra_Stars_Mass".Translate() + $": {StellarMassUtil.FormatMass(star.mass)}"),
        new("Astra_Stars_Metallicity".Translate() 
            + $": {StellarCompositionUtil.FormatMetallicity(star.metallicity)}"),
        new("Astra_Stars_Composition".Translate() + ":")
      ];
      
      foreach (string compositionLine in StellarCompositionUtil.FormatCompositionLines(
                 star.composition, 3))
        lines.Add(new StarInfoLine($"  {compositionLine}"));
      
      lines.Add(new StarInfoLine("Astra_Stars_Chromaticity".Translate() + ":", star.chromaticity));
      lines.Add(new StarInfoLine("Astra_Stars_Corona_Glow".Translate() + ":", star.corona));
      lines.Add(new StarInfoLine("Astra_Stars_Corona_Intensity".Translate() + ": " +
                                 $"{StellarCoronaUtil.FormatCoronaIntensity(star.coronaIntensity)}"));
      
      return lines;
    }
  }
}