using Verse;

namespace Astralum.Astronomy.Pulsars
{
  public class ModExt_Pulsars : DefModExtension
  {
    public float pulsarChance = 0.05f;
    public float pulsarSize = 0.8f;
    public IntRange pulsarCount = new(0, 1);
  }
}