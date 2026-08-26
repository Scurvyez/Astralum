using Astralum.Astronomy.LocalSystem.Stars;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public class SavedConstellationStar : SavedPlayerNameableCelestialObject
  {
    public Vector2 uv;
    public SpectralClass spectralClass;

    public override void ExposeData()
    {
      base.ExposeData();
      
      Scribe_Values.Look(ref uv, "uv");
      Scribe_Values.Look(ref spectralClass, "spectralClass");
    }
  }
}