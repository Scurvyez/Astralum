using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public class SavedConstellation : SavedPlayerNameableCelestialObject
  {
    public string categoryId;
    public Vector3 centerDir;
    public string maskName;
    public List<SavedConstellationStar> stars = [];
    
    public override void ExposeData()
    {
      base.ExposeData();
      
      Scribe_Values.Look(ref categoryId, "categoryId");
      Scribe_Values.Look(ref maskName, "maskName");
      Scribe_Values.Look(ref centerDir, "centerDir");
      Scribe_Collections.Look(ref stars, "stars", LookMode.Deep);

      if (Scribe.mode == LoadSaveMode.PostLoadInit)
        stars ??= [];
    }
  }
}