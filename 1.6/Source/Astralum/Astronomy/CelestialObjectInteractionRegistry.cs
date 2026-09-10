using System.Collections.Generic;
using Astralum.API;
using Astralum.Astronomy.LocalStars;
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
        if (Objects[i].Type != type)
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
        
        IndexByObject[KeyFor(obj.Type, obj.ID)] = i;
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
      public readonly CelestialObjectType Type;
      public readonly string ID;
      public readonly string Name;
      public readonly Vector3 LocalSkyPos;
      public readonly float Size;
      public readonly string Hemisphere;
      public readonly string RightAscension;
      public readonly string Declination;
      public readonly SpectralClass? SpectralClass;
      public readonly string ConstellationName;
      
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