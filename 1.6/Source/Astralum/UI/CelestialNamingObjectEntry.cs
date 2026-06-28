using Astralum.Astronomy;
using UnityEngine;

namespace Astralum.UI
{
  public readonly struct CelestialNamingObjectEntry
  {
    public readonly string CategoryLabel;
    public readonly string Id;
    public readonly IPlayerNamedCelestialObject Object;
    public readonly Vector3 LocalSkyPos;
    
    public string DisplayName => Object?.DisplayName;
    public string GeneratedName => Object?.GeneratedName;
    public bool HasPlayerName => Object?.HasPlayerSetName == true;
    
    public CelestialNamingObjectEntry(string categoryLabel, string id, IPlayerNamedCelestialObject obj, 
      Vector3 localSkyPos)
    {
      CategoryLabel = categoryLabel;
      Id = id;
      Object = obj;
      LocalSkyPos = localSkyPos;
    }
  }
}