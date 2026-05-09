using Astralum.Astronomy.Sky;
using RimWorld.Planet;

namespace Astralum.World
{
    public class AstralumStarfieldWorldComponent : WorldComponent
    {
        public AstralumStarfieldWorldComponent(RimWorld.Planet.World world) : base(world)
        {
        }
        
        public override void WorldComponentUpdate()
        {
            base.WorldComponentUpdate();
            
            AstralumWorldSkyboxManager.Apply();
        }
    }
}