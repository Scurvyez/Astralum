namespace Astralum.API
{
  /// <summary>
  /// Specifies the classification type of celestial object in the world sky or astronomy system.
  /// </summary>
  public enum CelestialObjectType
  {
    /// <summary>
    /// An unknown or unclassified celestial object.
    /// </summary>
    Unknown,

    /// <summary>
    /// A local system star that orbits the planet and provides direct surface illumination.
    /// </summary>
    LocalStar,

    /// <summary>
    /// A distant background star rendered in the deep celestial field.
    /// </summary>
    BackgroundStar,

    /// <summary>
    /// A named grouping or pattern of stars forming a constellation.
    /// </summary>
    Constellation,

    /// <summary>
    /// An individual constituent star belonging to a specific constellation.
    /// </summary>
    ConstellationStar,

    /// <summary>
    /// An interstellar cloud of cosmic dust, gas, and nebular emissions.
    /// </summary>
    Nebulae,

    /// <summary>
    /// A highly magnetized rotating neutron star emitting periodic radiation beams.
    /// </summary>
    Pulsar,

    /// <summary>
    /// A supermassive or stellar-mass gravitational singularity.
    /// </summary>
    BlackHole
  }
}