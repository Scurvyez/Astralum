using Astralum.Astronomy;
using UnityEngine;
using Verse;

namespace Astralum.UI
{
  public readonly struct CelestialCatalogueObjectEntry
  {
    public readonly string CategoryLabel;
    public readonly string Id;
    public readonly IPlayerNameableCelestialObject Object;
    public readonly Vector3 LocalSkyPos;
    public readonly string ParentId;
    
    public string DisplayName => Object?.DisplayName;
    public string GeneratedName => Object?.GeneratedName;
    public bool HasPlayerName => Object?.HasPlayerSetName == true;
    public bool HasParent => !ParentId.NullOrEmpty();
    
    public CelestialCatalogueObjectEntry(string categoryLabel, string id, IPlayerNameableCelestialObject obj, 
      Vector3 localSkyPos, string parentId = null)
    {
      CategoryLabel = categoryLabel;
      Id = id;
      Object = obj;
      LocalSkyPos = localSkyPos;
      ParentId = parentId;
    }
  }
}