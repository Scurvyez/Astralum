using Verse;

namespace Astralum.Astronomy
{
  /// <summary>
  /// Provides utility methods for safely updating, sanitizing, and clearing custom player-assigned names
  /// on celestial objects implementing <see cref="IPlayerNameableCelestialObject"/>.
  /// </summary>
  public static class PlayerNamedCelestialObjectUtil
  {
    /// <summary>
    /// Attempts to assign a custom player name to the celestial object, automatically trimming whitespace
    /// and resetting the name to null if the string is empty or contains only whitespace.
    /// </summary>
    /// <param name="obj">The player-nameable celestial object to update.</param>
    /// <param name="name">The proposed custom name to assign.</param>
    public static void TrySetPlayerName(IPlayerNameableCelestialObject obj, string name)
    {
      if (obj == null) 
        return;
      
      name = name?.Trim();
      obj.PlayerSetName = name.NullOrEmpty() ? null : name;
    }
    
    /// <summary>
    /// Clears any custom player-assigned name on the celestial object, reverting its effective display name
    /// to its procedurally generated default name.
    /// </summary>
    /// <param name="obj">The player-nameable celestial object whose custom name should be cleared.</param>
    public static void ClearPlayerName(IPlayerNameableCelestialObject obj)
    {
      obj?.PlayerSetName = null;
    }
  }
}