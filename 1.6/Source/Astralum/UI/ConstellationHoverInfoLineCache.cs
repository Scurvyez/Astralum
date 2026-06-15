using System.Collections.Generic;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.LocalSystem.Stars;
using Verse;

namespace Astralum.UI
{
  public static class ConstellationHoverInfoLineCache
  {
    private static ConstellationInteractionRegistry.HoverStar? _cachedStar;
    private static List<ConstellationHoverInfoLine> _cachedLines;

    public static List<ConstellationHoverInfoLine> GetLines(ConstellationInteractionRegistry.HoverStar star)
    {
      if (_cachedStar.HasValue &&
          _cachedStar.Value.localSkyPos == star.localSkyPos &&
          _cachedLines != null)
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

    private static List<ConstellationHoverInfoLine> BuildLines(ConstellationInteractionRegistry.HoverStar star)
    {
      return
      [
        new ConstellationHoverInfoLine(StellarNamingUtil.SafeName(
          star.name, "Astra_Stars_Unknown".Translate())),
        new ConstellationHoverInfoLine("Astra_Stars_Class".Translate() + $": {star.spectralClass}"),
        new ConstellationHoverInfoLine("Astra_Stars_Constellation".Translate() + $": {star.constellationName}"),
        new ConstellationHoverInfoLine("Astra_Stars_Region".Translate() + $": {star.hemisphere}"),
        new ConstellationHoverInfoLine($"RA: {star.rightAscension}"),
        new ConstellationHoverInfoLine($"Dec: {star.declination}")
      ];
    }
  }
}