using UnityEngine;

namespace Astralum.API
{
  /// <summary>
  /// Represents a lightweight, immutable descriptor conveying identification,
  /// classification, and local sky positional data for a celestial object.
  /// </summary>
  public readonly struct CelestialObjectInfo
  {
    /// <summary>
    /// The classification type of the celestial object.
    /// </summary>
    public readonly CelestialObjectType Type;

    /// <summary>
    /// The unique identifier of the celestial object.
    /// </summary>
    public readonly string ID;

    /// <summary>
    /// The human-readable display name of the celestial object.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The position vector of the celestial object in local sky space coordinates.
    /// </summary>
    public readonly Vector3 LocalSkyPos;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CelestialObjectInfo"/> struct.
    /// </summary>
    /// <param name="type">The classification category of the celestial object.</param>
    /// <param name="id">The unique identifier for the celestial object.</param>
    /// <param name="name">The human-readable display name of the celestial object.</param>
    /// <param name="localSkyPos">The local celestial sky coordinates of the object.</param>
    public CelestialObjectInfo(CelestialObjectType type, string id, string name, Vector3 localSkyPos)
    {
      Type = type;
      ID = id;
      Name = name;
      LocalSkyPos = localSkyPos;
    }
  }
}