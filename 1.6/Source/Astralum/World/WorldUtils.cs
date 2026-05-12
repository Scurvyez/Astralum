using Astralum.Astronomy.LocalSystem.Stars;
using RimWorld;
using UnityEngine;
using Verse;

namespace Astralum.World
{
    public static class WorldUtils
    {
        public static Vector3 GalacticPole => Quaternion.Euler(
            GenCelestial.CurSunPositionInWorldSpace()) * Vector3.up;
        
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