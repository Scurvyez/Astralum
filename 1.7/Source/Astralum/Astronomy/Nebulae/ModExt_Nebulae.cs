using Verse;

namespace Astralum.Astronomy.Nebulae
{
  public class ModExt_Nebulae : DefModExtension
  {
    public FloatRange galacticPlaneBounds = new(-0.18f, 0.18f);
    public IntRange nebulaCount = new(10, 13);
    public FloatRange nebulaSizeRange = new(6f, 18f);
  }
}