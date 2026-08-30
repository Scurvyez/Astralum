using System.Collections.Generic;
using Astralum.Astronomy.BlackHoles;
using Astralum.Astronomy.GalacticDustLanes;
using Astralum.Astronomy.LocalStars;
using Astralum.Astronomy.Nebulae;
using Astralum.Astronomy.Pulsars;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_CelestialObjectDataCache : WorldComponent
  {
    public SavedLocalStarSystem LocalStarSystem;
    public List<SavedLocalStar> LocalStars = [];
    public List<SavedBlackHole> BlackHoles = [];
    public List<SavedNebula> Nebulae = [];
    public List<SavedPulsar> Pulsars = [];
    public List<SavedGalacticDustLane> DustLanes = [];
    
    public bool HasGeneratedLocalStars => !LocalStars.NullOrEmpty();
    public bool HasGeneratedBlackHoles => !BlackHoles.NullOrEmpty();
    public bool HasGeneratedNebulae => !Nebulae.NullOrEmpty();
    public bool HasGeneratedPulsars => !Pulsars.NullOrEmpty();
    public bool HasGeneratedDustLanes => !DustLanes.NullOrEmpty();

    public void ClearLocalStars()
    {
      LocalStars.Clear();
      LocalStarSystem = null;
    }
    public void ClearBlackHoles() => BlackHoles.Clear();
    public void ClearNebulae() => Nebulae.Clear();
    public void ClearPulsars() => Pulsars.Clear();
    public void ClearDustLanes() => DustLanes.Clear();
    
    public WorldComponent_CelestialObjectDataCache(RimWorld.Planet.World world) : base(world)
    {
      
    }
    
    public override void ExposeData()
    {
      base.ExposeData();
      
      Scribe_Deep.Look(ref LocalStarSystem, "LocalStarSystem");
      Scribe_Collections.Look(ref LocalStars, "LocalStars", LookMode.Deep);
      Scribe_Collections.Look(ref BlackHoles, "BlackHoles", LookMode.Deep);
      Scribe_Collections.Look(ref Nebulae, "Nebulae", LookMode.Deep);
      Scribe_Collections.Look(ref Pulsars, "Pulsars", LookMode.Deep);
      Scribe_Collections.Look(ref DustLanes, "DustLanes", LookMode.Deep);
      
      if (Scribe.mode != LoadSaveMode.PostLoadInit) 
        return;
      
      LocalStars ??= [];
      BlackHoles ??= [];
      Nebulae ??= [];
      Pulsars ??= [];
      DustLanes ??= [];
    }
  }
}