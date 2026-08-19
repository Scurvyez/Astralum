using System.Collections.Generic;
using Astralum.Astronomy.Pulsars;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_PulsarData : WorldComponent
  {
    public List<SavedPulsar> Pulsars = [];
    
    public WorldComponent_PulsarData(RimWorld.Planet.World world) : base(world)
    {
    }
    
    public bool HasGeneratedPulsars => !Pulsars.NullOrEmpty();
    
    public void Clear()
    {
      Pulsars.Clear();
    }
    
    public override void ExposeData()
    {
      base.ExposeData();
      
      Scribe_Collections.Look(ref Pulsars, "Pulsars", LookMode.Deep);
      
      if (Scribe.mode == LoadSaveMode.PostLoadInit)
        Pulsars ??= [];
    }
  }
}