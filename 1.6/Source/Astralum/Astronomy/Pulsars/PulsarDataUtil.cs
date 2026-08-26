using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Pulsars
{
  public static class PulsarDataUtil
  {
    public static WorldComponent_CelestialObjectDataCache Data => Find.World?.GetComponent<WorldComponent_CelestialObjectDataCache>();
    
    public static SavedPulsar Create(string id, Vector3 dir, float size, float rotation)
    {
      return CelestialObjectDataUtil.CreateNameable<SavedPulsar>(id, dir, size,
        PulsarNamingUtil.GenerateName(dir), 0f);
    }
    
    public static SavedPulsar GetById(string id)
    {
      return Data?.Pulsars.GetById(id);
    }
  }
}