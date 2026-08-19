using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.World;
using Verse;

namespace Astralum.Astronomy.BlackHoles
{
  public static class BlackHoleDataUtil
  {
    public static WorldComponent_BlackHoleData Data => Find.World?.GetComponent<WorldComponent_BlackHoleData>();
    
    public static SavedBlackHole Create(int id, UnityEngine.Vector3 dir, float size)
    {
      return new SavedBlackHole
      {
        id = id,
        generatedName = StellarNamingUtil.GenerateGenericSystemName(),
        dir = dir.normalized,
        localSkyPos = dir.normalized * 20f,
        size = size
      };
    }
    
    public static SavedBlackHole GetById(int id)
    {
      WorldComponent_BlackHoleData data = Data;
      
      if (data?.BlackHoles.NullOrEmpty() != false)
        return null;
      
      for (int i = 0; i < data.BlackHoles.Count; i++)
      {
        if (data.BlackHoles[i].id == id)
          return data.BlackHoles[i];
      }
      
      return null;
    }
  }
}