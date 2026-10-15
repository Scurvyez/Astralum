using System;
using System.Collections.Generic;
using UnityEngine;

namespace Astralum.Astronomy
{
  public static class CelestialObjectDataUtil
  {
    private const float SkyDistance = 20f;

    public static T Create<T>(string id, Vector3 dir, float size, float rotation = 0f,
      Action<T> configure = null) where T : SavedCelestialObject, new()
    {
      Vector3 normalizedDir = dir.normalized;
      
      T celestialObject = new T
      {
        id = id,
        renderSize = size,
        rotation = rotation,
        localSkyPosition = normalizedDir * SkyDistance
      };
      
      configure?.Invoke(celestialObject);
      return celestialObject;
    }
    
    public static T CreateNameable<T>(string id, Vector3 dir, float size, string generatedName, float rotation = 0f,
      Action<T> configure = null) where T : SavedPlayerNameableCelestialObject, new()
    {
      return Create<T>(id, dir, size, rotation,
        celestialObject =>
        {
          celestialObject.generatedName = generatedName;
          configure?.Invoke(celestialObject);
        }
      );
    }
    
    public static T GetById<T>(this IList<T> objects, string id) where T : class, ICelestialObject
    {
      if (objects == null || objects.Count == 0)
        return null;
      
      for (int i = 0; i < objects.Count; i++)
      {
        T celestialObject = objects[i];
        
        if (celestialObject != null
            && celestialObject.Id == id)
        {
          return celestialObject;
        }
      }
      
      return null;
    }
  }
}