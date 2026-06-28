using System;
using System.Collections.Generic;
using Verse;

namespace Astralum.API
{
  public static class ObservationUtility
  {
    private static List<ObservationWorker> _workers;

    private static List<ObservationWorker> Workers
    {
      get
      {
        _workers ??= BuildWorkers();
        return _workers;
      }
    }
    
    public static void Notify_PawnObservedCelestialObject(Pawn pawn, CelestialObjectInfo objectInfo)
    {
      foreach (ObservationWorker worker in Workers)
        worker.Notify_PawnObservedCelestialObject(pawn, objectInfo);
    }
    
    public static void Notify_PawnDiscoveredCelestialObject(Pawn pawn, CelestialObjectInfo objectInfo)
    {
      foreach (ObservationWorker worker in Workers)
        worker.Notify_PawnDiscoveredCelestialObject(pawn, objectInfo);
    }
    
    public static void Notify_PawnObservedDistantStarsDuringEclipse(Pawn pawn)
    {
      foreach (ObservationWorker worker in Workers)
        worker.Notify_PawnObservedDistantStarsDuringEclipse(pawn);
    }
    
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