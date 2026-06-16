using System.Collections.Generic;
using Astralum.Astronomy.BlackHoles;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_BlackHoleData : WorldComponent
  {
    public List<SavedBlackHole> blackHoles = [];
    
    public WorldComponent_BlackHoleData(RimWorld.Planet.World world) : base(world)
    {
    }
    
    public bool HasGeneratedBlackHoles => !blackHoles.NullOrEmpty();
    
    public void Clear()
    {
      blackHoles.Clear();
    }
    
    public override void ExposeData()
    {
      base.ExposeData();
      
      Scribe_Collections.Look(ref blackHoles, "blackHoles", LookMode.Deep);
      
      if (Scribe.mode == LoadSaveMode.PostLoadInit)
        blackHoles ??= [];
    }
  }
}