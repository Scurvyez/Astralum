using Astralum.World;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public static class ConstellationDataUtil
  {
    public static WorldComponent_ConstellationData Data =>
      Find.World.GetComponent<WorldComponent_ConstellationData>();
  }
}