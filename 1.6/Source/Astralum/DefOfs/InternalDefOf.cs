using Astralum.Astronomy.Constellations;
using RimWorld;
using Verse;

namespace Astralum.DefOfs
{
    [DefOf]
    public class InternalDefOf
    {
        #region ShaderTypeDefs
        public static ShaderTypeDef Astra_Sun01;
        public static ShaderTypeDef Astra_BackgroundStar01;
        public static ShaderTypeDef Astra_Constellation01;
        public static ShaderTypeDef Astra_ConstellationHoverRing01;
        public static ShaderTypeDef Astra_Nebulae01;
        public static ShaderTypeDef Astra_ShootingStar01;
        public static ShaderTypeDef Astra_SkyCoordinateGrid01;
        #endregion
        
        #region GlobalWorldDrawLayerDefs
        public static GlobalWorldDrawLayerDef Astra_BackgroundStars;
        public static GlobalWorldDrawLayerDef Astra_Constellations;
        public static GlobalWorldDrawLayerDef Astra_Nebulae;
        public static GlobalWorldDrawLayerDef Astra_ShootingStars;
        #endregion
        
        #region ConfigDefs
        public static ConstellationNameSetDef Astra_ConstellationNames;
        #endregion
        
        static InternalDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
        }
    }
}