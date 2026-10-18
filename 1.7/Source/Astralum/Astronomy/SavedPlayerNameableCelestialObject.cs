using Verse;

namespace Astralum.Astronomy
{
  /// <summary>
  /// Represents a base persistent data model for celestial objects that support player-assigned custom names.
  /// </summary>
  public abstract class SavedPlayerNameableCelestialObject : SavedCelestialObject, IPlayerNameableCelestialObject
  {
    /// <summary>
    /// The procedurally generated or system-assigned default name of the celestial object.
    /// </summary>
    public string generatedName;

    /// <summary>
    /// The custom name assigned to the celestial object by the player.
    /// </summary>
    public string playerSetName;
    
    /// <inheritdoc/>
    public string GeneratedName
    {
      get => generatedName;
      set => generatedName = value;
    }
    
    /// <inheritdoc/>
    public string PlayerSetName
    {
      get => playerSetName;
      set => playerSetName = value;
    }
    
    /// <inheritdoc/>
    public string DisplayName => playerSetName.NullOrEmpty()
        ? generatedName
        : playerSetName;
    
    /// <inheritdoc/>
    public bool HasPlayerSetName => !playerSetName.NullOrEmpty();
    
    /// <summary>
    /// Saves or loads persistent state data using RimWorld's scribe serialization subsystem.
    /// </summary>
    public override void ExposeData()
    {
      base.ExposeData();
      
      Scribe_Values.Look(ref generatedName, "generatedName");
      Scribe_Values.Look(ref playerSetName, "playerSetName");
    }
  }
}