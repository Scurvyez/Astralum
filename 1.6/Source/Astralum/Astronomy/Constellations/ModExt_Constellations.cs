using Verse;

namespace Astralum.Astronomy.Constellations
{
    public class ModExt_Constellations : DefModExtension
    {
        public int constellationCount = 13;
        public int maxPlacementAttempts = 80;
        public float baseStarSize = 0.25f;
        public float brightStarSize = 0.85f;
        public float constellationSizeMin = 3.0f;
        public float constellationSizeMax = 3.5f;
        public float minViewRotationAngle = 160f;
        public float maxViewRotationAngle = 200f;
    }
}