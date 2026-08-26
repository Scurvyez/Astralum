using System.Collections.Generic;
using Astralum.Astronomy.Constellations;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_ConstellationDataCache : WorldComponent
  {
    public List<SavedConstellation> Constellations = [];

    public bool HasGeneratedConstellations => !Constellations.NullOrEmpty();
    public void Clear() => Constellations.Clear();
    
    public WorldComponent_ConstellationDataCache(RimWorld.Planet.World world) : base(world)
    {
      
    }
    
    public override void ExposeData()
    {
      base.ExposeData();

      Scribe_Collections.Look(ref Constellations, "Constellations", LookMode.Deep);

      if (Scribe.mode == LoadSaveMode.PostLoadInit)
        Constellations ??= [];
    }
  }
}