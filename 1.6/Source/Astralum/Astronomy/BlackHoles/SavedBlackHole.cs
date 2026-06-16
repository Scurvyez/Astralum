using UnityEngine;
using Verse;

namespace Astralum.Astronomy.BlackHoles
{
  public class SavedBlackHole : IExposable
  {
    public int blackHoleId;
    public string name;
    public Vector3 localSkyPos;
    public Vector3 dir;
    public float size;
    
    public void ExposeData()
    {
      Scribe_Values.Look(ref blackHoleId, "blackHoleId");
      Scribe_Values.Look(ref name, "name");
      Scribe_Values.Look(ref localSkyPos, "localSkyPos");
      Scribe_Values.Look(ref dir, "dir");
      Scribe_Values.Look(ref size, "size");
    }
  }
}