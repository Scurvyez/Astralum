using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public class SavedConstellation : IExposable
  {
    public string categoryId;
    public Vector3 centerDir;
    public string maskName;
    public string name;
    public float rotationDegrees;
    public float size;
    public List<SavedConstellationStar> stars = [];

    public void ExposeData()
    {
      Scribe_Values.Look(ref name, "name");
      Scribe_Values.Look(ref categoryId, "categoryId");
      Scribe_Values.Look(ref maskName, "maskName");
      Scribe_Values.Look(ref centerDir, "centerDir");
      Scribe_Values.Look(ref size, "size");
      Scribe_Values.Look(ref rotationDegrees, "rotationDegrees");
      Scribe_Collections.Look(ref stars, "stars", LookMode.Deep);

      if (Scribe.mode == LoadSaveMode.PostLoadInit)
        stars ??= [];
    }
  }
}