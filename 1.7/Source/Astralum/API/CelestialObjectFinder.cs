using System.Collections.Generic;
using Astralum.Astronomy;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.LocalStars;
using Astralum.WorldComponents;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.API
{
  /// <summary>
  /// Provides utility methods for querying celestial object positions, calculating horizon altitude,
  /// evaluating visibility, and retrieving visible objects of specific types across maps and planet tiles.
  /// </summary>
  public static class CelestialObjectFinder
  {
    /// <summary>
    /// Calculates the altitude angle (in degrees above or below the local horizon) of a celestial object from the vantage point of a map.
    /// </summary>
    /// <param name="celestialObject">The celestial object to evaluate.</param>
    /// <param name="map">The map whose tile is used as the observer's position.</param>
    /// <returns>The altitude angle in degrees (-90 to +90). Positive values indicate the object is above the horizon.</returns>
    public static float AltitudeFor(ICelestialObject celestialObject, Map map)
    {
      return AltitudeFor(celestialObject, map.Tile);
    }
    
    /// <summary>
    /// Calculates the altitude angle (in degrees above or below the local horizon) of a celestial object from a specific planet tile.
    /// </summary>
    /// <param name="celestialObject">The celestial object to evaluate.</param>
    /// <param name="tile">The planet tile used as the observer's position.</param>
    /// <returns>The altitude angle in degrees (-90 to +90). Returns -90 if the object is <c>null</c>.</returns>
    public static float AltitudeFor(ICelestialObject celestialObject, PlanetTile tile)
    {
      if (celestialObject == null)
        return -90f;
      
      Vector3 tileNormal = Find.WorldGrid.GetTileCenter(tile).normalized;
      Vector3 localSkyPosition = CurrentLocalSkyPositionFor(celestialObject);
      Vector3 objectDirection = CelestialTransformUtil.GetCurrentRotationForWorldSpace() * localSkyPosition.normalized;
      
      float dot = Mathf.Clamp(Vector3.Dot(tileNormal, objectDirection), -1f, 1f);
      
      return Mathf.Asin(dot) * Mathf.Rad2Deg;
    }
    
    /// <summary>
    /// Determines whether a celestial object is currently above the local horizon on the specified map.
    /// </summary>
    /// <param name="celestialObject">The celestial object to evaluate.</param>
    /// <param name="map">The map whose tile is used as the observer's position.</param>
    /// <returns><c>true</c> if the object is at or above the horizon (altitude &gt;= 0°); otherwise, <c>false</c>.</returns>
    public static bool IsVisible(ICelestialObject celestialObject, Map map)
    {
      return IsVisible(celestialObject, map.Tile);
    }
    
    /// <summary>
    /// Determines whether a celestial object is currently above the local horizon at the specified planet tile.
    /// </summary>
    /// <param name="celestialObject">The celestial object to evaluate.</param>
    /// <param name="tile">The planet tile used as the observer's position.</param>
    /// <returns><c>true</c> if the object is at or above the horizon (altitude &gt;= 0°); otherwise, <c>false</c>.</returns>
    public static bool IsVisible(ICelestialObject celestialObject, PlanetTile tile)
    {
      return AltitudeFor(celestialObject, tile) >= 0f;
    }

    /// <summary>
    /// Retrieves information for all celestial objects of the specified type that are currently above the horizon on the given map.
    /// </summary>
    /// <param name="type">The type of celestial objects to search for.</param>
    /// <param name="map">The map to evaluate visibility for.</param>
    /// <returns>A list of <see cref="CelestialObjectInfo"/> descriptors for all visible objects matching the type.</returns>
    public static List<CelestialObjectInfo> GetVisible(CelestialObjectType type, Map map)
    {
      return GetVisible(type, map.Tile);
    }
    
    /// <summary>
    /// Retrieves information for all celestial objects of the specified type that are currently above the horizon at the given planet tile.
    /// </summary>
    /// <param name="type">The type of celestial objects to search for.</param>
    /// <param name="tile">The planet tile to evaluate visibility for.</param>
    /// <returns>A list of <see cref="CelestialObjectInfo"/> descriptors for all visible objects matching the type.</returns>
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
        
        if (info.Type == CelestialObjectType.Unknown)
          continue;
        
        results.Add(info);
      }

      return results;
    }

    /// <summary>
    /// Attempts to retrieve the first celestial object of the specified type that is currently visible on the given map.
    /// </summary>
    /// <param name="type">The type of celestial object to search for.</param>
    /// <param name="map">The map to evaluate visibility for.</param>
    /// <param name="result">Outputs the <see cref="CelestialObjectInfo"/> of the first visible object if found; otherwise, default.</param>
    /// <returns><c>true</c> if at least one matching object is visible; otherwise, <c>false</c>.</returns>
    public static bool TryGetVisible(CelestialObjectType type, Map map, out CelestialObjectInfo result)
    {
      return TryGetVisible(type, map.Tile, out result);
    }

    /// <summary>
    /// Attempts to retrieve the first celestial object of the specified type that is currently visible at the given planet tile.
    /// </summary>
    /// <param name="type">The type of celestial object to search for.</param>
    /// <param name="tile">The planet tile to evaluate visibility for.</param>
    /// <param name="result">Outputs the <see cref="CelestialObjectInfo"/> of the first visible object if found; otherwise, default.</param>
    /// <returns><c>true</c> if at least one matching object is visible; otherwise, <c>false</c>.</returns>
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
        
        if (info.Type == CelestialObjectType.Unknown)
          continue;
        
        result = info;
        return true;
      }
      
      result = default;
      return false;
    }
    
    /// <summary>
    /// Attempts to select a random visible celestial object of the specified type on the given map.
    /// </summary>
    /// <param name="type">The type of celestial object to search for.</param>
    /// <param name="map">The map to evaluate visibility for.</param>
    /// <param name="result">Outputs the randomly selected <see cref="CelestialObjectInfo"/> if any are visible; otherwise, default.</param>
    /// <returns><c>true</c> if at least one matching object is visible and one was selected; otherwise, <c>false</c>.</returns>
    public static bool TryGetRandomVisible(CelestialObjectType type, Map map, out CelestialObjectInfo result)
    {
      return TryGetRandomVisible(type, map.Tile, out result);
    }
    
    /// <summary>
    /// Attempts to select a random visible celestial object of the specified type at the given planet tile.
    /// </summary>
    /// <param name="type">The type of celestial object to search for.</param>
    /// <param name="tile">The planet tile to evaluate visibility for.</param>
    /// <param name="result">Outputs the randomly selected <see cref="CelestialObjectInfo"/> if any are visible; otherwise, default.</param>
    /// <returns><c>true</c> if at least one matching object is visible and one was selected; otherwise, <c>false</c>.</returns>
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
    
    /// <summary>
    /// Gathers all registered celestial objects corresponding to the given <see cref="CelestialObjectType"/> from world data caches.
    /// </summary>
    /// <param name="type">The celestial object type to retrieve.</param>
    /// <param name="results">The destination list to populate with matching <see cref="ICelestialObject"/> instances.</param>
    private static void GetObjectsOfType(CelestialObjectType type, List<ICelestialObject> results)
    {
      var data = Find.World?.GetComponent<WorldComponent_CelestialObjectDataCache>();
      var constellationData = Find.World?.GetComponent<WorldComponent_ConstellationDataCache>();
      
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
    
    /// <summary>
    /// Helper method to add non-null celestial objects from a typed source list to a polymorphic destination list.
    /// </summary>
    /// <typeparam name="T">The concrete celestial object type implementing <see cref="ICelestialObject"/>.</typeparam>
    /// <param name="source">The source list of celestial objects.</param>
    /// <param name="destination">The destination list to append elements to.</param>
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
    
    /// <summary>
    /// Flattens and extracts all non-null constellation stars from a list of constellations into the destination list.
    /// </summary>
    /// <param name="constellations">The list of saved constellations containing star definitions.</param>
    /// <param name="destination">The destination list to receive the individual <see cref="ICelestialObject"/> stars.</param>
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
    
    /// <summary>
    /// Resolves the current local sky direction vector for the specified celestial object, evaluating dynamic orbital positions if applicable.
    /// </summary>
    /// <param name="celestialObject">The celestial object whose local sky position is being resolved.</param>
    /// <returns>The local sky position vector in celestial coordinates.</returns>
    public static Vector3 CurrentLocalSkyPositionFor(ICelestialObject celestialObject)
    {
      if (celestialObject is SavedLocalStar localStar)
        return LocalStarOrbitUtil.PositionFor(localStar);
      
      return celestialObject.LocalSkyPosition;
    }
  }
}