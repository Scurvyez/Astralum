using UnityEngine;
using Verse;

namespace Astralum.Astronomy
{
  /// <summary>
  /// Represents the base persistent data model for a celestial object stored in world save state.
  /// </summary>
  public abstract class SavedCelestialObject : IExposable, ICelestialObject
  {
    /// <summary>
    /// The unique identifier of the celestial object.
    /// </summary>
    public string id;

    /// <summary>
    /// The visual rendering scale or size of the celestial object on the sky dome.
    /// </summary>
    public float renderSize;

    /// <summary>
    /// The orientation or rotation angle of the celestial object in degrees.
    /// </summary>
    public float rotation;

    /// <summary>
    /// The position vector of the celestial object in local celestial sky coordinates.
    /// </summary>
    public Vector3 localSkyPosition;
    
    /// <inheritdoc/>
    public string Id => id;

    /// <inheritdoc/>
    public float RenderSize => renderSize;

    /// <inheritdoc/>
    public float Rotation => rotation;

    /// <inheritdoc/>
    public Vector3 LocalSkyPosition => localSkyPosition;
    
    /// <summary>
    /// Saves or loads persistent state data using RimWorld's scribe serialization subsystem.
    /// </summary>
    public virtual void ExposeData()
    {
      Scribe_Values.Look(ref id, "id");
      Scribe_Values.Look(ref renderSize, "renderSize");
      Scribe_Values.Look(ref rotation, "rotation");
      Scribe_Values.Look(ref localSkyPosition, "localSkyPosition");
    }
  }
}