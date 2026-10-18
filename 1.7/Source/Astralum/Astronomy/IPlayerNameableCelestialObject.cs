namespace Astralum.Astronomy
{
  /// <summary>
  /// Defines properties and naming contracts for celestial objects that can be custom named or renamed by the player.
  /// </summary>
  public interface IPlayerNameableCelestialObject
  {
    /// <summary>
    /// Gets or sets the procedurally generated or system-assigned default name of the celestial object.
    /// </summary>
    string GeneratedName { get; set; }

    /// <summary>
    /// Gets or sets the custom name assigned to the celestial object by the player.
    /// </summary>
    string PlayerSetName { get; set; }

    /// <summary>
    /// Gets the effective display name of the celestial object, prioritizing any player-assigned name over the generated name.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets a value indicating whether the player has assigned a custom name to this celestial object.
    /// </summary>
    bool HasPlayerSetName { get; }
  }
}