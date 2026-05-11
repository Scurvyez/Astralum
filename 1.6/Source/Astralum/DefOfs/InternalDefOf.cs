using RimWorld;
using Verse;

namespace Astralum.DefOfs
{
    [DefOf]
    public class InternalDefOf
    {
        #region ShaderTypeDefs
        public static ShaderTypeDef Astra_Sun01;
        public static ShaderTypeDef Astra_Starfield01;
        public static ShaderTypeDef Astra_ConstellationStar01;
        public static ShaderTypeDef Astra_ConstellationLine01;
        #endregion
        
        static InternalDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
        }
    }
}