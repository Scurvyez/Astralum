using System.Collections.Generic;
using Astralum.Astronomy.BlackHoles;
using Verse;

namespace Astralum.UI
{
  public static class BlackHoleHoverInfoLineCache
  {
    private static BlackHoleInteractionRegistry.HoverBlackHole? _cachedBlackHole;
    private static List<BlackHoleHoverInfoLine> _cachedLines;
    
    public static List<BlackHoleHoverInfoLine> GetLines(BlackHoleInteractionRegistry.HoverBlackHole blackHole)
    {
      if (_cachedBlackHole.HasValue &&
          _cachedBlackHole.Value.localSkyPos == blackHole.localSkyPos &&
          _cachedLines != null)
        return _cachedLines;
      
      _cachedBlackHole = blackHole;
      _cachedLines = BuildLines(blackHole);
      
      return _cachedLines;
    }
    
    public static void Clear()
    {
      _cachedBlackHole = null;
      _cachedLines = null;
    }
    
    private static List<BlackHoleHoverInfoLine> BuildLines(BlackHoleInteractionRegistry.HoverBlackHole blackHole)
    {
      Clear();
      
      SavedBlackHole saved = BlackHoleDataUtil.GetById(blackHole.id);
      string displayName = saved?.DisplayName ?? blackHole.name;
      
      return
      [
        new BlackHoleHoverInfoLine(displayName.NullOrEmpty()
          ? "Astra_Blackholes_Unknown".Translate()
          : displayName),
        
        new BlackHoleHoverInfoLine("Astra_Blackholes_Type".Translate()),
        new BlackHoleHoverInfoLine("Astra_Objects_Region".Translate() + $" {blackHole.hemisphere}"),
        new BlackHoleHoverInfoLine($"RA: {blackHole.rightAscension}"),
        new BlackHoleHoverInfoLine($"Dec: {blackHole.declination}")
      ];
    }
  }
}