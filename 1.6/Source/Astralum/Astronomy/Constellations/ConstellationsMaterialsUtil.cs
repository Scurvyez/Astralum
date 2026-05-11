using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
    [StaticConstructorOnStartup]
    public static class ConstellationsMaterialsUtil
    {
        public static readonly Material ConstellationStar;
        public static readonly Material ConstellationLine;
        
        static ConstellationsMaterialsUtil()
        {
            Shader conStarShader = InternalDefOf.Astra_ConstellationStar01.Shader;
            Shader conLineShader = InternalDefOf.Astra_ConstellationLine01.Shader;
            
            ConstellationStar = new Material(conStarShader)
            {
                name = "Astralum_ConstellationStar01"
            };

            ConstellationLine = new Material(conLineShader)
            {
                name = "Astralum_ConstellationLine01"
            };
        }
    }
}