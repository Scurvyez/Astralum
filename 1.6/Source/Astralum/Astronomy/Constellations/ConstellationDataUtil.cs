using System.Collections.Generic;
using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public static class ConstellationDataUtil
  {
    public static WorldComponent_ConstellationDataCache Data => Find.World.GetComponent<WorldComponent_ConstellationDataCache>();

    public static SavedConstellation Create(string id, Vector3 dir, float size, float rotationDegrees, 
      HashSet<string> usedNames, ConstellationMaskInfo maskInfo, Texture2D mask)
    {
      string generatedName = ConstellationNameGenerator.Generate(maskInfo.categoryId, usedNames);
      
      return CelestialObjectDataUtil.CreateNameable<SavedConstellation>(id, dir, size, generatedName, rotationDegrees,
        constellation =>
        {
          constellation.categoryId = maskInfo.categoryId;
          constellation.maskName = mask.name;
          constellation.centerDir = dir.normalized;
          constellation.stars = [];
        }
      );
    }
    
    public static SavedConstellationStar GetStarById(string id)
    {
      WorldComponent_ConstellationDataCache data = Data;
      
      if (data?.Constellations.NullOrEmpty() != false)
        return null;
      
      for (int i = 0; i < data.Constellations.Count; i++)
      {
        SavedConstellation constellation = data.Constellations[i];
        
        if (constellation.stars.NullOrEmpty())
          continue;
        
        for (int j = 0; j < constellation.stars.Count; j++)
        {
          SavedConstellationStar star = constellation.stars[j];
          
          if (star.Id == id)
            return star;
        }
      }
      
      return null;
    }

    public static SavedConstellation GetConstellationForStar(string id)
    {
      WorldComponent_ConstellationDataCache data = Data;
      
      if (data?.Constellations.NullOrEmpty() != false)
        return null;
      
      for (int i = 0; i < data.Constellations.Count; i++)
      {
        SavedConstellation constellation = data.Constellations[i];
        
        if (constellation.stars.NullOrEmpty())
          continue;
        
        for (int j = 0; j < constellation.stars.Count; j++)
        {
          if (constellation.stars[j].Id == id)
            return constellation;
        }
      }
      
      return null;
    }
    
    public static SavedConstellation GetById(string id)
    {
      return Data?.Constellations.GetById(id);
    }
  }
}