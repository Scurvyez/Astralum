using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Pulsars
{
  public static class PulsarDataUtil
  {
    public static WorldComponent_PulsarData Data => Find.World?.GetComponent<WorldComponent_PulsarData>();
    
    public static SavedPulsar Create(int id, Vector3 localSkyPos, float size, float rotationDegrees)
    {
      return new SavedPulsar
      {
        pulsarId = id,
        name = PulsarNamingUtil.GenerateName(localSkyPos),
        localSkyPos = localSkyPos,
        size = size,
        rotationDegrees = rotationDegrees
      };
    }
  }
}