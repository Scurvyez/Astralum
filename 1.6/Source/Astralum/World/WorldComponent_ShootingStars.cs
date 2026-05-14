using Astralum.Astronomy.ShootingStars;
using Astralum.Debugging;
using Astralum.DefOfs;
using RimWorld.Planet;

namespace Astralum.World
{
    public class WorldComponent_ShootingStars : WorldComponent
    {
        private ModExt_ShootingStars _ext;
        
        public WorldComponent_ShootingStars(RimWorld.Planet.World world) : base(world)
        {
            _ext = InternalDefOf.Astra_ShootingStars?.GetModExtension<ModExt_ShootingStars>();
            
            if (_ext != null) return;
            AstraLog.Warning("Astra_ShootingStars is missing ModExt_ShootingStars. Using fallback values.");
        }
        
        public override void WorldComponentTick()
        {
            ShootingStarManager.Tick(_ext);
        }
    }
}