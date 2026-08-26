using System.Collections.Generic;
using Astralum.Astronomy;
using Astralum.Astronomy.BlackHoles;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.Nebulae;
using Astralum.Astronomy.Pulsars;
using Verse;

namespace Astralum.UI
{
  public static class CelestialNamingRegistry
  {
    public static List<CelestialNamingObjectEntry> BuildEntries()
    {
      List<CelestialNamingObjectEntry> entries = [];

      AddEntries(entries, BlackHoleDataUtil.Data?.BlackHoles, 
        "Astra_UI_CelestialNamingBlackHolesCategory".Translate());
      AddEntries(entries, PulsarDataUtil.Data?.Pulsars,
        "Astra_UI_CelestialNamingPulsarsCategory".Translate());
      AddEntries(entries, NebulaDataUtil.Data?.Nebulas,
        "Astra_UI_CelestialNamingNebulaeCategory".Translate());
      AddConstellations(entries);
      
      return entries;
    }
    
    private static void AddEntries<T>(List<CelestialNamingObjectEntry> entries, IList<T> objects, string categoryLabel)
      where T : SavedPlayerNameableCelestialObject
    {
      if (objects == null || objects.Count == 0)
        return;
      
      for (int i = 0; i < objects.Count; i++)
      {
        T celestialObject = objects[i];
        
        if (celestialObject == null)
          continue;
        
        entries.Add(new CelestialNamingObjectEntry(
            categoryLabel,
            celestialObject.Id,
            celestialObject,
            celestialObject.LocalSkyPosition)
        );
      }
    }
    
    private static void AddConstellations(List<CelestialNamingObjectEntry> entries)
    {
      List<SavedConstellation> constellations = ConstellationDataUtil.Data?.Constellations;
      
      if (constellations.NullOrEmpty())
        return;
      
      AddEntries(entries, constellations, 
        "Astra_UI_CelestialNamingConstellationsCategory".Translate());
      
      for (int i = 0; i < constellations!.Count; i++)
      {
        SavedConstellation constellation = constellations[i];
        
        AddEntries(entries, constellation.stars, 
          "Astra_UI_CelestialNamingConstellationStarsCategory".Translate());
      }
    }
  }
}