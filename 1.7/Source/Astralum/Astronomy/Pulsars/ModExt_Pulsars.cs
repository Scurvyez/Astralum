using Verse;

namespace Astralum.Astronomy.Pulsars
{
  public class ModExt_Pulsars : DefModExtension
  {
    public float pulsarChance = 0.05f;
    public FloatRange pulsarSize = new(0.3f, 2f);
    public IntRange pulsarCount = new(0, 1);
  }
}