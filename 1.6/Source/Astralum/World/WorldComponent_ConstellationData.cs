using System.Collections.Generic;
using Astralum.Astronomy.Constellations;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_ConstellationData : WorldComponent
  {
    public List<SavedConstellation> Constellations = [];

    public WorldComponent_ConstellationData(RimWorld.Planet.World world) : base(world)
    {
    }

    public bool HasGeneratedConstellations => !Constellations.NullOrEmpty();

    public override void ExposeData()
    {
      base.ExposeData();

      Scribe_Collections.Look(ref Constellations, "Constellations", LookMode.Deep);

      if (Scribe.mode == LoadSaveMode.PostLoadInit)
        Constellations ??= [];
    }

    public void Clear()
    {
      Constellations.Clear();
    }

    public HashSet<string> GetUsedNames()
    {
      HashSet<string> result = [];

      if (Constellations.NullOrEmpty())
        return result;

      for (int i = 0; i < Constellations.Count; i++)
      {
        SavedConstellation constellation = Constellations[i];

        if (!constellation.name.NullOrEmpty())
          result.Add(constellation.name);

        if (constellation.stars.NullOrEmpty())
          continue;

        for (int j = 0; j < constellation.stars.Count; j++)
        {
          string starName = constellation.stars[j].name;

          if (!starName.NullOrEmpty())
            result.Add(starName);
        }
      }

      return result;
    }
  }
}