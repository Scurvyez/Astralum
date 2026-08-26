using Verse;

namespace Astralum.Astronomy.Constellations
{
  public class ModExt_Constellations : DefModExtension
  {
    public int constellationCount = 13;
    public float constellationSizeMax = 3.5f;
    public float constellationSizeMin = 3.0f;
    public int maxPlacementAttempts = 80;
    public float maxViewRotationAngle = 200f;
    public float minViewRotationAngle = 160f;
  }
}