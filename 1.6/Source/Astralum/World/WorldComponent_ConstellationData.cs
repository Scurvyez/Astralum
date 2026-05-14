using System.Collections.Generic;
using Astralum.Astronomy.Constellations;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
    public class WorldComponent_ConstellationData : WorldComponent
    {
        public List<SavedConstellation> constellations = [];
        
        public bool HasGeneratedConstellations => !constellations.NullOrEmpty();
        
        public WorldComponent_ConstellationData(RimWorld.Planet.World world) : base(world)
        {
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            
            Scribe_Collections.Look(ref constellations, "constellations", LookMode.Deep);
            
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                constellations ??= [];
        }
        
        public void Clear()
        {
            constellations.Clear();
        }
        
        public HashSet<string> GetUsedNames()
        {
            HashSet<string> result = [];
            
            if (constellations.NullOrEmpty())
                return result;
            
            for (int i = 0; i < constellations.Count; i++)
            {
                SavedConstellation constellation = constellations[i];
                
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