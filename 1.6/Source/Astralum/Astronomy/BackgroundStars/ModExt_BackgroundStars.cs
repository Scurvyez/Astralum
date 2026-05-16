using Verse;

namespace Astralum.Astronomy.BackgroundStars
{
  public class ModExt_BackgroundStars : DefModExtension
  {
    public FloatRange galacticPlaneBounds = new(-0.16f, 0.16f);
    public int starCount = 50000;
    public FloatRange starSizeRange = new(0.085f, 0.85f);
  }
}