using Astralum.Astronomy.LocalStars;
using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.BlackHoles
{
  public static class BlackHoleDataUtil
  {
    public static WorldComponent_CelestialObjectDataCache Data => Find.World?.GetComponent<WorldComponent_CelestialObjectDataCache>();
    
    public static SavedBlackHole Create(string id, Vector3 dir, float size, float rotation)
    {
      return CelestialObjectDataUtil.CreateNameable<SavedBlackHole>(id, dir, size, 
        StellarNamingUtil.GenerateGenericSystemName(), rotation);
    }
    
    public static SavedBlackHole GetById(string id)
    {
      return Data?.BlackHoles.GetById(id);
    }
  }
}