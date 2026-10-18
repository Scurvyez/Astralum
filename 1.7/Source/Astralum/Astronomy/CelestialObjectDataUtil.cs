using System;
using System.Collections.Generic;
using UnityEngine;

namespace Astralum.Astronomy
{
  /// <summary>
  /// Provides utility and factory methods for instantiating, configuring, and querying celestial object data models.
  /// </summary>
  public static class CelestialObjectDataUtil
  {
    /// <summary>
    /// The radial distance at which celestial objects are positioned on the local sky dome sphere.
    /// </summary>
    public const float SkyDistance = 20f;

    /// <summary>
    /// Creates and initializes a new celestial object data instance positioned along a directional vector in local sky space.
    /// </summary>
    /// <typeparam name="T">The concrete type of <see cref="SavedCelestialObject"/> to create.</typeparam>
    /// <param name="id">The unique identifier for the celestial object.</param>
    /// <param name="dir">The directional vector pointing toward the object's position in the sky.</param>
    /// <param name="size">The visual render size of the object.</param>
    /// <param name="rotation">The visual orientation rotation angle in degrees.</param>
    /// <param name="configure">An optional callback action to perform additional property configuration.</param>
    /// <returns>A configured instance of <typeparamref name="T"/>.</returns>
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
    
    /// <summary>
    /// Creates and initializes a new player-nameable celestial object data instance with a generated default name.
    /// </summary>
    /// <typeparam name="T">The concrete type of <see cref="SavedPlayerNameableCelestialObject"/> to create.</typeparam>
    /// <param name="id">The unique identifier for the celestial object.</param>
    /// <param name="dir">The directional vector pointing toward the object's position in the sky.</param>
    /// <param name="size">The visual render size of the object.</param>
    /// <param name="generatedName">The generated initial name assigned to the celestial object.</param>
    /// <param name="rotation">The visual orientation rotation angle in degrees.</param>
    /// <param name="configure">An optional callback action to perform additional property configuration.</param>
    /// <returns>A configured instance of <typeparamref name="T"/>.</returns>
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
    
    /// <summary>
    /// Searches a list of celestial objects and retrieves the first element matching the specified unique identifier.
    /// </summary>
    /// <typeparam name="T">The type of celestial object implementing <see cref="ICelestialObject"/>.</typeparam>
    /// <param name="objects">The list of celestial objects to search.</param>
    /// <param name="id">The unique identifier of the celestial object to find.</param>
    /// <returns>The matching celestial object if found; otherwise, <c>null</c>.</returns>
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