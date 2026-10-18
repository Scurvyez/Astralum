using UnityEngine;

namespace Astralum.Astronomy
{
  /// <summary>
  /// Defines common properties and contract requirements for celestial objects rendered on the sky dome.
  /// </summary>
  public interface ICelestialObject
  {
    /// <summary>
    /// Gets the unique identifier of the celestial object.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the visual rendering scale or size of the celestial object on the sky dome.
    /// </summary>
    public float RenderSize { get; }

    /// <summary>
    /// Gets the orientation or rotation angle of the celestial object in degrees.
    /// </summary>
    public float Rotation { get; }

    /// <summary>
    /// Gets the position vector of the celestial object in local celestial sky coordinates.
    /// </summary>
    public Vector3 LocalSkyPosition { get; }
  }
}