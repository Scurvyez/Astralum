using UnityEngine;

namespace Astralum.API
{
  public readonly struct CelestialObjectInfo
  {
    public readonly CelestialObjectType type;
    public readonly string id;
    public readonly string name;
    public readonly Vector3 localSkyPos;
    
    public CelestialObjectInfo(CelestialObjectType type, string id, string name, Vector3 localSkyPos)
    {
      this.type = type;
      this.id = id;
      this.name = name;
      this.localSkyPos = localSkyPos;
    }
  }
}