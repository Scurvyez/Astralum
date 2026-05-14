using Astralum.Astronomy.ShootingStars;
using RimWorld.Planet;

namespace Astralum.World
{
    public class WorldComponent_ShootingStars : WorldComponent
    {
        public WorldComponent_ShootingStars(RimWorld.Planet.World world) : base(world)
        {
        }
        
        public override void WorldComponentTick()
        {
            ShootingStarManager.Tick();
        }
    }
}