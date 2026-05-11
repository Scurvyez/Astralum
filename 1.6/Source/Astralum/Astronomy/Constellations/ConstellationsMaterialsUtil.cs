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
            /*ConstellationStar = MaterialPool.MatFrom(
                "Astralum/Constellations/constellationStar01", 
                InternalDefOf.Astra_ConstellationStar01.Shader);*/
            
            Shader shader = InternalDefOf.Astra_ConstellationStar01.Shader;

            ConstellationStar = new Material(shader)
            {
                name = "Astralum_ConstellationStar01"
            };
            
            ConstellationLine = MaterialPool.MatFrom(
                "Astralum/Constellations/constellationLine01", 
                InternalDefOf.Astra_ConstellationLine01.Shader);
        }
    }
}