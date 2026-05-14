using System.Collections.Generic;
using Astralum.Astronomy.Nebulae;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
    public class WorldComponent_NebulaeData : WorldComponent
    {
        public List<SavedNebula> nebulae = [];
        
        public bool HasGeneratedNebulae => !nebulae.NullOrEmpty();
        
        public WorldComponent_NebulaeData(RimWorld.Planet.World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            
            Scribe_Collections.Look(ref nebulae, "nebulae", LookMode.Deep);
            
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                nebulae ??= [];
        }
        
        public void Clear()
        {
            nebulae.Clear();
        }
    }
}