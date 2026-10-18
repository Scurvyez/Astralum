using Astralum.Debugging;
using Verse;

namespace Astralum.API
{
  /// <summary>
  /// Serves as the base class for handling pawn observation and discovery events dispatched by <see cref="ObservationUtil"/>.
  /// Subclasses can override these lifecycle hooks to execute custom gameplay, mood, thought, or other logic.
  /// </summary>
  public abstract class ObservationWorker
  {
    /// <summary>
    /// Invoked when a pawn actively observes a celestial object in the sky.
    /// </summary>
    /// <param name="pawn">The pawn observing the celestial object.</param>
    /// <param name="objectInfo">Descriptor containing metadata and coordinates of the observed celestial object.</param>
    public virtual void Notify_PawnObservedCelestialObject(Pawn pawn, CelestialObjectInfo objectInfo)
    {
      // AstraLog.Message($"{pawn.NameShortColored} observed celestial object: {objectInfo}");
    }
    
    /// <summary>
    /// Invoked when a pawn discovers a previously unknown celestial object in the sky.
    /// </summary>
    /// <param name="pawn">The pawn responsible for discovering the celestial object.</param>
    /// <param name="objectInfo">Descriptor containing metadata and coordinates of the discovered celestial object.</param>
    public virtual void Notify_PawnDiscoveredCelestialObject(Pawn pawn, CelestialObjectInfo objectInfo)
    {
      
    }
    
    /// <summary>
    /// Invoked when a pawn observes distant background stars exposed during a local stellar eclipse.
    /// </summary>
    /// <param name="pawn">The pawn observing distant stars during the eclipse.</param>
    public virtual void Notify_PawnObservedDistantStarsDuringEclipse(Pawn pawn)
    {
      
    }
  }
}