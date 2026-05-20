using Verse;

namespace Astralum.Astronomy.BlackHoles
{
  public class ModExt_BlackHoles : DefModExtension
  {
    public FloatRange galacticPlaneBounds = new(-0.18f, 0.18f);
    public float blackHoleChance = 0.05f;
    public FloatRange blackHoleSize = new(0.5f, 2f);
    public IntRange blackHoleCount = new(0, 1);
  }
}