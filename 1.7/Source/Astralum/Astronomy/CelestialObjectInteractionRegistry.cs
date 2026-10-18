using System.Collections.Generic;
using Astralum.API;
using Astralum.Astronomy.LocalStars;
using UnityEngine;

namespace Astralum.Astronomy
{
  /// <summary>
  /// Manages the registry and spatial index of interactive celestial objects currently available for hover inspection and UI interaction.
  /// </summary>
  public static class CelestialObjectInteractionRegistry
  {
    private static readonly List<HoverCelestialObject> Objects = [];
    private static readonly Dictionary<string, int> IndexByObject = [];
    
    /// <summary>
    /// Gets the read-only list of registered celestial objects eligible for hover detection.
    /// </summary>
    public static IReadOnlyList<HoverCelestialObject> HoverObjects => Objects;
    
    /// <summary>
    /// Gets a value indicating whether the interaction registry has been flagged as dirty and requires updating or re-rendering.
    /// </summary>
    public static bool Dirty { get; private set; }

    /// <summary>
    /// Flags the interaction registry as dirty, signaling that registered hover objects or their state need recalculation.
    /// </summary>
    public static void MarkDirty()
    {
      Dirty =  true;
    }
    
    /// <summary>
    /// Clears the dirty flag on the interaction registry.
    /// </summary>
    public static void ClearDirty()
    {
      Dirty =  false;
    }
    
    /// <summary>
    /// Removes all registered hover celestial objects and clears the internal index.
    /// </summary>
    public static void Clear()
    {
      Objects.Clear();
      IndexByObject.Clear();
    }
    
    /// <summary>
    /// Removes all registered hover celestial objects matching the specified celestial object type and rebuilds the lookup index.
    /// </summary>
    /// <param name="type">The type of celestial objects to remove from the registry.</param>
    public static void Clear(CelestialObjectType type)
    {
      for (int i = Objects.Count - 1; i >= 0; i--)
      {
        if (Objects[i].Type != type)
          continue;
        
        Objects.RemoveAt(i);
      }
      
      RebuildIndex();
    }
    
    /// <summary>
    /// Reconstructs the internal lookup dictionary mapping composite object keys to their respective list indices.
    /// </summary>
    private static void RebuildIndex()
    {
      IndexByObject.Clear();
      
      for (int i = 0; i < Objects.Count; i++)
      {
        HoverCelestialObject obj = Objects[i];
        
        IndexByObject[KeyFor(obj.Type, obj.ID)] = i;
      }
    }
    
    /// <summary>
    /// Registers a new interactive celestial object or updates an existing entry in the registry if already present.
    /// </summary>
    /// <param name="type">The celestial category or object type.</param>
    /// <param name="id">The unique identifier of the celestial object.</param>
    /// <param name="name">The display name of the celestial object.</param>
    /// <param name="localSkyPos">The projected position vector in local celestial sky space.</param>
    /// <param name="size">The visual display size or angular scale of the object.</param>
    /// <param name="hemisphere">The celestial hemisphere where the object resides.</param>
    /// <param name="rightAscension">The formatted right ascension coordinate string.</param>
    /// <param name="declination">The formatted declination coordinate string.</param>
    /// <param name="spectralClass">The optional spectral classification of the star, if applicable.</param>
    /// <param name="constellationName">The optional name of the parent constellation, if associated with one.</param>
    public static void Register(CelestialObjectType type, string id, string name, Vector3 localSkyPos,
      float size, string hemisphere, string rightAscension, string declination, SpectralClass? spectralClass = null,
      string constellationName = null)
    {
      HoverCelestialObject hoverObject = new(type, id, name, localSkyPos, size, hemisphere, rightAscension,
        declination, spectralClass, constellationName);
      
      string key = KeyFor(type, id);
      
      if (IndexByObject.TryGetValue(key, out int index))
      {
        Objects[index] = hoverObject;
        return;
      }
      
      IndexByObject[key] = Objects.Count;
      Objects.Add(hoverObject);
    }
    
    /// <summary>
    /// Generates a composite lookup key string combining the celestial object type and its identifier.
    /// </summary>
    /// <param name="type">The type of celestial object.</param>
    /// <param name="id">The unique identifier of the object.</param>
    /// <returns>A composite key string in the format <c>"{type}:{id}"</c>.</returns>
    private static string KeyFor(CelestialObjectType type, string id)
    {
      return $"{type}:{id}";
    }
    
    /// <summary>
    /// Represents an immutable descriptor of a celestial object registered for interactive hover inspection and tooltip display.
    /// </summary>
    public readonly struct HoverCelestialObject
    {
      /// <summary>
      /// The type classification of the celestial object.
      /// </summary>
      public readonly CelestialObjectType Type;

      /// <summary>
      /// The unique identifier of the celestial object.
      /// </summary>
      public readonly string ID;

      /// <summary>
      /// The display name of the celestial object.
      /// </summary>
      public readonly string Name;

      /// <summary>
      /// The projected position vector in local celestial sky coordinates.
      /// </summary>
      public readonly Vector3 LocalSkyPos;

      /// <summary>
      /// The visual display size or angular scale of the celestial object.
      /// </summary>
      public readonly float Size;

      /// <summary>
      /// The celestial hemisphere where the object resides.
      /// </summary>
      public readonly string Hemisphere;

      /// <summary>
      /// The formatted right ascension coordinate string.
      /// </summary>
      public readonly string RightAscension;

      /// <summary>
      /// The formatted declination coordinate string.
      /// </summary>
      public readonly string Declination;

      /// <summary>
      /// The optional spectral classification for stellar objects.
      /// </summary>
      public readonly SpectralClass? SpectralClass;

      /// <summary>
      /// The optional name of the parent constellation to which this object belongs.
      /// </summary>
      public readonly string ConstellationName;
      
      /// <summary>
      /// Initializes a new instance of the <see cref="HoverCelestialObject"/> struct with positional, coordinate, and metadata properties.
      /// </summary>
      /// <param name="type">The type classification of the celestial object.</param>
      /// <param name="id">The unique identifier of the celestial object.</param>
      /// <param name="name">The display name of the celestial object.</param>
      /// <param name="localSkyPos">The projected position vector in local celestial sky space.</param>
      /// <param name="size">The visual display size or angular scale.</param>
      /// <param name="hemisphere">The celestial hemisphere where the object resides.</param>
      /// <param name="rightAscension">The formatted right ascension coordinate string.</param>
      /// <param name="declination">The formatted declination coordinate string.</param>
      /// <param name="spectralClass">The optional spectral classification for stellar objects.</param>
      /// <param name="constellationName">The optional name of the parent constellation.</param>
      public HoverCelestialObject(CelestialObjectType type, string id, string name, Vector3 localSkyPos, float size,
        string hemisphere, string rightAscension, string declination, SpectralClass? spectralClass = null,
        string constellationName = null)
      {
        Type = type;
        ID = id;
        Name = name;
        LocalSkyPos = localSkyPos;
        Size = size;
        Hemisphere = hemisphere;
        RightAscension = rightAscension;
        Declination = declination;
        SpectralClass = spectralClass;
        ConstellationName = constellationName;
      }
    }
  }
}