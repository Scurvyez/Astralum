using System.Collections.Generic;
using Astralum.Astronomy.BlackHoles;
using Astralum.Astronomy.GalacticDustLanes;
using Astralum.Astronomy.Nebulae;
using Astralum.Astronomy.Pulsars;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_CelestialObjectDataCache : WorldComponent
  {
    public List<SavedBlackHole> BlackHoles = [];
    public List<SavedNebula> Nebulas = [];
    public List<SavedPulsar> Pulsars = [];
    public List<SavedGalacticDustLane> DustLanes = [];
    
    public bool HasGeneratedBlackHoles => !BlackHoles.NullOrEmpty();
    public bool HasGeneratedNebulae => !Nebulas.NullOrEmpty();
    public bool HasGeneratedPulsars => !Pulsars.NullOrEmpty();
    public bool HasGeneratedDustLanes => !DustLanes.NullOrEmpty();
    public void ClearBlackHoles() => BlackHoles.Clear();
    public void ClearNebulas() => Nebulas.Clear();
    public void ClearPulsars() => Pulsars.Clear();
    public void ClearDustLanes() => DustLanes.Clear();
    
    public WorldComponent_CelestialObjectDataCache(RimWorld.Planet.World world) : base(world)
    {
      
    }
    
    public override void ExposeData()
    {
      base.ExposeData();
      
      Scribe_Collections.Look(ref BlackHoles, "BlackHoles", LookMode.Deep);
      Scribe_Collections.Look(ref Nebulas, "Nebulas", LookMode.Deep);
      Scribe_Collections.Look(ref Pulsars, "Pulsars", LookMode.Deep);
      Scribe_Collections.Look(ref DustLanes, "DustLanes", LookMode.Deep);
      
      if (Scribe.mode != LoadSaveMode.PostLoadInit) 
        return;
      
      BlackHoles ??= [];
      Nebulas ??= [];
      Pulsars ??= [];
      DustLanes ??= [];
    }
  }
}