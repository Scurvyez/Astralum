using Verse;

namespace Astralum.API
{
  public abstract class ObservationWorker
  {
    /// <summary>
    /// Hook for modders. Called when a pawn is observing a celestial object.
    /// For inspiration chances, research, discovery XP, records, letters, etc.
    /// </summary>
    /// <param name="pawn">Pawn currently observing the object in space.</param>
    /// <param name="objectInfo">Relevant data for the currently observed object.</param>
    public virtual void Notify_PawnObservedCelestialObject(Pawn pawn, CelestialObjectInfo objectInfo)
    {
      
    }
    
    /// <summary>
    /// Hook for modders. Called when a celestial object is discovered for the first time.
    /// For naming, history entry, world component discovery state, etc.
    /// </summary>
    /// <param name="pawn">Pawn who made the initial discovery of the object in space.</param>
    /// <param name="objectInfo">Relevant data for the newly discovered object.</param>
    public virtual void Notify_PawnDiscoveredCelestialObject(Pawn pawn, CelestialObjectInfo objectInfo)
    {
      
    }
  }
}