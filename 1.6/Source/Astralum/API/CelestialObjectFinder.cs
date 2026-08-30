using System.Collections.Generic;
using Astralum.Astronomy;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.LocalStars;
using Astralum.World;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.API
{
  public static class CelestialObjectFinder
  {
    public static float AltitudeFor(ICelestialObject celestialObject, Map map)
    {
      return AltitudeFor(celestialObject, map.Tile);
    }
    
    public static float AltitudeFor(ICelestialObject celestialObject, PlanetTile tile)
    {
      if (celestialObject == null)
        return -90f;

      Vector3 tileNormal = Find.WorldGrid.GetTileCenter(tile).normalized;
      Vector3 objectDirection = WorldUtils.GetCurrentRotationForWorldSpace() * celestialObject.LocalSkyPosition.normalized;
      
      float dot = Mathf.Clamp(Vector3.Dot(tileNormal, objectDirection), -1f, 1f);
      
      return Mathf.Asin(dot) * Mathf.Rad2Deg;
    }
    
    public static bool IsVisible(ICelestialObject celestialObject, Map map)
    {
      return IsVisible(celestialObject, map.Tile);
    }
    
    public static bool IsVisible(ICelestialObject celestialObject, PlanetTile tile)
    {
      return AltitudeFor(celestialObject, tile) >= 0f;
    }

    public static List<CelestialObjectInfo> GetVisible(CelestialObjectType type, Map map)
    {
      return GetVisible(type, map.Tile);
    }
    
    public static List<CelestialObjectInfo> GetVisible(CelestialObjectType type, PlanetTile tile)
    {
      List<CelestialObjectInfo> results = [];
      List<ICelestialObject> objects = [];
      
      GetObjectsOfType(type, objects);
      
      for (int i = 0; i < objects.Count; i++)
      {
        ICelestialObject celestialObject = objects[i];
        
        if (!IsVisible(celestialObject, tile))
          continue;
        
        CelestialObjectInfo info = CelestialObjectInfoUtil.From(celestialObject);
        
        if (info.type == CelestialObjectType.Unknown)
          continue;
        
        results.Add(info);
      }

      return results;
    }

    public static bool TryGetVisible(CelestialObjectType type, Map map, out CelestialObjectInfo result)
    {
      return TryGetVisible(type, map.Tile, out result);
    }

    public static bool TryGetVisible(CelestialObjectType type, PlanetTile tile, out CelestialObjectInfo result)
    {
      List<ICelestialObject> objects = [];
      
      GetObjectsOfType(type, objects);
      
      for (int i = 0; i < objects.Count; i++)
      {
        ICelestialObject celestialObject = objects[i];
        
        if (!IsVisible(celestialObject, tile))
          continue;
        
        CelestialObjectInfo info = CelestialObjectInfoUtil.From(celestialObject);
        
        if (info.type == CelestialObjectType.Unknown)
          continue;
        
        result = info;
        return true;
      }
      
      result = default;
      return false;
    }
    
    public static bool TryGetRandomVisible(CelestialObjectType type, Map map, out CelestialObjectInfo result)
    {
      return TryGetRandomVisible(type, map.Tile, out result);
    }
    
    public static bool TryGetRandomVisible(CelestialObjectType type, PlanetTile tile, out CelestialObjectInfo result)
    {
      List<CelestialObjectInfo> visible = GetVisible(type, tile);
      
      if (visible.Count == 0)
      {
        result = default;
        return false;
      }
      
      result = visible.RandomElement();
      return true;
    }
    
    private static void GetObjectsOfType(CelestialObjectType type, List<ICelestialObject> results)
    {
      WorldComponent_CelestialObjectDataCache data = Find.World?.GetComponent<WorldComponent_CelestialObjectDataCache>();
      WorldComponent_ConstellationDataCache constellationData = Find.World?.GetComponent<WorldComponent_ConstellationDataCache>();
      
      switch (type)
      {
        case CelestialObjectType.LocalStar:
          AddRange(data?.LocalStars, results);
          break;
        case CelestialObjectType.Nebulae:
          AddRange(data?.Nebulae, results);
          break;
        case CelestialObjectType.BlackHole:
          AddRange(data?.BlackHoles, results);
          break;
        case CelestialObjectType.Pulsar:
          AddRange(data?.Pulsars, results);
          break;
        case CelestialObjectType.Constellation:
          AddRange(constellationData?.Constellations, results);
          break;
        case CelestialObjectType.ConstellationStar:
          AddConstellationStars(constellationData?.Constellations, results);
          break;
      }
    }
    
    private static void AddRange<T>(List<T> source, List<ICelestialObject> destination) where T : ICelestialObject
    {
      if (source == null)
        return;
      
      for (int i = 0; i < source.Count; i++)
      {
        if (source[i] != null)
          destination.Add(source[i]);
      }
    }
    
    private static void AddConstellationStars(List<SavedConstellation> constellations, List<ICelestialObject> destination)
    {
      if (constellations == null)
        return;
      
      for (int i = 0; i < constellations.Count; i++)
      {
        List<SavedConstellationStar> stars = constellations[i]?.stars;
        
        if (stars == null)
          continue;
        
        for (int j = 0; j < stars.Count; j++)
        {
          if (stars[j] != null)
            destination.Add(stars[j]);
        }
      }
    }
    
    public static Vector3 CurrentLocalSkyPositionFor(ICelestialObject celestialObject)
    {
      if (celestialObject is SavedLocalStar localStar)
        return LocalStarOrbitUtil.PositionFor(localStar);
      
      return celestialObject.LocalSkyPosition;
    }
  }
}