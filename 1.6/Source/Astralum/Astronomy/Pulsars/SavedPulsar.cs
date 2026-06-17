using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Pulsars
{
  public class SavedPulsar : IExposable
  {
    public int pulsarId;
    public string name;
    
    public Vector3 localSkyPos;
    public float size;
    public float rotationDegrees;
    
    public void ExposeData()
    {
      Scribe_Values.Look(ref pulsarId, "pulsarId");
      Scribe_Values.Look(ref name, "name");
      Scribe_Values.Look(ref localSkyPos, "localSkyPos");
      Scribe_Values.Look(ref size, "size");
      Scribe_Values.Look(ref rotationDegrees, "rotationDegrees");
    }
  }
}