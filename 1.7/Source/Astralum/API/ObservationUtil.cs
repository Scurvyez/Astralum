using System;
using System.Collections.Generic;
using Verse;

namespace Astralum.API
{
  /// <summary>
  /// Provides utility methods for discovering, instantiating, and dispatching celestial observation events
  /// across all registered <see cref="ObservationWorker"/> implementations.
  /// </summary>
  public static class ObservationUtil
  {
    private static List<ObservationWorker> _workers;

    /// <summary>
    /// Gets the cached list of active observation workers, dynamically discovering and instantiating them on initial access.
    /// </summary>
    private static List<ObservationWorker> Workers
    {
      get
      {
        _workers ??= BuildWorkers();
        return _workers;
      }
    }
    
    /// <summary>
    /// Broadcasts an observation event to all registered workers when a pawn observes a specific celestial object.
    /// </summary>
    /// <param name="pawn">The pawn actively observing the celestial object.</param>
    /// <param name="objectInfo">Descriptor containing metadata and coordinates of the observed celestial object.</param>
    public static void Notify_PawnObservedCelestialObject(Pawn pawn, CelestialObjectInfo objectInfo)
    {
      foreach (ObservationWorker worker in Workers)
        worker.Notify_PawnObservedCelestialObject(pawn, objectInfo);
    }
    
    /// <summary>
    /// Broadcasts a discovery event to all registered workers when a pawn discovers a previously unknown celestial object.
    /// </summary>
    /// <param name="pawn">The pawn responsible for discovering the celestial object.</param>
    /// <param name="objectInfo">Descriptor containing metadata and coordinates of the newly discovered celestial object.</param>
    public static void Notify_PawnDiscoveredCelestialObject(Pawn pawn, CelestialObjectInfo objectInfo)
    {
      foreach (ObservationWorker worker in Workers)
        worker.Notify_PawnDiscoveredCelestialObject(pawn, objectInfo);
    }
    
    /// <summary>
    /// Broadcasts an observation event to all registered workers when a pawn observes distant background stars exposed during a stellar eclipse.
    /// </summary>
    /// <param name="pawn">The pawn actively observing distant stars during the eclipse.</param>
    public static void Notify_PawnObservedDistantStarsDuringEclipse(Pawn pawn)
    {
      foreach (ObservationWorker worker in Workers)
        worker.Notify_PawnObservedDistantStarsDuringEclipse(pawn);
    }
    
    /// <summary>
    /// Discovers all non-abstract concrete implementations of <see cref="ObservationWorker"/> across loaded assemblies and instantiates them.
    /// </summary>
    /// <returns>A list of instantiated <see cref="ObservationWorker"/> objects.</returns>
    private static List<ObservationWorker> BuildWorkers()
    {
      List<ObservationWorker> workers = [];
      
      foreach (Type type in GenTypes.AllTypes)
      {
        if (type.IsAbstract)
          continue;
        
        if (!typeof(ObservationWorker).IsAssignableFrom(type))
          continue;
        
        try
        {
          workers.Add((ObservationWorker)Activator.CreateInstance(type));
        }
        catch (Exception ex)
        {
          Log.Warning($"Astralum could not create observation worker {type.FullName}: {ex}");
        }
      }
      
      return workers;
    }
  }
}