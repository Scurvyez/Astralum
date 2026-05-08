using RimWorld;
using Verse;

namespace Astralum.DefOfs
{
    [DefOf]
    public class InternalDefOf
    {
        #region ShaderTypeDefs
        public static ShaderTypeDef Astra_Sun01;
        #endregion
        
        static InternalDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
        }
    }
}