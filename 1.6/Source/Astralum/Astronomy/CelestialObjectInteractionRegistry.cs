using System.Collections.Generic;
using Astralum.API;
using Astralum.Astronomy.LocalSystem.Stars;
using UnityEngine;

namespace Astralum.Astronomy
{
  public static class CelestialObjectInteractionRegistry
  {
    private static readonly List<HoverCelestialObject> Objects = [];
    private static readonly Dictionary<string, int> IndexByObject = [];
    
    public static IReadOnlyList<HoverCelestialObject> HoverObjects => Objects;
    
    public static bool Dirty { get; private set; }

    public static void MarkDirty()
    {
      Dirty =  true;
    }
    
    public static void ClearDirty()
    {
      Dirty =  false;
    }
    
    public static void Clear()
    {
      Objects.Clear();
      IndexByObject.Clear();
    }
    
    public static void Clear(CelestialObjectType type)
    {
      for (int i = Objects.Count - 1; i >= 0; i--)
      {
        if (Objects[i].type != type)
          continue;
        
        Objects.RemoveAt(i);
      }
      
      RebuildIndex();
    }
    
    private static void RebuildIndex()
    {
      IndexByObject.Clear();
      
      for (int i = 0; i < Objects.Count; i++)
      {
        HoverCelestialObject obj = Objects[i];
        
        IndexByObject[KeyFor(obj.type, obj.id)] = i;
      }
    }
    
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
    
    private static string KeyFor(CelestialObjectType type, string id)
    {
      return $"{type}:{id}";
    }
    
    public readonly struct HoverCelestialObject
    {
      public readonly CelestialObjectType type;
      public readonly string id;
      public readonly string name;
      public readonly Vector3 localSkyPos;
      public readonly float size;
      public readonly string hemisphere;
      public readonly string rightAscension;
      public readonly string declination;
      public readonly SpectralClass? spectralClass;
      public readonly string constellationName;
      
      public HoverCelestialObject(CelestialObjectType type, string id, string name, Vector3 localSkyPos, float size,
        string hemisphere, string rightAscension, string declination, SpectralClass? spectralClass = null,
        string constellationName = null)
      {
        this.type = type;
        this.id = id;
        this.name = name;
        this.localSkyPos = localSkyPos;
        this.size = size;
        this.hemisphere = hemisphere;
        this.rightAscension = rightAscension;
        this.declination = declination;
        this.spectralClass = spectralClass;
        this.constellationName = constellationName;
      }
    }
  }
}