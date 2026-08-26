using Verse;

namespace Astralum.Astronomy.GalacticDustLanes
{
  public class ModExt_GalacticDustLanes : DefModExtension
  {
    public IntRange dustLaneCount = new(4, 7);
    public FloatRange dustLaneSizeRange = new(18f, 36f);
    public FloatRange galacticPlaneBounds = new(-0.10f, 0.10f);
  }
}