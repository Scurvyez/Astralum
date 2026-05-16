using System.Collections.Generic;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.LocalSystem.Stars;

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
        new ConstellationHoverInfoLine(StellarNamingUtil.SafeName(star.name, "Unknown Star")),
        new ConstellationHoverInfoLine($"Class: {star.spectralClass}"),
        new ConstellationHoverInfoLine($"Constellation: {star.constellationName}"),
        new ConstellationHoverInfoLine($"Region: {star.hemisphere}"),
        new ConstellationHoverInfoLine($"RA: {star.rightAscension}"),
        new ConstellationHoverInfoLine($"Dec: {star.declination}")
      ];
    }
  }
}