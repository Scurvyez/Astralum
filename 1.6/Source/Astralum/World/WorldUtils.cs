using Astralum.Astronomy.Stars;
using Verse;

namespace Astralum.World
{
    public static class WorldUtils
    {
        public static SavedStar CurrentStar
        {
            get
            {
                RimWorld.Planet.World world = Find.World;
                AstralumWorldComponent comp = world?.GetComponent<AstralumWorldComponent>();
                
                return comp?.Star;
            }
        }
    }
}