using UnityEngine;
using Verse;

namespace Astralum.Astronomy
{
  public abstract class SavedCelestialObject : IExposable, ICelestialObject
  {
    public string id;
    public float renderSize;
    public float rotation;
    public Vector3 localSkyPosition;
    
    public string Id => id;
    public float RenderSize => renderSize;
    public float Rotation => rotation;
    public Vector3 LocalSkyPosition => localSkyPosition;
    
    public virtual void ExposeData()
    {
      Scribe_Values.Look(ref id, "id");
      Scribe_Values.Look(ref renderSize, "renderSize");
      Scribe_Values.Look(ref rotation, "rotation");
      Scribe_Values.Look(ref localSkyPosition, "localSkyPosition");
    }
  }
}