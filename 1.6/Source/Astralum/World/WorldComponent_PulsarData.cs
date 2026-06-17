using System.Collections.Generic;
using Astralum.Astronomy.Pulsars;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_PulsarData : WorldComponent
  {
    public List<SavedPulsar> pulsars = [];
    
    public WorldComponent_PulsarData(RimWorld.Planet.World world) : base(world)
    {
    }
    
    public bool HasGeneratedPulsars => !pulsars.NullOrEmpty();
    
    public void Clear()
    {
      pulsars.Clear();
    }
    
    public override void ExposeData()
    {
      base.ExposeData();
      
      Scribe_Collections.Look(ref pulsars, "pulsars", LookMode.Deep);
      
      if (Scribe.mode == LoadSaveMode.PostLoadInit)
        pulsars ??= [];
    }
  }
}