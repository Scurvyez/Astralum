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
        #endregion
        
        static InternalDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
        }
    }
}