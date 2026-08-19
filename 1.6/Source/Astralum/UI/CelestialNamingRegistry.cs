using System.Collections.Generic;
using Astralum.Astronomy.BlackHoles;
using Astralum.World;
using Verse;

namespace Astralum.UI
{
  public static class CelestialNamingRegistry
  {
    public static List<CelestialNamingObjectEntry> BuildEntries()
    {
      List<CelestialNamingObjectEntry> entries = [];
      
      AddBlackHoles(entries);
      //AddConstellations(entries);
      //AddNebulae(entries);
      //AddPulsars(entries);
      //AddLocalStar(entries);
      
      return entries;
    }
    
    // TODO: add keyed lang strings for the categories and everything else
    
    private static void AddBlackHoles(List<CelestialNamingObjectEntry> entries)
    {
      WorldComponent_BlackHoleData data = BlackHoleDataUtil.Data;
      
      if (data?.BlackHoles.NullOrEmpty() != false)
        return;
      
      for (int i = 0; i < data.BlackHoles.Count; i++)
      {
        SavedBlackHole blackHole = data.BlackHoles[i];
        
        entries.Add(new CelestialNamingObjectEntry(
          "Black Holes",
          blackHole.id.ToString(),
          blackHole,
          blackHole.localSkyPos));
      }
    }

    /*private static void AddPulsars(List<CelestialNamingObjectEntry> entries)
    {
      WorldComponent_PulsarData data = PulsarDataUtil.Data;
      
      if (data?.pulsars.NullOrEmpty() != false)
        return;

      for (int i = 0; i < data.pulsars.Count; i++)
      {
        SavedPulsar pulsar = data.pulsars[i];
        
        entries.Add(new CelestialNamingObjectEntry(
          "Pulsars",
          pulsar.pulsarId.ToString(),
          pulsar,
          pulsar.localSkyPos));
      }
    }*/
  }
}