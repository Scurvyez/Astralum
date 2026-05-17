using Verse;

namespace Astralum.Astronomy.BlackHoles
{
  public class ModExt_BlackHoles : DefModExtension
  {
    public float blackHoleChance = 0.05f;
    public FloatRange blackHoleSize = new(0.5f, 2f);
    public int blackHoleCount = 1;
  }
}