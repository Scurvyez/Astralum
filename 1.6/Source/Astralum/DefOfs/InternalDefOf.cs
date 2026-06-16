using RimWorld;
using Verse;

namespace Astralum.DefOfs
{
  [DefOf]
  public class InternalDefOf
  {
    static InternalDefOf()
    {
      DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
    }
    
    #region Jobs

    public static JobDef UseTelescope;
    
    #endregion

    #region RulePacks
    
    public static RulePackDef Astra_NebulaName_Catalog;
    public static RulePackDef Astra_NebulaName_Coordinates;
    public static RulePackDef Astra_NebulaName_Descriptive;
    public static RulePackDef Astra_NebulaName_Discoverer;
    public static RulePackDef Astra_NebulaName_Formal;
    
    #endregion

    #region ShaderTypeDefs

    public static ShaderTypeDef Astra_Sun01;
    public static ShaderTypeDef Astra_BackgroundStar01;
    public static ShaderTypeDef Astra_Constellation01;
    public static ShaderTypeDef Astra_ConstellationHoverRing01;
    public static ShaderTypeDef Astra_Nebulae01;
    public static ShaderTypeDef Astra_ShootingStar01;
    public static ShaderTypeDef Astra_SkyCoordinateGrid01;
    public static ShaderTypeDef Astra_Pulsar01;
    public static ShaderTypeDef Astra_BlackHole01;
    public static ShaderTypeDef Astra_GalacticDustLane01;

    #endregion

    #region GlobalWorldDrawLayerDefs

    public static GlobalWorldDrawLayerDef Astra_BackgroundStars;
    public static GlobalWorldDrawLayerDef Astra_Constellations;
    public static GlobalWorldDrawLayerDef Astra_Nebulae;
    public static GlobalWorldDrawLayerDef Astra_ShootingStars;
    public static GlobalWorldDrawLayerDef Astra_Pulsars;
    public static GlobalWorldDrawLayerDef Astra_BlackHoles;
    public static GlobalWorldDrawLayerDef Astra_GalacticDustLanes;

    #endregion
  }
}