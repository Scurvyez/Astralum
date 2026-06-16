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
        blackHoleId = id,
        name = StellarNamingUtil.GenerateGenericSystemName(),
        dir = dir.normalized,
        localSkyPos = dir.normalized * 20f,
        size = size
      };
    }
  }
}