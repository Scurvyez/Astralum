using System.Collections.Generic;
using Astralum.Astronomy.Pulsars;
using Verse;

namespace Astralum.UI
{
  public static class PulsarHoverInfoLineCache
  {
    private static PulsarInteractionRegistry.HoverPulsar? _cachedPulsar;
    private static List<PulsarHoverInfoLine> _cachedLines;
    
    public static List<PulsarHoverInfoLine> GetLines(PulsarInteractionRegistry.HoverPulsar pulsar)
    {
      if (_cachedPulsar.HasValue &&
          _cachedPulsar.Value.localSkyPos == pulsar.localSkyPos &&
          _cachedLines != null)
        return _cachedLines;
      
      _cachedPulsar = pulsar;
      _cachedLines = BuildLines(pulsar);
      
      return _cachedLines;
    }
    
    public static void Clear()
    {
      _cachedPulsar = null;
      _cachedLines = null;
    }
    
    private static List<PulsarHoverInfoLine> BuildLines(PulsarInteractionRegistry.HoverPulsar pulsar)
    {
      return
      [
        new PulsarHoverInfoLine(pulsar.name.NullOrEmpty()
          ? "Astra_Pulsar_Unknown".Translate()
          : pulsar.name),
        
        new PulsarHoverInfoLine("Astra_Pulsars_Type".Translate()),
        new PulsarHoverInfoLine("Astra_Objects_Region".Translate() + $" {pulsar.hemisphere}"),
        new PulsarHoverInfoLine($"RA: {pulsar.rightAscension}"),
        new PulsarHoverInfoLine($"Dec: {pulsar.declination}")
      ];
    }
  }
}