using System.Collections.Generic;
using Astralum.Astronomy.BlackHoles;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_BlackHoleData : WorldComponent
  {
    public List<SavedBlackHole> BlackHoles = [];
    
    public WorldComponent_BlackHoleData(RimWorld.Planet.World world) : base(world)
    {
    }
    
    public bool HasGeneratedBlackHoles => !BlackHoles.NullOrEmpty();
    
    public void Clear()
    {
      BlackHoles.Clear();
    }
    
    public override void ExposeData()
    {
      base.ExposeData();
      
      Scribe_Collections.Look(ref BlackHoles, "BlackHoles", LookMode.Deep);
      
      if (Scribe.mode == LoadSaveMode.PostLoadInit)
        BlackHoles ??= [];
    }
  }
}