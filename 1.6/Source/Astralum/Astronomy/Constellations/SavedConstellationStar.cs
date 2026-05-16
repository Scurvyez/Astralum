using Astralum.Astronomy.LocalSystem.Stars;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public class SavedConstellationStar : IExposable
  {
    public Vector3 localSkyPos;
    public string name;
    public float rotationDegrees;
    public SpectralClass spectralClass;
    public Vector2 uv;
    public float visualSize;

    public void ExposeData()
    {
      Scribe_Values.Look(ref name, "name");
      Scribe_Values.Look(ref uv, "uv");
      Scribe_Values.Look(ref localSkyPos, "localSkyPos");
      Scribe_Values.Look(ref spectralClass, "spectralClass");
      Scribe_Values.Look(ref visualSize, "visualSize");
      Scribe_Values.Look(ref rotationDegrees, "rotationDegrees");
    }
  }
}