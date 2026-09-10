using UnityEngine;

namespace Astralum.API
{
  public readonly struct CelestialObjectInfo
  {
    public readonly CelestialObjectType Type;
    public readonly string ID;
    public readonly string Name;
    public readonly Vector3 LocalSkyPos;
    
    public CelestialObjectInfo(CelestialObjectType type, string id, string name, Vector3 localSkyPos)
    {
      Type = type;
      ID = id;
      Name = name;
      LocalSkyPos = localSkyPos;
    }
  }
}