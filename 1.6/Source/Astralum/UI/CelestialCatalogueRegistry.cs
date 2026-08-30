using System.Collections.Generic;
using Astralum.Astronomy;
using Astralum.Astronomy.BlackHoles;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.LocalStars;
using Astralum.Astronomy.Nebulae;
using Astralum.Astronomy.Pulsars;
using UnityEngine;
using Verse;

namespace Astralum.UI
{
  public static class CelestialCatalogueRegistry
  {
    public static List<CelestialCatalogueObjectEntry> BuildEntries()
    {
      List<CelestialCatalogueObjectEntry> entries = [];
      
      AddEntries(entries, LocalStarDataUtil.Data?.LocalStars,
        "Astra_UI_CelestialNamingLocalStarsCategory".Translate());
      AddConstellations(entries);
      AddEntries(entries, NebulaDataUtil.Data?.Nebulae,
        "Astra_UI_CelestialNamingNebulaeCategory".Translate());
      AddEntries(entries, BlackHoleDataUtil.Data?.BlackHoles, 
        "Astra_UI_CelestialNamingBlackHolesCategory".Translate());
      AddEntries(entries, PulsarDataUtil.Data?.Pulsars,
        "Astra_UI_CelestialNamingPulsarsCategory".Translate());
      
      return entries;
    }
    
    private static void AddEntries<T>(List<CelestialCatalogueObjectEntry> entries, IList<T> objects, string categoryLabel)
      where T : SavedPlayerNameableCelestialObject
    {
      if (objects == null || objects.Count == 0)
        return;
      
      for (int i = 0; i < objects.Count; i++)
      {
        T celestialObject = objects[i];
        
        if (celestialObject == null)
          continue;
        
        Vector3 position = celestialObject is SavedLocalStar localStar
          ? LocalStarOrbitUtil.PositionFor(localStar)
          : celestialObject.LocalSkyPosition;
        
        entries.Add(new CelestialCatalogueObjectEntry(
            categoryLabel,
            celestialObject.Id,
            celestialObject,
            position)
        );
      }
    }
    
    private static void AddConstellations(List<CelestialCatalogueObjectEntry> entries)
    {
      List<SavedConstellation> constellations = ConstellationDataUtil.Data?.Constellations;
      
      if (constellations.NullOrEmpty())
        return;
      
      string constellationCategory = "Astra_UI_CelestialNamingConstellationsCategory".Translate();
      string starCategory = "Astra_UI_CelestialNamingConstellationStarsCategory".Translate();
      
      for (int i = 0; i < constellations!.Count; i++)
      {
        SavedConstellation constellation = constellations[i];
        
        entries.Add(new CelestialCatalogueObjectEntry(
          constellationCategory,
          constellation.Id,
          constellation,
          constellation.LocalSkyPosition
        ));
        
        if (constellation.stars.NullOrEmpty())
          continue;
        
        for (int j = 0; j < constellation.stars.Count; j++)
        {
          SavedConstellationStar star = constellation.stars[j];
          
          entries.Add(new CelestialCatalogueObjectEntry(
            starCategory,
            star.Id,
            star,
            star.LocalSkyPosition,
            constellation.Id));
        }
      }
    }
  }
}