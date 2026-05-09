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
                AstralumStarWorldComponent comp = world?.GetComponent<AstralumStarWorldComponent>();
                
                return comp?.Star;
            }
        }
    }
}