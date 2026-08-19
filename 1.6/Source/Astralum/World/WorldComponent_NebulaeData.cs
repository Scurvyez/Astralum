using System.Collections.Generic;
using Astralum.Astronomy.Nebulae;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_NebulaeData : WorldComponent
  {
    public List<SavedNebula> Nebulae = [];

    public WorldComponent_NebulaeData(RimWorld.Planet.World world) : base(world)
    {
    }

    public bool HasGeneratedNebulae => !Nebulae.NullOrEmpty();

    public override void ExposeData()
    {
      base.ExposeData();

      Scribe_Collections.Look(ref Nebulae, "Nebulae", LookMode.Deep);

      if (Scribe.mode == LoadSaveMode.PostLoadInit)
        Nebulae ??= [];
    }

    public void Clear()
    {
      Nebulae.Clear();
    }
  }
}